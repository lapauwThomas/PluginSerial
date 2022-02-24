using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSerialFW;

namespace PluginSerial.Recipes
{
    public class ComportRecipe : SerialPortRecipe
    {

        public string Port { get; private set; }

        private Process proc = null;


        public bool ProcessRunning
        {
            get
            {
                return ! (proc?.HasExited ?? true);
            }
        }




        public override void Invoke(string currentPort)
        {
            if(!currentPort.Equals(Port)) return; //sanity check


            proc = ShellInvoker.CreateAndInvokeProcess(ProcessPath, ProcessArguments, currentPort);
        }
    }
}
