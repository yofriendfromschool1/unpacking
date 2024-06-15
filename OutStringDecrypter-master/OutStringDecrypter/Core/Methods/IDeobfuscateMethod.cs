using dnlib.DotNet;

namespace OutStringDecrypter.Core.Methods
{
    public abstract class DeobfuscateMethod
    {
        public abstract void Deobfuscate(ModuleDef moduleDef);
    }
}    