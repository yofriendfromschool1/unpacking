using System;
using LogicDeobfuscator.Interfaces;

namespace LogicDeobfuscator.Core
{
    public class Logger : ILogger
    {
        
        public void Information(object msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[INFO]: {msg}");
            Console.ResetColor();
        }

        public void Success(object msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SUCCESS]: {msg}");
            Console.ResetColor();
        }

        public void Warning(object msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[WARN]: {msg}");
            Console.ResetColor();
        }

        public void Error(object msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"[ERROR]: {msg}");
            Console.ResetColor();
        }
    }
}