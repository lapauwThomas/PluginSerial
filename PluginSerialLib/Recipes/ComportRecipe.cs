using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSerialLib;

namespace PluginSerialLib.Recipes
{
    public class ComportRecipe : SerialPortRecipe
    {

        public string Port { get;  set; }

        private Process proc = null;


        public bool ProcessRunning
        {
            get
            {
                return ! (proc?.HasExited ?? true);
            }
        }

        public void KillProcess()
        {
            proc.Kill();
        }


        public override bool RecipeIsValid(SerialPortInst port)
        {
            return port.Port.Equals(Port);
        }


    }
}
