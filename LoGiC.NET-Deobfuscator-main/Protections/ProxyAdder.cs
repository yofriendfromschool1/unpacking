using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LogicDeobfuscator.Core;
using LogicDeobfuscator.Models;
using LogicDeobfuscator.Interfaces;

namespace LogicDeobfuscator.Protections
{
    // I have no idea what I'm really doing, had to rewrite this code like 80 times and still only partially works kek
    // If anyone wants to fix this shit, please do. I have given up on proxy calls lmfao
    // Don't judge plz, k thnx bai.
    // LINQ GO BRRR
    public class ProxyAdder : IProtection
    {
        public string Name => nameof(ProxyAdder);
        private static bool IsProxyCallMethod(IMethod method, MethodDef currentMethod)
        {
            // Check if method is valid.
            if (method == null || currentMethod == null || !method.IsMethodDef || method.DeclaringType != currentMethod.DeclaringType)
            {
                return false;
            }
            
            // Check if parameter count matches with proxy method's parameter count.
            var callMethodParamCount = method.GetParamCount();
            var cmpCount = currentMethod.GetParamCount();
            if (callMethodParamCount != cmpCount)
            {
                return false;
            }
            var methodDef = method.ResolveMethodDef();
            
            // Check if resolved method has body, instructions and if call instruction count is 1 and that all instruction count is proxy method parameter count + 2 (call instruction and ret instruction)
            return methodDef.HasBody && methodDef.Body.HasInstructions && methodDef.Body.Instructions.Count(instr => instr.OpCode == OpCodes.Call) == 1 && methodDef.Body.Instructions.Count == callMethodParamCount + 2;
        }

        private static IEnumerable<MethodDef> FindProxyCallMethods(ModuleDef module)
        {
            // Find all methods that are proxy methods.
            return (from type in module.GetTypes().Where(t => t.HasMethods) from method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions) let instr = method.Body.Instructions from instruction in instr.Where(i => i.OpCode == OpCodes.Call && i.GetOperand() is MethodDef callMethod && IsProxyCallMethod(method, callMethod)) select method).ToList();
        }
        
        private static IEnumerable<MethodDef> FindOriginalMethods(ModuleDef module, MethodDef[] proxyCallMethods)
        {
            // Find all methods that are the begging of a proxy method (the original method that gets called).
            return (from type in module.GetTypes().Where(t => t.HasMethods) from method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions) let anyCall = GetCallMethodFromMethod(method) where anyCall != null && IsProxyCallMethod(method, anyCall) && NoProxyMethodCallsMethod(proxyCallMethods, method) select method).ToList();
        }
        
        private static bool NoProxyMethodCallsMethod(IEnumerable<MethodDef> ProxyMethods, IMDTokenProvider method)
        {
            // Loop through all provided methods.
            foreach (var proxyMethod in ProxyMethods)
            {
                var instr = proxyMethod.Body.Instructions;
                foreach (var instruction in instr)
                {
                    // If instruction is not call and operand doesnt go to a method definition continue
                    if (instruction.OpCode != OpCodes.Call || instruction.GetOperand() is not MethodDef callMethod) continue;
                    
                    // If the instructions operand is method and said method points to the provided method return false because a proxy method calls provided method.
                    if (callMethod == method)
                    {
                        return false;
                    }
                }
            }
            // None of the provided methods call provided method.
            return true;
        }

        private static MethodDef GetCallMethodFromMethod(MethodDef methodDef)
        {
            // Returns first call method from method
            var instr = methodDef.Body.Instructions;
            foreach (var instruction in instr)
            {
                // If instruction has opcode call and the operand is a method definition return it.
                if (instruction.OpCode == OpCodes.Call && instruction.GetOperand() is MethodDef callMethod)
                {
                    return callMethod;
                }
            }
            return null;
        }
        
        private static MethodDef FollowCallUntilLastMethod(MethodDef methodDef)
        {
            // It follows all call instructions that call method definitions until the last one and returns it.
            var currentMethod = methodDef;
            var currentCallMethod = GetCallMethodFromMethod(currentMethod);
            for (;;)
            {
                if (IsProxyCallMethod(currentMethod, currentCallMethod))
                {
                    currentMethod = currentCallMethod;
                    currentCallMethod = GetCallMethodFromMethod(currentMethod);
                    continue;
                }
                break;
            }
            return currentCallMethod ?? currentMethod;
        }

        public void Execute(Context ctx)
        {
            foreach (var type in ctx.Module.GetTypes().Where(t => t.HasMethods))
            {
                // Clean them methods boii. (Just in case, also remove nops because i noticed sometimes there was nop at start of method body).
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    method.De4dotBlocks();
                    method.RemoveUselessNops();
                }
            }
            
            // Find Proxy Methods.
            var proxyCallMethods = FindProxyCallMethods(ctx.Module).ToArray();
            
            // Get Original Methods from Proxy Methods.
            var originalMethods = FindOriginalMethods(ctx.Module, proxyCallMethods.ToArray()).ToArray();
            
            // Resolve The Original Methods to their Original Method Bodies.
            foreach (var orgMethod in originalMethods)
            {
                var lastCall = FollowCallUntilLastMethod(orgMethod);
                var possibleRealLastCall = GetCallMethodFromMethod(lastCall);
                orgMethod.Body = possibleRealLastCall != null ? possibleRealLastCall.Body : lastCall.Body;
            }
        }
    }
}
