using System.Linq;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class Md5CheckSum : IProtection
    {
        public string Name => nameof(Md5CheckSum);
        public void Execute(Context ctx)
        {
            var globalType = ctx.Module.GlobalType;
            foreach (var method in globalType.Methods.Where(m => m.HasBody && m.Body.HasInstructions && m.Body.Instructions.Any(i => i.OpCode == OpCodes.Callvirt && i.Operand.ToString().Contains("ComputeHash"))).ToArray())
            {
                globalType.Methods.Remove(method);
            }
            var newBody = new CilBody();
            newBody.Instructions.Insert(0, Instruction.Create(OpCodes.Ret));
            globalType.FindOrCreateStaticConstructor().Body = newBody;
        }
    }
}