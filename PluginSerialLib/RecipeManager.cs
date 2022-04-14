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
    public class RecipeManager
    {
        private Dictionary<string, SerialPortRecipe> runningRecipes = new Dictionary<string, SerialPortRecipe>();

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private RecipeCollection recipeCollection;

        public EventHandler<QueryRecipeEventArgs> OnQueryRecipeExecution;
        public EventHandler<RecipeExecutedEventArgs> OnRecipeExecuted;

        public readonly string RecipeFolderPath;

        FileSystemWatcher folderWatcher;
        private bool AutoReloadRecipesOnChange { get; set; } = true;

        public RecipeManager(string recipeFolderPath)
        {
            SerialPortManager mgr = SerialPortManager.GetSerialPortManager();
            mgr.OnPortAdded += HandleNewPort;
            mgr.OnPortRemoved += HandleRemovedPort;
            recipeCollection =  new RecipeCollection();

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
            //folderWatcher.Created += OnRecipesChanged;
            //folderWatcher.Deleted += OnRecipesChanged;
            //folderWatcher.Renamed += OnRecipesChanged;


            folderWatcher.Filter = "*.json";
            folderWatcher.EnableRaisingEvents = true;


        }


        public List<SerialPortRecipe> GetAvailableRecipesForPort(SerialPortInst port)
        {
            List<SerialPortRecipe> recipelList = new List<SerialPortRecipe>();
            foreach (var recipe in recipeCollection)
            {
                if (recipe.RecipeIsValid(port))
                {
                    recipelList.Add(recipe);
                }
            }

            return recipelList;
        }


        public RecipeManager(IEnumerable<SerialPortRecipe> recipes) : this(new List<SerialPortRecipe>())
        {


        }

        public RecipeManager(List<SerialPortRecipe> recipes)
        {
            SerialPortManager mgr = SerialPortManager.GetSerialPortManager();
            mgr.OnPortAdded += HandleNewPort;
            mgr.OnPortRemoved += HandleRemovedPort;
            recipeCollection = new RecipeCollection(recipes.ToList());
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
                runningRecipes[removedPort.Port].PortDisconnected();
            }
        }

        private void OnRecipesChanged(object sender, FileSystemEventArgs e)
        {
            if (!AutoReloadRecipesOnChange) return;


                LoadFromFolder(RecipeFolderPath);
            

        }

        private void LoadFromFolder(string path)
        {
            string[] allfiles = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);
            RecipeSerializer ser = new RecipeSerializer();
            foreach (string file in allfiles)
            {
                try
                {
                    recipeCollection.Add(ser.RecipeFromFile(file));
                    logger.Trace($"Added recipe from file [{file}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex,$"Could not load recipe from file [{file}");
                }
            }

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
                        availableRecipes.Add(serialPortRecipe);
                        //ExecuteRecipe(serialPortRecipe, addedPort);
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
            else
            {
                OnQueryRecipeExecution?.Invoke(this, new QueryRecipeEventArgs(availableRecipes, args.Port));
            }
        }

        public bool ExecuteRecipe(SerialPortRecipe recipe, SerialPortInst port)
        {
            recipe.OnRecipeFinished += (sender, args) => runningRecipes.Remove(port.Port); //allow for removal on finished
            bool runstatus = recipe.TryInvokeRecipe(port);
            logger.Info($"Started Recipe \"{recipe.Name}\" for port {port.Port}");

            runningRecipes.Add(port.Port, recipe);

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
