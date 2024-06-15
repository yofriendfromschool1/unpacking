using System;
using System.IO;
using dnlib.DotNet;
using OutStringDecrypter.Core.Helpers;

namespace OutStringDecrypter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 || !File.Exists(args[0]))
                return;

            var filePath = args[0];

            Console.WriteLine("Loading module..");
            var module = ModuleDefMD.Load(filePath);

            Console.WriteLine("Initializing deobfuscator..");
            var deobfuscator = new Deobfuscator(module, filePath);

            var outputPath = Misc.GetOutPath(filePath);
            var result = deobfuscator.Process(outputPath);

            var message = result
                ? "Successfully deobfuscated.."
                : "Failed to deobfuscate..";

            Console.WriteLine(message);
            Console.Read();
        }
    }
}