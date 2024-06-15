using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using LogicDeobfuscator.Exceptions;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;
using LogicDeobfuscator.Protections;

namespace LogicDeobfuscator.Core
{
    public class Deobfuscator
    {
        public Deobfuscator(string path) => Context = new Context(path);
        private Context Context { get; }
        
        private readonly IEnumerable<IProtection> protections = new IProtection[]
        {
            //new ProxyAdder(), Simplifies Proxy Calls. Don't expect it to work.
            new AntiDe4dot(),
            new Watermark(),
            new Md5CheckSum(),
            new Renamer(),
            new SystemMath(),
            new ControlFlow(),
            new SizeOf(),
            new StringDecryptor(),
            new Junk()
        };
        
        public void Execute()
        {
            foreach (var protection in protections)
            {
                try
                {
                    protection.Execute(Context);
                    Context.Logger.Information($"Executed protection {protection.Name}!");
                }
                catch (Exception ex)
                {
                    Context.Logger.Error($"Error Executing Protection {protection.Name}!{Environment.NewLine}{ex}");
                }
            }
        }

        public void Save()
        {
            try
            {
                if (Context.Module.IsILOnly)
                {
                    Context.Module.Write(Context.OutputPath, new ModuleWriterOptions(Context.Module) { Logger = DummyLogger.NoThrowInstance, MetadataOptions = { Flags = MetadataFlags.PreserveAll } });
                }
                else
                {
                    Context.Module.NativeWrite(Context.OutputPath, new NativeModuleWriterOptions(Context.Module, false) { Logger = DummyLogger.NoThrowInstance, MetadataOptions = { Flags = MetadataFlags.PreserveAll } });
                }
                Context.Logger.Success("Saved Module!");
            }
            catch (Exception ex)
            {
                throw new ModuleSavingException($"Error saving Module {Context.Module.Name}!", ex);
            }
        }
    }
}