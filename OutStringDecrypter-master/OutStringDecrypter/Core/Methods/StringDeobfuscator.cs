using System;
using dnlib.DotNet;
using System.Reflection;
using dnlib.DotNet.Emit;

namespace OutStringDecrypter.Core.Methods
{
    [DeobfuscateMethodAttribute("Decrypts all strings in an assembly.")]
    public class StringDeobfuscator : DeobfuscateMethod
    {      
        private MethodDef FindStringDecryptionMethod(ModuleDef moduleDef)
        {
            foreach (var type in moduleDef.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;

                    var instructions = method.Body.Instructions;
                    if (instructions.Count >= 30)
                        continue;

                    var hasLibraryString = false;
                    var hasDecryptionString = false;

                    foreach (var instruction in instructions)
                    {
                        if (instruction.OpCode.Code != Code.Ldstr)
                            continue;

                        var operandValue = instruction.Operand.ToString();
                        if (!hasLibraryString)
                            hasLibraryString = operandValue.Equals("Protect.NET_Lib.Encryption");
                        else
                            hasDecryptionString = operandValue.Equals("Decrypt");

                        if (hasLibraryString && hasDecryptionString)
                            return method;
                    }
                }
            }

            return null;
        }

        private int DecryptStrings(ModuleDef moduleDef, IMDTokenProvider decryptionMethod, IFullName declaringType)
        {
            var assembly = Assembly.LoadFile(moduleDef.Location);
            var decryptCount = 0;

            foreach (var type in moduleDef.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;

                    var instructions = method.Body.Instructions;

                    for (var i = 0; i < instructions.Count; i++)
                    {
                        if (instructions[i].OpCode != OpCodes.Ldstr)
                            continue;

                        if (instructions[i + 1].OpCode != OpCodes.Ldstr)
                            continue;

                        if (!instructions[i + 2].Operand.ToString().
                            Equals(decryptionMethod.ToString()))
                            continue;

                        var param1 = instructions[i].Operand.ToString();
                        var param2 = instructions[i + 1].Operand.ToString();

                        var methodType = assembly.GetType(declaringType.Name);
                        if (methodType == null)
                            continue;

                        var metaData = decryptionMethod.MDToken.ToInt32();
                        var methodBase = methodType.Module.ResolveMethod(metaData);
                        if (methodBase == null)
                            continue;

                        var parameters = methodBase.GetParameters();
                        if (parameters.Length == 0)
                            continue;

                        var result
                            = methodBase.Invoke(null, new object[] { param1, param2 });

                        var body = method.Body;

                        body.Instructions[i].OpCode = OpCodes.Ldstr;
                        body.Instructions[i].Operand = result.ToString();

                        body.Instructions.RemoveAt(i + 1);
                        body.Instructions.RemoveAt(i + 1);

                        decryptCount++;
                    }
                }
            }

            return decryptCount;
        }

        public override void Deobfuscate(ModuleDef moduleDef)
        {                                           
            Console.WriteLine("Cleaning strings..");

            var decryptionMethod = FindStringDecryptionMethod(moduleDef);
            if (decryptionMethod == null)
                return;

            var declaringType = decryptionMethod.DeclaringType;

            var cleanedCount = DecryptStrings(moduleDef, decryptionMethod, declaringType);
            Console.WriteLine("Successfully decrypted {0} strings..", cleanedCount);
        }
    }
}        