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
                
                throw new NotImplementedException();
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
            throw new NotImplementedException();
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
