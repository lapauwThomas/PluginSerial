using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginSerialLib
{
    internal static class IconBase64Converter
    {
        public static string IconToString(Icon image)
        {
            MemoryStream memory = new MemoryStream();
            image.Save(memory);
            string base64 = Convert.ToBase64String(memory.ToArray());
            memory.Close();

            return base64;
        }
        public static Icon StringToIcon(string base64)
        {
            MemoryStream memory = new MemoryStream(Convert.FromBase64String(base64));
            Icon result = new Icon(memory);
            memory.Close();

            return result;
        }
    }
}
