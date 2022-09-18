using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using PluginSerialLib.Recipes;

namespace PluginSerialLib
{
    public class RecipeManager:IDisposable
    {
        public bool StopAllProcessesOnDispose;

        private Dictionary<string, RecipeRunHandleBase> runningRecipes = new Dictionary<string, RecipeRunHandleBase>();

        private static Logger logger = LogManager.GetCurrentClassLogger();
        public RecipeCollection RecipeCollection { get; private set; }

        public event EventHandler<QueryRecipeEventArgs> OnQueryRecipeExecution;
        public event EventHandler<RecipeExecutedEventArgs> OnRecipeStarted; 

        public RecipeManager()
        {
            SerialPortManager mgr = SerialPortManager.GetSerialPortManager();
            mgr.OnPortAdded += HandleNewPort;
            mgr.OnPortRemoved += HandleRemovedPort;
            RecipeCollection =  new RecipeCollection();
        }


        public List<RecipeBase> GetAvailableRecipesForPort(SerialPortInst port)
        {
            List<RecipeBase> recipelList = new List<RecipeBase>();
            foreach (var recipe in RecipeCollection)
            {
                if (recipe.RecipeIsValid(port))
                {
                    recipelList.Add(recipe);
                }
            }

            return recipelList;
        }


        public RecipeManager(IEnumerable<RecipeBase> recipes) : this(new List<RecipeBase>())
        { }

        public RecipeManager(List<RecipeBase> recipes)
        {
            SerialPortManager mgr = SerialPortManager.GetSerialPortManager();
            mgr.OnPortAdded += HandleNewPort;
            mgr.OnPortRemoved += HandleRemovedPort;
            RecipeCollection = new RecipeCollection(recipes.ToList());
        }


        public void AddRecipe(RecipeBase recipe)
        {
            RecipeCollection.Add(recipe);
        }

        private void HandleRemovedPort(object sender, SerialPortManager.PortChangedEventArgs args)
        {
            SerialPortInst removedPort = args.Port;

            if (runningRecipes.ContainsKey(removedPort.Port))
            {
                runningRecipes[removedPort.Port].PortDisconnect();
            }
        }


        private void HandleNewPort(object sender, SerialPortManager.PortChangedEventArgs args)
        {
            SerialPortInst addedPort = args.Port;
            List<RecipeBase> availableRecipes = new List<RecipeBase>();
            foreach (RecipeBase serialPortRecipe in RecipeCollection)
            {
                if (serialPortRecipe.RecipeIsValid(addedPort))
                {
                    if (serialPortRecipe.RunType == RecipeRuntype.AutorunFinal)
                    {
                        ExecuteRecipe(serialPortRecipe, addedPort);
                        return;
                    }

                    if (serialPortRecipe.RunType != RecipeRuntype.Disabled)
                    {
                        availableRecipes.Add(serialPortRecipe);
                        //ExecuteRecipe(serialPortRecipe, addedPort);
                    }

                }
            }

            if (availableRecipes.Count == 1)
            {
                RecipeBase recipe = availableRecipes.First();
                if (recipe.RunType == RecipeRuntype.AutoRunIfOnly)
                {
                    ExecuteRecipe(recipe, addedPort);
                }
            }
            else
            {
                OnQueryRecipeExecution?.Invoke(this, new QueryRecipeEventArgs(availableRecipes, args.Port));
            }
        }

        public bool ExecuteRecipe(RecipeBase recipe, SerialPortInst port)
        {
            if (runningRecipes.ContainsKey(port.Port))
            {
                logger.Warn($"There is alread a running recipe for port {port.Port}");
                return false;
            }


            bool runstatus = recipe.TryInvokeRecipe(port, out var runHandle);
            if (runstatus)
            {
                logger.Info($"Started Recipe \"{recipe.Name}\" for port {port.Port}");
                if (runHandle != null) //if we get a handle, add it to running recipes
                {
                    runningRecipes.Add(port.Port, runHandle);

                    recipe.OnRecipeFinished += (sender, args) =>
                    {
                        runningRecipes.Remove(port.Port);
                    };
                }
                OnRecipeStarted?.Invoke(this, new RecipeExecutedEventArgs(recipe, port));
            }

            return runstatus;
        }

        /// <summary>
        /// Kill the running recipe for a port if any
        /// </summary>
        /// <param name="port"></param>
        public void KillRecipe(SerialPortInst port)
        {
            GetRunningRecipe(port)?.EndRecipe();
        }
        /// <summary>
        /// Gets the running recipe for a port. Returns null if no recipe running
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public RecipeRunHandleBase GetRunningRecipe(SerialPortInst port)
        {
            if (runningRecipes.ContainsKey(port.Port))
            {
                return runningRecipes[port.Port];
            }

            return null;
        }

      

        public class QueryRecipeEventArgs : EventArgs
        {
            public readonly List<RecipeBase> availableRecipes;
            public readonly SerialPortInst portinst;

            public QueryRecipeEventArgs(List<RecipeBase> availableRecipes, SerialPortInst port)
            {
                this.availableRecipes = availableRecipes;
                portinst = port;
            }
        }

        public class RecipeExecutedEventArgs : EventArgs
        {
            public readonly RecipeBase Recipe;
            public readonly SerialPortInst Port;

            public RecipeExecutedEventArgs(RecipeBase recipe, SerialPortInst port)
            {
                Recipe = recipe;
                Port = port;
            }
        }

        private bool disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (StopAllProcessesOnDispose)
                {
                    var procs = runningRecipes.Values.ToList();
                    foreach (var runningRecipe in procs)
                    {
                        runningRecipe.EndRecipe();
                    }
                }
                disposed = true;
            }
        }

        ~RecipeManager()
        {
            Dispose(false);
        }

      
    }
}
