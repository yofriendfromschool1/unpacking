using System;            
using dnlib.DotNet;
using OutStringDecrypter.Core.Methods;

namespace OutStringDecrypter.Core.Helpers
{
    public class Deobfuscator
    {
        private static readonly DeobfuscateMethod[] DeobfuscateMethods;

        public ModuleDef Module { get; set; }

        static Deobfuscator()
        {
            DeobfuscateMethods = new DeobfuscateMethod[]
            {
                new StringDeobfuscator()
            };
        }

        public Deobfuscator(ModuleDef module, string filePath)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
                                 
            Module = module;
        }

        public bool Process(string filePath)
        {
            try
            {             
                foreach (var deobfuscateMethod in DeobfuscateMethods)
                    deobfuscateMethod.Deobfuscate(Module);

                Module.Write(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured: {0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return false;
        }
    }
}                     