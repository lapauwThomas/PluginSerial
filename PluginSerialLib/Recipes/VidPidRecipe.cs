using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PluginSerialLib.Recipes
{

    [JsonObject(MemberSerialization.OptIn)]
    public class VidPidRecipe : RecipeBase
    {
        [JsonObject]
        public class VidPidFilter
        {
            public string VID { get; set; }
            public string PID { get; set; }

            public VidPidFilter(string vid, string pid)
            {
                VID = vid;
                PID = pid;
            }
        }

        [JsonProperty] public VidPidFilter Filter;

        public override bool RecipeIsValid(SerialPortInst port)
        {
            if (port is UsbSerialPortInst usbSerial)
            {
                return usbSerial.VID.Equals(Filter.VID) && usbSerial.PID.Equals(Filter.PID); ;
            }

            return false;
        }

    }
}
