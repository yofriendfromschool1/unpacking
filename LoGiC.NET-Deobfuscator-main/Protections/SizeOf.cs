using System.Linq;
using System.Runtime.InteropServices;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class SizeOf : IProtection
    {
        public string Name => nameof(SizeOf);
        public void Execute(Context ctx)
        {
            foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode != OpCodes.Sizeof) continue;
                        instr[i] = Instruction.CreateLdcI4(Marshal.SizeOf(instr[i].Operand));
                        if (instr[i + 1].OpCode != OpCodes.Add) continue;
                        instr[i + 1].OpCode = OpCodes.Nop;
                        instr[i - 1] = Instruction.CreateLdcI4(instr[i - 1].GetLdcI4Value() + instr[i].GetLdcI4Value());
                    }
                }
            }
        }
    }
}