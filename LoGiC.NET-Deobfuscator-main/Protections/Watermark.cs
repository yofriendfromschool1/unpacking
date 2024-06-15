using System.Linq;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class Watermark : IProtection
    {
        public string Name => nameof(Watermark);
        public void Execute(Context ctx)
        {
            ctx.Module.CustomAttributes.Remove(ctx.Module.CustomAttributes.First(c => c.TypeFullName.Contains("LoGiCdotNet")));
            ctx.Module.Types.Remove(ctx.Module.Types.First(t => t.FullName.Contains("LoGiCdotNet")));
        }
    }
}