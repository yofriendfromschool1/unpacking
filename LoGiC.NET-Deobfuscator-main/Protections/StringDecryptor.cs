using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Core;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class StringDecryptor : IProtection
    {
        public string Name => nameof(StringDecryptor);
        private static string Decrypt(string str, int key)
        {
            var builder = new StringBuilder();
            foreach (var c in str)
                builder.Append((char)(c - key));
            return builder.ToString();
        }
        private static string DecryptString(string A_0, int A_2)
        {
            var stringBuilder = new StringBuilder();
            foreach (var c in A_0.ToCharArray())
            {
                stringBuilder.Append((char)(c - A_2));
            }
            return stringBuilder.ToString();
        }
        public void Execute(Context ctx)
        {
            foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods))
            {
                if (type == ctx.Module.GlobalType) continue;
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    method.Body.SimplifyBranches();
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode != OpCodes.Ldstr || !instr[i + 1].IsLdcI4() || !instr[i + 2].IsLdcI4() || !instr[i + 3].IsLdcI4() || !instr[i + 4].IsLdcI4() || !instr[i + 5].IsLdcI4() || instr[i + 6].OpCode != OpCodes.Call || instr[i + 6].Operand is not MethodDef dec) continue;
                        try
                        {
                            ctx.Module.GlobalType.Remove(dec);
                        }
                        catch
                        {
                            // Already Removed.
                        }
                        instr[i].Operand = DecryptString(instr[i].Operand.ToString(), instr[i + 2].GetLdcI4Value());
                        instr[i + 1].OpCode = OpCodes.Nop;
                        instr[i + 2].OpCode = OpCodes.Nop;
                        instr[i + 3].OpCode = OpCodes.Nop;
                        instr[i + 4].OpCode = OpCodes.Nop;
                        instr[i + 5].OpCode = OpCodes.Nop;
                        instr[i + 6].OpCode = OpCodes.Nop;
                    }
                    method.Body.OptimizeBranches();
                    method.RemoveUselessNops();
                }
            }
            foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods).ToArray())
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        checkNext:
                        if (instr[i].OpCode != OpCodes.Br || !instr[i].Operand.ToString().Contains("ldc.i4.1")) continue;
                        instr.RemoveAt(i);
                        if (instr[i] != instr.Last())
                        {
                            goto checkNext;
                        }
                    }
                }
            }
        }
    }
}