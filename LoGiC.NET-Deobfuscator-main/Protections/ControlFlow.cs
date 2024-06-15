using System;
using System.Linq;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Core;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class ControlFlow : IProtection
    {
        public string Name => nameof(ControlFlow);
        public void Execute(Context ctx)
        {
            foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods).ToArray())
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions && m.Body.HasVariables))
                {
                    foreach (var variable in method.Body.Variables.Where(v => v.Type == ctx.Module.ImportAsTypeSig(typeof(InsufficientMemoryException))).ToArray())
                    {
                        method.Body.Variables.Remove(variable);
                    }
                    method.RemoveUselessNops();
                    method.Body.SimplifyBranches();
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (!instr[i].IsLdcI4() || !instr[i + 1].IsStloc() || !instr[i + 2].IsLdcI4() || !instr[i + 3].IsLdcI4() || !instr[i + 4].IsLdcI4() || instr[i + 5].OpCode != OpCodes.Xor || !instr[i + 6].IsLdcI4() || !instr[i + 8].IsLdcI4() || !instr[i + 9].IsStloc() || instr[i + 12].OpCode != OpCodes.Nop) continue;
                        i++;
                        do
                        {
                            method.Body.Instructions.RemoveAt(i);
                        } while (instr[i].OpCode != OpCodes.Nop);
                    }
                    method.Body.OptimizeBranches();
                    method.RemoveUselessNops();
                }
            }
        }
    }
}