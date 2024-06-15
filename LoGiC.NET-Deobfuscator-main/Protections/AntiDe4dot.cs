using System.Linq;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class AntiDe4dot : IProtection
    {
        public string Name => nameof(AntiDe4dot);
        public void Execute(Context ctx)
        {
            foreach (var type in ctx.Module.GetTypes().Where(t => t.FullName.Contains("Form") && t.HasInterfaces && t.Interfaces.Count == 2).ToArray())
            {
                ctx.Module.Types.Remove(type);
            }
        }
    }
}