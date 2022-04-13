using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginSerialLib
{
    static class PathExeHelper
    {
        public static bool FindExeEnvironmentPath(string exe, out string fullpath)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        path = Path.Combine(path, exe);
                        if (File.Exists(path))
                        {
                            fullpath = Path.GetFullPath(path);
                            return true;
                        }
                    }
                }

                fullpath = "";
                return false;
            }
            fullpath = Path.GetFullPath(exe);
            return true;
        }


    }
}
