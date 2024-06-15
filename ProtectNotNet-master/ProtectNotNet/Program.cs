using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Text;

namespace ProtectNotNet
{
    class Program
    {
        static string Author()
        {
            return "lool";
        }

        public static int count = 0;

        static string methodname = String.Empty;

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Protect.NET Deobfuscator [by lool]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Choose Assembly:");
            var filename = Console.ReadLine();
            ModuleDefMD md = ModuleDefMD.Load(filename);
            if (SearchStringDecryptor(md))
            {
                DecryptStrings(md);

                md.Write(FixName(filename));
                Console.WriteLine("Decrypted " + count + " strings!");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Saved to: " + FixName(filename));

            } else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not identificate Protect.NET Obfuscator, if this is an Assembly protected by Protect.NET contact me on Github");
            }
            Console.ReadKey();


        }
        public static bool SearchStringDecryptor(ModuleDefMD md)
        {
            foreach (var type in md.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) { continue; }
                    CilBody body = method.Body;
                    for (int i = 0; i < body.Instructions.Count; i++)
                    {
                        if (body.Instructions[i].OpCode == OpCodes.Nop && body.Instructions[i + 1].OpCode == OpCodes.Newobj
                            && body.Instructions[i + 2].OpCode == OpCodes.Stloc_0 && body.Instructions[i+3].OpCode == OpCodes.Ldc_I4_0
                            && body.Instructions[i+4].OpCode == OpCodes.Stloc_1 && body.Instructions[i+5].OpCode == OpCodes.Br_S)
                        {
                            methodname = method.Name;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Found string decryption method");
                            return true;
                        }

                    }
                }
            }
            return false;
        }
        private static void DecryptStrings(ModuleDefMD md)
        {
            foreach (var type in md.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) { continue; }
                    CilBody body = method.Body;
                    for (int i = 0; i < body.Instructions.Count; i++)
                    {
                        if (body.Instructions[i].OpCode == OpCodes.Ldstr && body.Instructions[i + 1].OpCode == OpCodes.Ldstr
                            && body.Instructions[i + 2].OpCode == OpCodes.Call && body.Instructions[i + 2].Operand.ToString().Contains(methodname))
                        {
                            var param1 = body.Instructions[i].Operand.ToString();
                            var param2 = body.Instructions[i + 1].Operand.ToString();
                            body.Instructions[i].Operand = decryptionmethod(param1, param2);

                            body.Instructions[i + 1].OpCode = OpCodes.Nop;
                            body.Instructions[i + 1].Operand = null;
                            body.Instructions[i + 2].OpCode = OpCodes.Nop;
                            body.Instructions[i + 2].Operand = null;
                            count++;
                        }
                    }
                }
            }
        }
        private static string decryptionmethod(string text, string key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                uint num = (uint)c;
                int index = i % key.Length;
                char c2 = key[index];
                uint num2 = (uint)c2;
                uint num3 = num ^ num2;
                char value = (char)num3;
                stringBuilder.Append(value);
            }
            return stringBuilder.ToString();
        }
        private static string FixName(string name)
        {
            if(name.Contains(".exe"))
            {
                name = name.Replace(".exe", "-deobf.exe");

            }
            else if(name.Contains(".dll"))
            {
                name = name.Replace(".dll", "-deobf.dll");
            }
            return name;
        }


    }

}

