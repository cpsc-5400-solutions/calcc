using System;

namespace CalcC
{
    class Program
    {
        static void Main(string[] _)
        {
            const string expr = "2 2 +";

            var compiler = new CalcC();
            compiler.CompileToCil(expr);
            Console.WriteLine("Generated CIL code:");
            Console.WriteLine(compiler.Cil);
            Console.WriteLine();

            compiler.AssembleToObjectCode();

            // See comments in CalcC_Internal.cs about this method.
            // compiler.WriteDll("/tmp", "Test");

            var result = compiler.ExecuteObjectCode();
            Console.WriteLine($"Output is {result}");
        }
    }
}
