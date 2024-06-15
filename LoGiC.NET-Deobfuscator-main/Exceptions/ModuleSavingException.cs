using System;

namespace LogicDeobfuscator.Exceptions
{
    public class ModuleSavingException : Exception
    {
        public ModuleSavingException(string message, Exception innerException) : base(message, innerException) { }
    }
}