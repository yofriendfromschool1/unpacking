using System;

namespace LogicDeobfuscator.Exceptions
{
    public class ModuleLoadingException : Exception
    {
        public ModuleLoadingException(string message, Exception innerException) : base(message, innerException) { }
    }
}