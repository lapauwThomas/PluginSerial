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
    public class ComportRecipe : RecipeBase
    {


        public override bool RecipeIsValid(SerialPortInst port)
        {
            return port.Port.Equals(Filter.Port);
        }

        [JsonObject]
        public class ComportFilter
        {
            public string Port { get; set; }

            public ComportFilter(string port)
            {
                Port = port;
            }
        }

        [JsonProperty] public ComportFilter Filter;

    }
}
