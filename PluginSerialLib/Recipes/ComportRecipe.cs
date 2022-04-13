using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PluginSerialLib;

namespace PluginSerialLib.Recipes
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ComportRecipe : SerialPortRecipe
    {
        [JsonProperty]
        public string Port { get;  set; }

        private Process proc = null;


        public override bool RecipeIsValid(SerialPortInst port)
        {
            return port.Port.Equals(Port);
        }


    }
}
