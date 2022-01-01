using System;
using System.IO;
using System.Runtime.Loader;
using Mobius.ILasm.Core;

namespace CalcC
{
    public sealed partial class CalcC : IDisposable
    {
        public MemoryStream ObjectCodeStream { get; private set; }

        public void CompileToObjectCode()
        {
            var logger = new MobiusLogger();
            var driver = new Driver(logger, Driver.Target.Dll, showParser: false, debuggingInfo: false, showTokens: false);

            ObjectCodeStream = new MemoryStream();
            driver.Assemble(new[] { Cil }, ObjectCodeStream);
            ObjectCodeStream.Seek(0, SeekOrigin.Begin);
        }

        public void WriteDll(string path)
        {
            using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
            ObjectCodeStream.WriteTo(file);
        }

        public string ExecuteObjectCode()
        {
            if (ObjectCodeStream is null)
            {
                throw new InvalidOperationException("Must call CompileToObjectCode() first");
            }

            var assemblyContext = new AssemblyLoadContext(null);
            var assembly = assemblyContext.LoadFromStream(ObjectCodeStream);
            var entryPoint = assembly.EntryPoint;

            // Redirect STDOUT to a memory stream so we can record what the compiled
            // program is printing out.
            using var memStream = new MemoryStream();
            using var memWriter = new StreamWriter(memStream) { AutoFlush = true };
            Console.SetOut(memWriter);

            // Execute the compiled program.
            entryPoint?.Invoke(null, Array.Empty<object>());

            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });

            memStream.Seek(0, SeekOrigin.Begin);
            using var memReader = new StreamReader(memStream);
            var result = memReader.ReadLine();

            return result;
        }

        public void Dispose()
        {
            ObjectCodeStream?.Dispose();
        }
    }
}