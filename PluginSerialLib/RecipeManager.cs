using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace PluginSerialLib
{
    public class RecipeManager
    {
        private Dictionary<string, SerialPortRecipe> runningRecipes = new Dictionary<string, SerialPortRecipe>();

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private List<SerialPortRecipe> recipeCollection;

        public EventHandler<QueryRecipeEventArgs> OnQueryRecipeExecution;
        public EventHandler<RecipeExecutedEventArgs> OnRecipeExecuted;

        public readonly string RecipeFolderPath;
        FileSystemWatcher folderWatcher;

        public RecipeManager(string recipeFolderPath)
        {
            SerialPortManager mgr = SerialPortManager.GetSerialPortManager();
            mgr.OnPortAdded += HandleNewPort;
            mgr.OnPortRemoved += HandleRemovedPort;
            recipeCollection =  new List<SerialPortRecipe>();

            RecipeFolderPath = recipeFolderPath;
            if (!Directory.Exists(RecipeFolderPath))
            {
                Directory.CreateDirectory(RecipeFolderPath);
            }

            folderWatcher = new FileSystemWatcher(RecipeFolderPath);

            folderWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite;


            folderWatcher.Changed += OnRecipesChanged;
            folderWatcher.Filter = "*.json";


        }



        public RecipeManager(IEnumerable<SerialPortRecipe> recipes) : this(new List<SerialPortRecipe>())
        {


        }

        public RecipeManager(List<SerialPortRecipe> recipes)
        {
            SerialPortManager mgr = SerialPortManager.GetSerialPortManager();
            mgr.OnPortAdded += HandleNewPort;
            mgr.OnPortRemoved += HandleRemovedPort;
            recipeCollection = recipes.ToList();
        }


        public void AddRecipe(SerialPortRecipe recipe)
        {
            recipeCollection.Add(recipe);
        }

        private void HandleRemovedPort(object sender, SerialPortManager.PortChangedEventArgs args)
        {
            SerialPortInst removedPort = args.Port;
            if (runningRecipes.ContainsKey(removedPort.Port))
            {
                runningRecipes[removedPort.Port].KillProcess();
                runningRecipes.Remove(removedPort.Port);
            }
        }

        private void OnRecipesChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private void ReloadRecipes(string path)
        {

        }

        private void HandleNewPort(object sender, SerialPortManager.PortChangedEventArgs args)
        {
            SerialPortInst addedPort = args.Port;
            List<SerialPortRecipe> availableRecipes = new List<SerialPortRecipe>();
            foreach (SerialPortRecipe serialPortRecipe in recipeCollection)
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
                        ExecuteRecipe(serialPortRecipe, addedPort);
                    }

                }
            }

            if (availableRecipes.Count == 1)
            {
                SerialPortRecipe recipe = availableRecipes.First();
                if (recipe.RunType == RecipeRuntype.AutoRunIfOnly)
                {
                    ExecuteRecipe(recipe, addedPort);
                }
            }
        }

        public bool ExecuteRecipe(SerialPortRecipe recipe, SerialPortInst port)
        {
            bool runstatus = recipe.TryInvokeRecipe(port);
            logger.Info($"Started Recipe \"{recipe.Name}\" for port {port.Port}");

            //keep track of running recipes that might need killing.
            if (recipe.KillOnDisconnect)
            {
                runningRecipes.Add(port.Port, recipe);
            }

            return runstatus;
        }



        public class QueryRecipeEventArgs : EventArgs
        {
            public readonly List<SerialPortRecipe> availableRecipes;
            public readonly SerialPortInst portinst;

            public QueryRecipeEventArgs(List<SerialPortRecipe> availableRecipes, SerialPortInst port)
            {
                this.availableRecipes = availableRecipes;
                portinst = port;
            }
        }

        public class RecipeExecutedEventArgs : EventArgs
        {
            public readonly SerialPortRecipe Recipe;
            public readonly SerialPortInst Port;

            public RecipeExecutedEventArgs(SerialPortRecipe recipe, SerialPortInst port)
            {
                Recipe = recipe;
                Port = port;
            }
        }
    }
}
