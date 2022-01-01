using System;

namespace CalcC
{
    class Program
    {
        static void Main(string[] _)
        {
            var compiler = new CalcC();
            compiler.CompileToCil("3 4 + sx rx rx *");
            compiler.CompileToObjectCode();

            Console.WriteLine("Generated CIL code:");
            Console.WriteLine(compiler.Cil);
            Console.WriteLine();

            var result = compiler.ExecuteObjectCode();
            Console.WriteLine($"Output is {result}");
        }
    }
}
