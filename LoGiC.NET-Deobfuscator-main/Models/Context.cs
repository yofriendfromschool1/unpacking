using System;
using System.Reflection;
using dnlib.DotNet;
using LogicDeobfuscator.Core;
using LogicDeobfuscator.Exceptions;
using ILogger = LogicDeobfuscator.Interfaces.ILogger;

namespace LogicDeobfuscator.Models
{
    public class Context
    {
        public ILogger Logger { get; }
        public string OutputPath { get; }
        public string InputPath { get; }
        public Assembly Asm { get; }
        public ModuleDefMD Module { get; }
        public Context(string filePath)
        {
            Logger = new Logger();
            InputPath = filePath;
            OutputPath = InputPath.Insert(InputPath.Length - 4, "-Deobfuscated");
            try
            {
                Asm = Assembly.UnsafeLoadFrom(InputPath);
                Module = ModuleDefMD.Load(InputPath);
                Logger.Success($"Loaded Module {Module.Name}!");
            }
            catch (Exception ex)
            {
                if (Module == null)
                {
                    throw new ModuleLoadingException($"Error loading Module from {filePath}!", ex);
                }
                throw;
            }
        }
    }
}