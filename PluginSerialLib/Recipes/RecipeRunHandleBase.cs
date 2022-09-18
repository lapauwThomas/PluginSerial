using System;
using System.Diagnostics;

namespace PluginSerialLib
{
    public abstract class RecipeRunHandleBase
    {
        public abstract bool IsRunning { get; }
        public SerialPortInst Port { get; }

        public RecipeBase Recipe { get; private set; }

        public RecipeRunHandleBase(RecipeBase recipe, SerialPortInst port)
        {
            Recipe = recipe;
            Port = port;
        }

        public void PortDisconnect()
        {
            if(Recipe.ProcessPolicy == ProcessKillPolicy.KillOnRemoval) EndRecipe();
        }

        public abstract void EndRecipe();

        public static RecipeRunHandleBase CreateRecipeRunHandle(RecipeBase recipe, SerialPortInst port, Process proc = null)
        {
            if (proc != null)
            {
                return new RecipeProcessHandle(recipe, port, proc);
            }
            else
            {
                return new RecipeRunHandle(recipe, port);
            }
        }
    }

    public class RecipeRunHandle : RecipeRunHandleBase
    {
        public override bool IsRunning
        {
            get { return _running; }
        }

        private bool _running;
        public override void EndRecipe()
        {
            _running = false;
            Recipe.RecipeEnded();
        }

        public RecipeRunHandle(RecipeBase recipe, SerialPortInst port) : base(recipe, port)
        {
            _running = true;
        }
    }

    public class RecipeProcessHandle : RecipeRunHandleBase
    {

        public override bool IsRunning
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

        private readonly Process CurrentProcess;


        public RecipeProcessHandle(RecipeBase recipe, SerialPortInst port, Process proc):base(recipe, port)
        {
            CurrentProcess = proc;
            if (proc != null)
            {
                proc.EnableRaisingEvents = true;
                proc.Exited += (sender, args) => EndRecipe(); ;
            }
        }

        public override void EndRecipe()
        {
            try
            {
                if (CurrentProcess != null && !CurrentProcess.HasExited)
                {
                    //kill the process, should trigger "Exited" eventhandler
                    CurrentProcess.Kill();
                    CurrentProcess.WaitForExit();
                    CurrentProcess.Dispose();
                }
                else
                {
                    //if there was no process, trigger the cleanup
                    Recipe.RecipeEnded();
                }
            }
            catch
            {
                Recipe.RecipeEnded();
            }
           
        }
        /// <summary>
        /// Force the recipe to finish. Will kill the process if its running
        /// </summary>
        public void KillSilently()
        {
            try
            {
                if (CurrentProcess != null)
                {
                    CurrentProcess.EnableRaisingEvents = false;
                    CurrentProcess.Kill();
                }
            }
            catch (Exception ex)
            {

            }
        }

    }

}