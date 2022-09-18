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
    public enum ProcessKillPolicy { Keep, FireAndForget, KillOnRemoval, Disabled }

    [JsonObject(MemberSerialization.OptIn)]
    public abstract partial class RecipeBase
    {


        public const string serialPlaceholderString = "{@PORT}";

        public string RecipePath = null;

        public EventHandler<RecipeInvokedEventArgs> OnRecipeInvoked;
        public EventHandler<RecipeInvokedEventArgs> OnRecipeFinished;


        public RecipeRunHandleBase RunHandle { get; private set; }


        [JsonProperty]
        public string RecipeType { 
            get
            {
                return this.GetType().Name;
            } 
        }

        //[JsonProperty]
        public string Name;

        [JsonProperty]
        public string Description;

        [JsonProperty]
        public RecipeRuntype RunType;

        public Bitmap Icon;

       

        [JsonProperty]
        public ProcessKillPolicy ProcessPolicy;

        [JsonProperty]
        public string ProcessPath { get;  set; }

        [JsonProperty]
        public string  ProcessArguments { get; set; }



        [JsonProperty]
        protected string ICON_BASE64
        {
            get => IconBase64Converter.BitmapToString(Icon);
            set => Icon = IconBase64Converter.StringToBitmap(value);
        }


        public void RecipeEnded()
        {
            if (RunHandle != null)
            {
                OnRecipeFinished?.Invoke(this, new RecipeInvokedEventArgs(this, RunHandle.Port));
                RunHandle = null;
            }

        }

        /// <summary>
        /// Method to be implemented in child classes. Checks if the provided port is valid for the supplied filter
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public abstract bool RecipeIsValid(SerialPortInst port);


        /// <summary>
        /// Safely try to invoke the recipe
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool TryInvokeRecipe(SerialPortInst port, out RecipeRunHandleBase handle)
        {
            handle = null;
            if (!RecipeIsValid(port)) return false;

            try
            {
                var process = InvokeProcess(port.Port);
                if (ProcessPolicy != ProcessKillPolicy.FireAndForget)
                {
                    RunHandle = RecipeRunHandleBase.CreateRecipeRunHandle(this, port, process);
                    handle = RunHandle;

                }
                else
                {
                     //in this case, we have a succesful invoke, but no handle since it is fire and forget 
                }
                OnRecipeInvoked?.Invoke(this, new RecipeInvokedEventArgs(this, port));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private Process InvokeProcess(string currentPort)
        {
            if (!string.IsNullOrEmpty(ProcessPath))
            {
                return ShellInvoker.CreateAndInvokeProcess(ProcessPath, ProcessArguments, currentPort);
            }
            return null;
        }

        /// <summary>
        /// To be called by the recipemanager when a port is disconnected
        /// </summary>
        public void PortDisconnected()
        {
            if (ProcessPolicy.Equals(ProcessKillPolicy.KillOnRemoval))
            {
                RunHandle?.EndRecipe();
            }
        }

    }

    public class RecipeInvokedEventArgs : EventArgs
    {
        public SerialPortInst Port { get; private set; }

        public RecipeBase Recipe { get; private set; }

        public Type RecipeType { get; private set; }

        public RecipeInvokedEventArgs(RecipeBase recipe, SerialPortInst port){
            this.Recipe = recipe;
            this.Port = port;
            this.RecipeType = recipe.GetType();
        }
    }
}
