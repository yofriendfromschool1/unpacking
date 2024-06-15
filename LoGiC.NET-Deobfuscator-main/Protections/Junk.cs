using System.Linq;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class Junk : IProtection
    {
        public string Name => nameof(Junk);
        public void Execute(Context ctx)
        {
            foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods).ToArray())
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions && m.HasReturnType && (m.ReturnType.IsPrimitive || m.ReturnType.ToString() == "System.String") && m.Body.Instructions.Count == 2).ToArray())
                {
                    var first = method.Body.Instructions.FirstOrDefault();
                    if (first == null) continue;
                    if (first.IsLdcI4() && first.GetLdcI4Value() is >= 5 and <= 10 || first.OpCode == OpCodes.Ldstr && first.Operand.ToString().Length == 1)
                    {
                        type.Methods.Remove(method);
                    }
                }
            }
        }
    }
}