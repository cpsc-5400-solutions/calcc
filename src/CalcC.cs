using System;
using System.Linq;
using static CalcC.TokenType;

namespace CalcC
{
    public partial class CalcC
    {
        public string Cil { get; set; }

        public void CompileToCil(string src)
        {
            // Emit the preamble
            var cil = Preamble();

            // Tokenize the input string (in this case,
            // just split on spaces).
            var tokens = src.Split(' ').Select(t => t.Trim());

            foreach (var token in tokens)
            {
                var tokenType = GetTokenType(token);

                // TODO:
                // Finish the code in this loop to emit
                // the correct CIL instructions based on the
                // tokenType and the token's value.
                //
                // Hint: this will likely be a big
                // switch-case statement or a bunch of
                // if-else-if statements.
                //
                // To emit the instructions, all you 
                // have to do is say
                //   `cil += "...";
                // to append the instructions to the output.
                //
                // If you get stuck, think about what the
                // code would look like in C# and use
                // sharplab.io to see what the CIL would be.
                switch (tokenType)
                {
                    case Number:
                        cil += $@"
    // Push {token} on the stack
    ldloc.0
    ldc.i4.s {token}
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)
";
                        break;

                    case BinaryOperator:
                        var instruction = token switch
                        {
                            "+" => "add",
                            "-" => "sub",
                            "*" => "mul",
                            "/" => "div",
                            "%" => "rem",
                            _ => throw new InvalidOperationException(nameof(token)),
                        };
                        cil += $@"
    // Pop two values off the stack, execute a {instruction} operation, and push the result
    ldloc.0
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    {instruction}
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)
";
                        break;

                    case UnaryOperator:
                        switch (token)
                        {
                            case "sqrt":
                                cil += $@"
    // Pop one value off the stack, execute a {token} operation, and push the result
    ldloc.0
    ldloc.0
    callvirt instance !0 class [System.Collections] System.Collections.Generic.Stack`1<int32>::Pop()
    conv.r8
    call float64[System.Private.CoreLib] System.Math::Sqrt(float64)
    conv.ovf.i4
    callvirt instance void class [System.Collections] System.Collections.Generic.Stack`1<int32>::Push(!0)
";
                                break;
                            default:
                                throw new InvalidOperationException(nameof(token));
                        }
                        break;

                    case StoreInstruction:
                        var register = token[1];
                        cil += $@"
    // Pop the stack and store it in register '{register}'
    ldloc.1
    ldc.i4.s {(int)register}
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    callvirt instance void class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32>::set_Item(!0, !1)
";
                        break;

                    case RetrieveInstruction:
                        register = token[1];
                        cil += $@"
    // Push the value of register '{register}' onto the stack
    ldloc.0
    ldloc.1
    ldc.i4.s {(int)register}
    callvirt instance !1 class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32>::get_Item(!0)
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)
";
                        break;
                }
            }

            // Emit the postamble.
            cil += Postamble();

            Cil = cil;
        }

        //
        // TODO:
        // Fill in this method so that it returns the type
        // of token represented by the string.  The token
        // types are given to you in TokenType.cs.
        private static TokenType GetTokenType(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Blank;
            }
            else if (token[0] >= '0' && token[0] <= '9')
            {
                return Number;
            }
            else if (token[0] == '-')
            {
                if (token.Length == 1)
                {
                    return BinaryOperator;
                }
                else
                {
                    return Number;
                }
            }
            else if ("+*/%".IndexOf(token[0]) > -1)
            {
                return BinaryOperator;
            }
            else if (token == "sqrt")
            {
                return UnaryOperator;
            }
            else if (token[0] == 's')
            {
                return StoreInstruction;
            }
            else if (token[0] == 'r')
            {
                return RetrieveInstruction;
            }
            else
            {
                return Unknown;
            }
        }

        // Preamble:
        // * Initialize the assembly
        // * Declare `static void main()` function
        // * Declare two local variables: the Stack and the registers Dictionary<>
        // * Call the constructors on the Stack<> and the registers Dictionary<>
        //
        // Note the @"..." string construct; this is for multiline strings.
        private static string Preamble()
        {
            return @"
// Preamble
.assembly _ { }
.assembly extern System.Collections {}
.assembly extern System.Console {}
.assembly extern System.Private.CoreLib {}

.method public hidebysig static void main() cil managed
{
    .entrypoint
    .maxstack 3

    // Declare two local vars: a Stack<int> and a Dictionary<char, int>
    .locals init (
        [0] class [System.Collections]System.Collections.Generic.Stack`1<int32> stack,
        [1] class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32> registers
    )

    // Initialize the Stack<>
    newobj instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::.ctor()
    stloc.0
    // Initialize the Dictionary<>
    newobj instance void class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32>::.ctor()
    stloc.1
";
        }

        // Postamble.  Pop the top of the stack and print whatever is there.
        private static string Postamble()
        {
            return @"
    // Pop the top of the stack and print it
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    call void [System.Console]System.Console::WriteLine(int32)

    ret
}";
        }
    }
}