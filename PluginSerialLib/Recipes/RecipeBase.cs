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
    public abstract class RecipeBase
    {


        public const string serialPlaceholderString = "{PORT}";

        public string RecipePath = null;

        public EventHandler<RecipeInvokedEventArgs> OnRecipeInvoked;
        public EventHandler<RecipeInvokedEventArgs> OnRecipeFinished;

        private bool _isInvoked = false;

        public class RecipeProcessHandle
        {

            public bool IsRunning
            {
                get
                {
                    if (CurrentProcess == null) //if there is no process, it is always running
                    {
                        return true;
                    }
                    return !CurrentProcess.HasExited;
                }
            }

            public readonly Process CurrentProcess;

            private RecipeBase _recipe;

            public SerialPortInst Port { get; }

            public RecipeProcessHandle(RecipeBase recipe, SerialPortInst port, Process proc)
            {
                _recipe = recipe;
                Port = port;
                CurrentProcess = proc;
                if (proc != null)
                {
                    proc.EnableRaisingEvents = true;
                    proc.Exited += (sender, args) => _recipe.RecipeFinished(); ;
                }
            }

            public void EndProcess()
            {
                if (CurrentProcess != null && !CurrentProcess.HasExited)
                {
                    //kill the process, should trigger "Exited" eventhandler
                    CurrentProcess.Kill();
                    CurrentProcess.WaitForExit();
                    CurrentProcess.Dispose();
                }
                else
                {   //if there was no process, trigger the cleanup
                    _recipe.RecipeFinished();
                }
            }
        }
        public RecipeProcessHandle ProcessHandle { get; private set; }


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
            get => IconBase64Converter.IconToString(Icon);
            set => Icon = IconBase64Converter.StringToIcon(value);
        }



        public bool ProcessRunning => ProcessHandle?.IsRunning ?? false; //if the process handle is not null, it evaluates to running. If null there is no process.


        /// <summary>
        /// Force the recipe to finish. Will kill the process if its running
        /// </summary>
        public void FinishRecipe()
        {
            ProcessHandle?.EndProcess();
        }

        /// <summary>
        /// Force the recipe to finish. Will kill the process if its running
        /// </summary>
        public void KillSilently()
        {
            try
            {
                ProcessHandle.CurrentProcess.EnableRaisingEvents = false;
                ProcessHandle?.CurrentProcess.Kill();
            }
            catch (Exception ex)
            {

            }
        }

        private void RecipeFinished()
        {
            var port = ProcessHandle.Port;
            OnRecipeFinished?.Invoke(this, new RecipeInvokedEventArgs(this, port));
            ProcessHandle = null;
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
        public bool TryInvokeRecipe(SerialPortInst port)
        {
            if (!RecipeIsValid(port)) return false;

            try
            {
                var process = InvokeProcess(port.Port);
                ProcessHandle = new RecipeProcessHandle(this, port, process);
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
            if (KillOnDisconnect)
            {
                FinishRecipe();
            }
        }

    }

    public class RecipeInvokedEventArgs : EventArgs
    {
        public SerialPortInst Port { get; private set; }

        public RecipeBase Recipe { get; private set; }

        public Type RecipeTipe { get; private set; }

        public RecipeInvokedEventArgs(RecipeBase recipe, SerialPortInst port){
            this.Recipe = recipe;
            this.Port = port;
            this.RecipeTipe = recipe.GetType();
        }
    }
}
