using System;

namespace OutStringDecrypter.Core.Methods
{
    public class DeobfuscateMethodAttribute : Attribute
    {
        public string Description { get; set; }

        public DeobfuscateMethodAttribute(string description)
        {
            Description = description;
        }
    }
}