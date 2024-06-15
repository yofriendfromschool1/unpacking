using System;
using LogicDeobfuscator.Core;

namespace LogicDeobfuscator
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Console.Clear();
            Console.Title = "LoGiC.NET Deobfuscator | V1.0.1 | Made by ePiC#2696";
            var deobf = new Deobfuscator(args[0]);
            deobf.Execute();
            deobf.Save();
            Console.ReadKey();
        }
    }
}