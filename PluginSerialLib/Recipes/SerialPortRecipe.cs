using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSerialLib;
using System.Drawing;
using Newtonsoft.Json;

namespace PluginSerialLib
{

    public enum RecipeRuntype{ Ask, AutoRunIfOnly, AutorunFinal, Disabled}

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class SerialPortRecipe
    {


        public const string serialPlaceholderString = "{PORT}";

        public string RecipePath = null;

        [JsonProperty]
        public string RecipeType { 
            get
            {
                return this.GetType().Name;
            } 
        }

        [JsonProperty]
        public string Name;

        [JsonProperty]
        public string Description;

        [JsonProperty]
        public RecipeRuntype RunType;

        public Icon Icon;

       

        [JsonProperty]
        public bool KillOnDisconnect;

        [JsonProperty]
        public string ProcessPath { get;  set; }

        [JsonProperty]
        public IList<string>  ProcessArguments { get; set; }



        [JsonProperty]
        protected string ICON_BASE64
        {
            get
            {
                return IconBase64Converter.IconToString(Icon);
            }
            set
            {
                Icon = IconBase64Converter.StringToIcon(value);
            }
        }

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
