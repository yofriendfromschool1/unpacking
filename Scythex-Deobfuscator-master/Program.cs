using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Scythex.Deobfuscator;

internal class Program
{
    static bool JustDecrypt = true;

    static void Main(string[] Params)
    {
        Console.Title = "Scythex String Decryptor";

        ModuleContext Context = ModuleDef.CreateModuleContext();
        ModuleDefMD Module = ModuleDefMD.Load("Scythex.exe", Context);
        Assembly Assembly = Assembly.LoadFile(Path.GetFullPath(Module.FullName));

        List<string> Strings = new List<string>();

        foreach (TypeDef Type in Module.GetTypes())
        {
            foreach (MethodDef Method in Type.Methods)
            {
                if (!Method.HasBody) continue;

                foreach (Instruction Inst in Method.Body.Instructions)
                    if (Inst.OpCode == OpCodes.Ldstr)
                        Strings.Add(Inst.Operand as string);
            }
        }

        MethodDef DecryptMethod = FindMethodByName(
            Module,
            "syncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardsyncguardwrlmstamwwvanweqysflyjmbylbcxehgannmwoflnnbelaognhrsaqsrrppdnjkhgjfioqavukndemenjpgvsarfufscuwbfcib");

        foreach ((MethodDef Method, Instruction Instruction) CallInfo in FindMethodCalls(Module.Assembly, DecryptMethod))
        {
            if (JustDecrypt != true)
            {
                Console.Write("Called In", Console.ForegroundColor = ConsoleColor.White);
                Console.Write($" {CallInfo.Method.Name}\n", Console.ForegroundColor = ConsoleColor.Green);
            }

            Instruction Input = null;
            Instruction Size = null;

            for (int i =0; i < CallInfo.Method.Body.Instructions.Count; ++i)
            {
                if (CallInfo.Method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                    Input = CallInfo.Method.Body.Instructions[i];

                if (CallInfo.Method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
                    Size = CallInfo.Method.Body.Instructions[i];
            }

            if (JustDecrypt != true)
            {
                Console.Write("Input: ", Console.ForegroundColor = ConsoleColor.White);
                Console.WriteLine($"{Input.Operand as string}", Console.ForegroundColor = ConsoleColor.Blue);
                Console.Write("Size: ", Console.ForegroundColor = ConsoleColor.White);
                Console.WriteLine($"{(int)Size.Operand}", Console.ForegroundColor = ConsoleColor.Red);
            }

            try
            {
                string Output = InvokeMethod(Assembly, DecryptMethod, Input.Operand as string, (int)Size.Operand) as string;
               
                if (JustDecrypt != true) Console.WriteLine($"Output: {Output}\n", Console.ForegroundColor = ConsoleColor.Yellow);
                else Console.WriteLine(Output);
            }
            catch (Exception Ex)
            {
                Console.WriteLine($"Failed To Decrypt Method {CallInfo.Method.Name}: {Ex.Message}\n", Console.ForegroundColor = ConsoleColor.Red);
            }
        }
        Console.ReadLine();
    }

    static MethodDef FindMethodByName(ModuleDefMD Module, string Name)
    {
        foreach (TypeDef Type in Module.Types)
            foreach (MethodDef Method in Type.Methods)
                if (Method.Name == Name)
                    return Method;

        return null;
    }

    static List<(MethodDef Method, Instruction Instruction)> FindMethodCalls(AssemblyDef Assembly, MethodDef TargetMethod)
    {
        List<(MethodDef Method, Instruction Instruction)> CallSites = [];

        foreach (ModuleDef Module in Assembly.Modules)
            foreach (TypeDef Types in Module.GetTypes())
                foreach (MethodDef Method in Types.Methods)
                {
                    if (!Method.HasBody) continue;

                    foreach (Instruction Inst in Method.Body.Instructions)
                    {
                        if (Inst.OpCode == OpCodes.Call
                            || Inst.OpCode == OpCodes.Callvirt)
                        {
                            IMethod CallMethod = Inst.Operand as IMethod;
                            if (CallMethod != null && CallMethod.FullName == TargetMethod.FullName)
                                CallSites.Add((Method, Inst));
                        }
                    }
                }

        return CallSites;
    }

    static object InvokeMethod(Assembly Module, MethodDef TargetMethod, params object[] Arguments)
    {
        foreach (Type Type in Module.GetTypes())
        {
            foreach (MethodInfo Method in Type.GetMethods())
            {
                if (Method.Name == TargetMethod.Name)
                {
                    object Instance = Method.IsStatic ? null : Activator.CreateInstance(Type);
                    return Method.Invoke(Instance, Arguments);
                }
            }
        }

        return null;
    }
}
