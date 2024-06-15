using System;
using System.IO;
using Mono.Cecil;

namespace SimpleDeobfuscator
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                WriteHelp();
                return;
            }

            ProcessFile(args[0]);
        }

        const string DeobfuscatedLibSuffix = "_1";

        private static void ProcessFile(string sourceFileName)
        {
            try
            {
                // get the assembly given the path name, and get the main module 
                var assembly = AssemblyDefinition.ReadAssembly(sourceFileName);
                foreach (var module in assembly.Modules)
                    LiteralRenamer.FixNaming(module);
                
                var directoryName = Path.GetDirectoryName(sourceFileName);
                var fileName = Path.GetFileNameWithoutExtension(sourceFileName);
                var fileExt = Path.GetExtension(sourceFileName);

                var destFileName = Path.Combine(directoryName, fileName + DeobfuscatedLibSuffix + fileExt);
                assembly.Write(destFileName);
            }
            catch (Exception e)
            {
                WriteOutput($"Error while processing file:\n{e}");
            }
        }

        public static void WriteOutput(string line)
        {
            Console.WriteLine(line);
        }

        private static void WriteHelp()
        {
            WriteOutput("Usage:");
            WriteOutput($"\t{nameof(SimpleDeobfuscator)}.exe <path_to_binary file>");
            WriteOutput("Examples:");
            WriteOutput($"\t{nameof(SimpleDeobfuscator)}.exe ConsoleApp1.exe");
            WriteOutput("\tConsoleApp1.exe will be deobfuscated and saved as ConsoleApp1_1.exe.");
            WriteOutput($"\t{nameof(SimpleDeobfuscator)}.exe SomeLib.dll");
            WriteOutput("\tSomeLib.dll will be deobfuscated and saved as SomeLib_1.exe.");
        }
    }
}
