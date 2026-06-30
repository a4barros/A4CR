using System;
using System.Collections.Generic;
using System.IO;

namespace GUI
{
    public static class StartupArgs
    {
        public static IReadOnlyList<string> GetFilePaths(IEnumerable<string>? args)
        {
            if (args == null)
            {
                return Array.Empty<string>();
            }

            var paths = new List<string>();

            foreach (var arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg))
                {
                    continue;
                }

                if (arg.StartsWith("-", StringComparison.Ordinal))
                {
                    continue;
                }

                if (File.Exists(arg))
                {
                    paths.Add(arg);
                }
            }

            return paths;
        }
    }
}
