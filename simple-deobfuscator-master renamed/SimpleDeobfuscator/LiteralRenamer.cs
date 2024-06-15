using System;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace SimpleDeobfuscator
{
    public static class LiteralRenamer
    {
        public static void FixNaming(ModuleDefinition module)
        {
            foreach (var type in module.GetTypes())
                FixTypeNaming(type);

            foreach (var type in module.GetTypes())
                FixMembersNaming(type);

            foreach (var type in module.GetTypes())
                FixMethodBodyReferenceNaming(type);
        }

        private static void FixTypeNaming(TypeDefinition type)
        {
            if (type.Name == "<Module>")
                return;

            FixNaming(type, t => t.Namespace, (t, name) => t.Namespace = name, "Ns_");

            const string privateImplDetailsPrefix = "<PrivateImplementationDetails>";
            const int guidInStringLength = 36;

            if (type.Name.StartsWith(privateImplDetailsPrefix))
            {
                var switchUniqueName = "Switch_" + type.Name
                    .Substring(privateImplDetailsPrefix.Length + 1, guidInStringLength)
                    .Replace("-", "");
                type.Name = switchUniqueName;
            }
            else
            {
                var typePrefix = "";

                if (type.IsInterface) typePrefix = "Interface_";
                else if (type.IsValueType) typePrefix = "Struct_";
                else if (type.IsEnum) typePrefix = "Enum_";
                else if (type.IsException()) typePrefix = "Exception_";
                else if (type.IsSealed && type.IsAbstract) typePrefix = "StaticClass_";
                else if (type.IsClass) typePrefix = "Class_";

                FixNaming(type, t => t.Name, (t, name) => t.Name = name, typePrefix);
            }

            foreach (var nestedType in type.NestedTypes)
                FixTypeNaming(nestedType);

            foreach (var typeParameter in type.GenericParameters)
                FixNaming(typeParameter, t => t.Name, (t, name) => t.Name = name);
        }

        private static void FixMembersNaming(TypeDefinition type)
        {
            foreach (var nestedType in type.NestedTypes)
                FixMembersNaming(nestedType);

            foreach (var field in type.Fields)
                FixFieldNaming(field);

            foreach (var property in type.Properties)
                FixPropertyNaming(property);

            foreach (var method in type.Methods)
                FixMethodNaming(method);
        }

        private static void FixMethodBodyReferenceNaming(TypeDefinition type)
        {
            foreach (var nestedType in type.NestedTypes)
                FixMethodBodyReferenceNaming(nestedType);

            foreach (var method in type.Methods)
                if (method.HasBody)
                {
                    var body = method.Body;
                    foreach (var instruction in body.Instructions)
                    {
                        switch (instruction.Operand)
                        {
                            case MethodReference methodRef:
                                FixMethodNaming(methodRef);
                                break;

                            case FieldReference fieldRef:
                                FixFieldNaming(fieldRef);
                                break;
                        }
                    }
                }
        }

        private static void FixMethodNaming(MethodReference method)
        {
            if (method.Name == ".ctor" || method.Name == ".cctor")
                return;

            FixNaming(method, t => t.Name, (t, name) => t.Name = name, "_" + TypeNameToMemberPrefix(method.ReturnType.GetElementType().Name));
        }

        private static void FixFieldNaming(FieldReference field)
        {
            FixNaming(field, t => t.Name, (t, name) => t.Name = name,
                TypeNameToMemberPrefix(field.FieldType.GetElementType().Name));
        }

        private static void FixPropertyNaming(PropertyReference property)
        {
            FixNaming(property, t => t.Name, (t, name) => t.Name = name,
                TypeNameToMemberPrefix(property.PropertyType.GetElementType().Name));
        }

        private static string TypeNameToMemberPrefix(string memberName)
        {
            return memberName.Replace('.', '_').Replace('[', '_').Replace(']', '_').Replace('`', '_') + "_";
        }

        private static bool IsException(this TypeDefinition type)
        {
            try
            {
                for (var currentType = type; currentType != null; currentType = currentType.BaseType?.Resolve())
                    if (currentType.Name.Contains("Exception"))
                        return true;
            }
            catch (Exception e)
            {
                Program.WriteOutput($"Warning: unable to resolve type {type.FullName} or one of its ancestors");
                Program.WriteOutput($"\t {e.Message}");
            }

            return false;
        }

        private static void FixNaming<T>(T member, Func<T, string> getFunc, Action<T, string> setFunc,
            string namePrefix = "")
        {
            var currentName = getFunc(member);
            var newName = TryFixName(currentName);
            if (newName != currentName)
            {
                setFunc(member, namePrefix + newName);
            }
        }

        /// <summary>
        /// Fix non-printed symbols in <paramref name="name"/> (if they exists).
        /// </summary>
        private static string TryFixName(string name)
        {
            if (name.Length == 0)
                return "";

            var newName = new StringBuilder(name.Length);

            if (name[0] == '_' || IsLatinLetter(name[0]))
                newName.Append(name[0]);
            else
                newName.AppendCharAsPrintable(name[0]);

            foreach (var с in name.Skip(1))
                if (с == '_' || с == '.' || IsLatinLetterOrDigit(с))
                    newName.Append(с);
                else
                    newName.AppendCharAsPrintable(с);

            return newName.ToString();
        }

        private static bool IsLatinLetter(char c)
        {
            return c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z';
        }
        private static bool IsLatinLetterOrDigit(char c)
        {
            return c >= '0' && c <= '9' || IsLatinLetter(c);
        }

        /// <summary>
        /// Convert char <paramref name="c"/> to printable set of chars, that can be part of member name.
        /// </summary>
        private static void AppendCharAsPrintable(this StringBuilder stringBuilder, char c)
        {
            stringBuilder.Append(GetCharForCode(c & 31)); // bits 0..4
            c >>= 5;
            stringBuilder.Append(GetCharForCode(c & 63)); // bits 5..10
            c >>= 6;
            stringBuilder.Append(GetCharForCode(c & 63)); // bits 11..15
        }

        /// <summary>
        /// Convert 6-bit value to text in XXEncode-like style.
        /// </summary>
        /// <param name="code">6-bit value in interval [0..63] that will be converted to symbol.</param>
        /// <returns>Symbol, that definitely can be a part of method name.</returns>
        /// <remarks>for 5-bit value [0..31] returns only letters.</remarks>
        private static char GetCharForCode(int code)
        {
            const int latinLettersCount = 'Z' - 'A' + 1; // 27

            if (code < latinLettersCount) // 0 .. 26
                return (char)('A' + code);
            code -= latinLettersCount;
            if (code < latinLettersCount) // 27 .. 53
                return (char)('a' + code);
            code -= latinLettersCount;
            return (char)('0' + code); // 54 .. 63
        }
    }
}
