using System.Linq;
using de4dot.blocks;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace LogicDeobfuscator.Core
{
    public static class Utilities
    {
        public static bool CallsAnyMethod(this IMethod method, IMethod[] methods)
        {
            if (method is not {IsMethodDef: true})
            {
                return false;
            }

            var methodDef = method.ResolveMethodDef();
            if (methodDef is not {HasBody: true} || !methodDef.Body.HasInstructions)
            {
                return false;
            }
            
            var callsAnyMethod = false;
            methodDef.Body.SimplifyBranches();
            var instr = methodDef.Body.Instructions;
            foreach (var instruction in instr)
            {
                if (instruction.OpCode != OpCodes.Call || instruction.Operand is not MethodDef callMethod || !methods.Contains(callMethod)) continue;
                callsAnyMethod = true;
            }
            methodDef.Body.OptimizeBranches();
            return callsAnyMethod;
        }

        public static bool CanRename(this object member)
        {
            return member switch
            {
                MethodDef mDef => !mDef.IsRuntimeSpecialName && !mDef.DeclaringType.IsForwarder && !mDef.IsConstructor,
                FieldDef fDef => !fDef.IsRuntimeSpecialName && !(fDef.IsLiteral && fDef.DeclaringType.IsEnum),
                PropertyDef pDef => !pDef.IsRuntimeSpecialName && !pDef.IsEmpty && pDef.IsSpecialName,
                EventDef evDef => !evDef.IsRuntimeSpecialName,
                _ => false
            };
        }
        
        // https://github.com/DevT02/Junk-Remover/blob/master/ConsoleAppLmao/Program.cs
        public static void RemoveUselessNops(this MethodDef method)
        {
            var instr = method.Body.Instructions;
            for (var i = 0; i < instr.Count; i++)
            {
                checkNext:
                if (instr[i].OpCode != OpCodes.Nop || IsNopBranchTarget(method, instr[i]) || IsNopSwitchTarget(method, instr[i]) || IsNopExceptionHandlerTarget(method, instr[i])) continue;
                method.Body.Instructions.RemoveAt(i);
                if (instr[i] != instr.Last())
                {
                    goto checkNext;
                }
            }
        }
        
        private static bool IsNopBranchTarget(MethodDef method, Instruction nopInstr)
        {
            var instr = method.Body.Instructions;
            return (from t in instr where t.OpCode.OperandType == OperandType.InlineBrTarget || t.OpCode.OperandType == OperandType.ShortInlineBrTarget && t.Operand != null select (Instruction) t.Operand).Any(instruction2 => instruction2 == nopInstr);
        }

        private static bool IsNopSwitchTarget(MethodDef method, Instruction nopInstr)
        {
            var instr = method.Body.Instructions;
            return (from t in instr where t.OpCode.OperandType == OperandType.InlineSwitch && t.Operand != null select (Instruction[]) t.Operand).Any(source => source.Contains(nopInstr));
        }

        private static bool IsNopExceptionHandlerTarget(MethodDef method, Instruction nopInstr)
        {
            if (!method.Body.HasExceptionHandlers) return false;
            var exceptionHandlers = method.Body.ExceptionHandlers;
            return exceptionHandlers.Any(exceptionHandler => exceptionHandler.FilterStart == nopInstr || exceptionHandler.HandlerEnd == nopInstr || exceptionHandler.HandlerStart == nopInstr || exceptionHandler.TryEnd == nopInstr || exceptionHandler.TryStart == nopInstr);
        }

        public static void RemoveNegativeInstructions(this MethodDef method)
        {
            var instr = method.Body.Instructions;
            for (var i = 0; i < instr.Count; i++)
            {
                checkNext:
                if (instr[i].OpCode != OpCodes.Neg) continue;
                method.Body.Instructions.RemoveAt(i);
                if (instr[i] != instr.Last())
                {
                    goto checkNext;
                }
            }
        }
        
        // https://github.com/MindSystemm/SuperCalculator/blob/cac79b73b7a0d125c0892d0d7cd28a686e0ebf53/Program.cs#L322
        public static void De4dotBlocks(this MethodDef method)
        {
            var blocksCflowDeobfuscator = new BlocksCflowDeobfuscator();
            var blocks = new Blocks(method);
            blocksCflowDeobfuscator.Initialize(blocks);
            blocksCflowDeobfuscator.Deobfuscate();
            blocks.RepartitionBlocks();
            blocks.GetCode(out var list, out var exceptionHandlers);
            DotNetUtils.RestoreBody(method, list, exceptionHandlers);
        }
    }
}
