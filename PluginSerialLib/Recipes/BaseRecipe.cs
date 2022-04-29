using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSerialLib;
using System.Drawing;

namespace PluginSerialLib
{

    public enum RecipeRuntype{ Ask, AutoRunIfOnly, AutorunFinal, Disabled}
    public abstract class BaseRecipe
    {


        public const string serialPlaceholderString = "{PORT}";

        public virtual RecipeRuntype RunType
        {
            get;
            set;
        }


        public string Name;
        public string Description;
        public Icon Icon;

        public bool KillOnDisconnect;


        public string ProcessPath { get;  set; }
        public List<string>  ProcessArguments { get; set; }



        private Process proc = null;


        public bool ProcessRunning
        {
            get
            {
                return !(proc?.HasExited ?? true);
            }
        }

        public void KillProcess()
        {
            proc.Kill();
        }


        public abstract bool RecipeIsValid(SerialPortInst port);



        public bool TryInvokeRecipe(SerialPortInst port)
        {
            if (!RecipeIsValid(port)) return false;

            try
            {
                Invoke(port.Port);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Invoke(string currentPort)
        {
            proc = ShellInvoker.CreateAndInvokeProcess(ProcessPath, ProcessArguments, currentPort);
        }

        //public abstract void OnDisconnect();
        public void InvokeDisconnect()
        {
            if (KillOnDisconnect)
            {
                KillProcess();
            }
        }

    }
}
