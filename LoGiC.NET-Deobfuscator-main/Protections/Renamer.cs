using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using LogicDeobfuscator.Core;
using LogicDeobfuscator.Interfaces;
using LogicDeobfuscator.Models;

namespace LogicDeobfuscator.Protections
{
    public class Renamer : IProtection
    {
        public string Name => nameof(Renamer);
        private readonly HashSet<string> GeneratedNames = new();
        private string GenerateName()
        {
            var rnd = new Random();
            string generated;
            do 
            {
                generated = $"dio{rnd.Next(0, 999999999)}";
            } while (GeneratedNames.Contains(generated));
            GeneratedNames.Add(generated);
            return generated;
        }
        public void Execute(Context ctx)
        {
            ctx.Module.Name = Path.GetFileNameWithoutExtension(ctx.InputPath);
            foreach (var type in ctx.Module.GetTypes())
            {
                if (type.CanRename())
                {
                    type.Name = GenerateName();
                }
                var typeMembers = new List<IMemberDef>();
                typeMembers.AddRange(type.Events);
                typeMembers.AddRange(type.Fields);
                typeMembers.AddRange(type.Methods);
                typeMembers.AddRange(type.Properties);
                typeMembers
                    .AsParallel()
                    .WithDegreeOfParallelism(typeMembers.Count)
                    .ForAll(member =>
                    {
                        if (!member.CanRename()) return;
                        member.Name = GenerateName();
                    });
            }
        }
    }
}