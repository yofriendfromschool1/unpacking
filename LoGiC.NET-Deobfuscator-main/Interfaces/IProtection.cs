using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Interfaces
{
    public interface IProtection
    {
        public string Name { get; }
        public void Execute(Context ctx);
    }
}