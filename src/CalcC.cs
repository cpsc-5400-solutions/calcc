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
            var cil = @"
.assembly _ { }

.method public hidebysig static void main() cil managed
{
    .entrypoint
    .maxstack 3
    .locals init (
        [0] class [System.Collections]System.Collections.Generic.Stack`1<int32> stack,
        [1] class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32> registers
    )

    nop
    newobj instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::.ctor()
    stloc.0
    newobj instance void class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32>::.ctor()
    stloc.1

";

            foreach (var token in src.Split(' ').Select(t => t.Trim()))
            {
                var tokenType = GetTokenType(token);

                switch (tokenType)
                {
                    case Number:
                        cil += $@"
    ldloc.0
    ldc.i4.{token}
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)
";
                        break;

                    case Operator:
                        var instruction = token switch
                        {
                            "+" => "add",
                            "-" => "sub",
                            "*" => "mul",
                            "/" => "div",
                            "%" => "rem",
                            "sqrt" => "call float64 [System.Private.CoreLib]System.Math::Sqrt(float64)",
                            _ => throw new InvalidOperationException(nameof(token)),
                        };
                        cil += $@"
    ldloc.0
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()

    {instruction}

    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)
";
                        break;

                    case StoreInstruction:
                        var register = token[1];
                        cil += $@"
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
    ldloc.0
    ldloc.1
    ldc.i4.s {(int)register}
    callvirt instance !1 class [System.Private.CoreLib]System.Collections.Generic.Dictionary`2<char, int32>::get_Item(!0)
    callvirt instance void class [System.Collections]System.Collections.Generic.Stack`1<int32>::Push(!0)
";
                        break;
                }
            }

            cil += @"
    ldloc.0
    callvirt instance !0 class [System.Collections]System.Collections.Generic.Stack`1<int32>::Pop()
    call void [System.Console]System.Console::WriteLine(int32)

    ret
}";

            Cil = cil;
        }

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
                if (token[1] >= '0' && token[1] <= '9')
                {
                    return Number;
                }
                else
                {
                    return Operator;
                }
            }
            else if ("+*/%".IndexOf(token[0]) > -1)
            {
                return Operator;
            }
            else if (token == "sqrt")
            {
                return Operator;
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
    }
}