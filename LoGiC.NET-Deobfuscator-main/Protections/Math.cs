using System;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Core;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class SystemMath : IProtection
    {
        public string Name => nameof(SystemMath);
        public void Execute(Context ctx)
        {
            foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    method.RemoveUselessNops();
                    method.RemoveNegativeInstructions();
                    method.Body.SimplifyBranches();
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (!instr[i].IsLdcI4() || !instr[i + 1].IsLdcI4() || instr[i + 2].OpCode != OpCodes.Call || instr[i + 3].OpCode != OpCodes.Call || instr[i + 4].OpCode != OpCodes.Call) continue;
                        if (new[]
                        {
                            ((MemberRef) instr[i + 2].Operand).Name,
                            ((MemberRef) instr[i + 3].Operand).Name,
                            ((MemberRef) instr[i + 4].Operand).Name,
                        }.Any(m => m != "Abs" && m != "Min"))
                        {
                            break;
                        }
                        instr[i] = Instruction.CreateLdcI4(Math.Abs(Math.Min(instr[i].GetLdcI4Value(), Math.Abs(Convert.ToInt32(instr[i + 1].Operand)))));
                        instr[i + 1].OpCode = OpCodes.Nop;
                        instr[i + 2].OpCode = OpCodes.Nop;
                        instr[i + 3].OpCode = OpCodes.Nop;
                        instr[i + 4].OpCode = OpCodes.Nop;
                    }
                    method.Body.OptimizeBranches();
                    method.RemoveUselessNops();
                }
            }
        }
    }
}