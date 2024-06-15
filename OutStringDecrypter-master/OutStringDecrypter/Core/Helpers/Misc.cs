using System;
using System.IO;

namespace OutStringDecrypter.Core.Helpers
{
    public static class Misc
    {
        public static string GetExtension(this string value)
        {
            var extension = Path.GetExtension(value);
            return extension == null
                ? string.Empty
                : extension.Replace(".", string.Empty);
        }

        public static string GetOutPath(string filePath)
        {
            var baseDirectory = Path.GetDirectoryName(filePath);
            if (baseDirectory == null)
                throw new InvalidOperationException("Directory is null.");

            if (!baseDirectory.EndsWith("\\"))
                baseDirectory += "\\";

            return Path.Combine(
                baseDirectory,
                Path.GetFileNameWithoutExtension(filePath) + "_cleaned.exe");
        }
    }
}