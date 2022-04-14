using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PluginSerialLib.Recipes
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VidRecipe : RecipeBase
    {
        [JsonObject]
        public class VidFilter
        {
            public string VID { get; set; }

            public VidFilter(string vid)
            {
                VID = vid;
            }
        }

        [JsonProperty] public VidFilter Filter;


        public override bool RecipeIsValid(SerialPortInst port)
        {
            if (port is UsbSerialPortInst usbSerial)
            {
                return usbSerial.VID.Equals(Filter.VID);
            }

            return false;
        }

    }
}
