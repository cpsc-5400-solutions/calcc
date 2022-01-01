using System;
using System.IO;
using System.Runtime.Loader;
using Mobius.ILasm.Core;

//
// This file includes the more technical part of the CalcC compiler.
// At this time, you needn't worry about what's going on in here,
// though I've commented it in case you're curious.
//

namespace CalcC
{
    public sealed partial class CalcC : IDisposable
    {
        // A MemoryStream (like an in-memory file) to hold the
        // compiled bytes after the CIL is assembled.
        public MemoryStream ObjectCodeStream { get; private set; }

        // Does the work of assembling the CIL we generated
        // into the actual bytes of a .NET assembly.
        public void AssembleToObjectCode()
        {
            var logger = new MobiusLogger();
            var driver = new Driver(logger, Driver.Target.Dll, showParser: false, debuggingInfo: false, showTokens: false);

            ObjectCodeStream = new MemoryStream();
            driver.Assemble(new[] { Cil }, ObjectCodeStream);
            ObjectCodeStream.Seek(0, SeekOrigin.Begin);
        }

        // Not used right now, but if you want to actually write
        // out your program to disk, call
        //  `compiler.WriteDll("/tmp/MyProgram.dll")`
        public void WriteDll(string path)
        {
            using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
            ObjectCodeStream.WriteTo(file);
        }

        // A utility function that runs the assembled object code
        // on the fly and captures the output to a string.  This is
        // pretty neat code here... it really shows off the dynamic
        // underpinnings of .NET.
        public string ExecuteObjectCode()
        {
            if (ObjectCodeStream is null)
            {
                throw new InvalidOperationException("Must call CompileToObjectCode() first");
            }

            // Set up an execution context, load our assembled object code,
            // and find the entrypoint (Main).
            var assemblyContext = new AssemblyLoadContext(null);
            var assembly = assemblyContext.LoadFromStream(ObjectCodeStream);
            var entryPoint = assembly.EntryPoint;

            // Capture the current STDOUT stream, then redirect STDOUT to a
            // memory stream so we can capture the output of our object code.
            var oldStdout = Console.Out;
            using var memStream = new MemoryStream();
            using var memWriter = new StreamWriter(memStream) { AutoFlush = true };
            Console.SetOut(memWriter);

            // Execute the compiled program.
            entryPoint?.Invoke(null, Array.Empty<object>());

            // Set STDOUT back to normal.
            Console.SetOut(oldStdout);

            // Rewind the memory stream to the beginning, then read what's in it.
            memStream.Seek(0, SeekOrigin.Begin);
            using var memReader = new StreamReader(memStream);
            var result = memReader.ReadLine();

            return result;
        }

        // MemoryStream is an IDisposable, so we need to make sure it gets
        // disposed of properly
        public void Dispose()
        {
            ObjectCodeStream?.Dispose();
        }
    }
}