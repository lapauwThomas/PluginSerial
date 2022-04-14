using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PluginSerialLib.Recipes
{
    [JsonObject(MemberSerialization.OptIn)]
    public class InstancePathRecipe : RecipeBase
    {

      

        [JsonProperty] public InstancePathFilter Filter;
        public override bool RecipeIsValid(SerialPortInst port)
        {
            return port.InstancePath.Equals(Filter.InstancePath);
        }

        [JsonObject]
        public class InstancePathFilter
        {
            public string InstancePath { get; set; }

            public InstancePathFilter(string instancePath)
            {
                InstancePath = instancePath;
            }
        }

    }
}
