using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using PluginSerialLib;
using PluginSerialLib.Recipes;

namespace PluginSerialFormsGui
{
    public class RecipeFolderMonitor
    {


        private static Logger logger = LogManager.GetCurrentClassLogger();

        public readonly string RecipeFolderPath;

        FileSystemWatcher folderWatcher;
        private bool AutoReloadRecipesOnChange { get; set; } = true;

        private RecipeCollection recipeCollection;
        RecipeSerializer ser = new RecipeSerializer();

        internal RecipeFolderMonitor(string recipeFolderPath, RecipeCollection collection)
        {
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


            // folderWatcher.Changed += OnRecipesFolderChanged;
            folderWatcher.Created += OnRecipesFolderChanged;
            folderWatcher.Deleted += OnRecipesFolderChanged;
            folderWatcher.Renamed += OnRecipesFolderChanged;


            folderWatcher.Filter = $"*{ConfigManager.DefaultRecipeExtension}";
            folderWatcher.EnableRaisingEvents = true;

            this.recipeCollection = collection;
            LoadFromFolder(recipeFolderPath);
        }

        public bool WriteRecipeToFile(RecipeBase recipe, bool overwrite = false)
        {
            if (recipeCollection.HasRecipeWithName(recipe.Name) && !overwrite) return false;

            WriteRecipeToFile(recipe, RecipeFolderPath);
            return true;

        }
        public void WriteRecipeToFile(RecipeBase recipe, string path, string OverrideFilename = null)
        {
            //folderWatcher.EnableRaisingEvents = false;
            ser.RecipeToFile(recipe, path, OverrideFilename);
            //folderWatcher.EnableRaisingEvents = true;
        }

        private void OnRecipesFolderChanged(object sender, FileSystemEventArgs e)
        {
            if (!AutoReloadRecipesOnChange) return;
            LoadFromFolder(RecipeFolderPath);
        }

        private void LoadFromFolder(string path)
        {
            string[] allfiles = Directory.GetFiles(path, $"*{ConfigManager.DefaultRecipeExtension}", SearchOption.TopDirectoryOnly);

            List<RecipeBase> recipes = new List<RecipeBase>();

            foreach (string file in allfiles)
            {
                try
                {
                    recipes.Add(ser.RecipeFromFile(file));
                    logger.Trace($"Added recipe from file [{file}");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Could not load recipe from file [{file}");
                }
            }

            recipeCollection.RebuildCollection(recipes);

        }

    }


}
