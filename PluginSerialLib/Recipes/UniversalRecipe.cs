using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PluginSerialLib.Recipes
{

    [JsonObject(MemberSerialization.OptIn)]
    public class UniversalRecipe : RecipeBase
    {
     
        public override bool RecipeIsValid(SerialPortInst port)
        {
            return true;
        }

    }
}
