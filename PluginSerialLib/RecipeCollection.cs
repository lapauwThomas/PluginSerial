using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSerialLib.Recipes;

namespace PluginSerialLib
{
    public class RecipeCollection: ObservableCollection<IEnumerable<RecipeBase>>, IEnumerable<RecipeBase>
    {
        private List<InstancePathRecipe> instancePathRecipes;
        private List<VidPidRecipe> vidPidRecipes;
        private List<VidRecipe> vidRecipes;
        private List<ComportRecipe> comportRecipes;
        private List<UniversalRecipe> universalRecipes;


        public bool ReplaceRecipeIfSameName = true;
        /// <summary>
        /// Rebuild collection from previous collection
        /// </summary>
        /// <param name="recipes">Existing recipeCollection</param>
        public RecipeCollection(IEnumerable<RecipeBase> recipes): this()
        {
            foreach (var serialPortRecipe in recipes)
            {
                AddRecipe(serialPortRecipe);
            }
        }

        public RecipeCollection()
        {
            instancePathRecipes = new List<InstancePathRecipe>();
            vidPidRecipes = new List<VidPidRecipe>();
            vidRecipes = new List<VidRecipe>();
            comportRecipes = new List<ComportRecipe>();
            universalRecipes = new List<UniversalRecipe>();

        }

        public void RebuildCollection(IEnumerable<RecipeBase> recipes)
        {
            //instancePathRecipes = new List<InstancePathRecipe>();
            //vidPidRecipes = new List<VidPidRecipe>();
            //vidRecipes = new List<VidRecipe>();
            //comportRecipes = new List<ComportRecipe>();
            //universalRecipes = new List<UniversalRecipe>();

            foreach (var serialPortRecipe in recipes)
            {
                AddRecipe(serialPortRecipe);
            }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, recipes));
        }


        public List<RecipeBase> ToList()
        {
            var recipelists = new List<IEnumerable>(){ instancePathRecipes, vidPidRecipes, vidRecipes, comportRecipes, universalRecipes };

            var output = new List<RecipeBase>();
            foreach (var list in recipelists)
            {
                var casted = list.Cast<RecipeBase>();
                output.AddRange(casted);
            }

            return output;
        }

        public void AddRange(IEnumerable<RecipeBase> recipes)
        {
            foreach (var serialPortRecipe in recipes)
            {
                AddRecipe(serialPortRecipe);
            }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, recipes));
        }

        public void Add(RecipeBase recipe)
        {
            AddRecipe(recipe);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<RecipeBase>() { recipe }));
        }
        private void AddRecipe(RecipeBase recipe)
        {
            var currentItems = ToList();
            if (currentItems.Any(si => si.Name.Equals(recipe.Name)))
            {
                var existing = currentItems.FirstOrDefault(si => si.Name.Equals(recipe.Name));
                if (ReplaceRecipeIfSameName)
                {
                    RemoveRecipe(existing);
                }
                else
                {
                    return;
                }
            }
            switch (recipe)
            {
                case InstancePathRecipe instancePathRecipe:
                    instancePathRecipes.Add(instancePathRecipe);
                    break;
                case VidPidRecipe vidPidRecipe:
                    vidPidRecipes.Add(vidPidRecipe);
                    break;
                case VidRecipe vidIdRecipe:
                    vidRecipes.Add(vidIdRecipe);
                    break;
                case ComportRecipe comportRecipe:
                    comportRecipes.Add(comportRecipe);
                    break;
                case UniversalRecipe universalRecipe:
                    universalRecipes.Add(universalRecipe);
                    break;
                default:
                    throw new ArgumentException("Recipe not supported");

            }
         
        }

        public void Remove(RecipeBase recipe)
        {
            RemoveRecipe(recipe);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<RecipeBase>() { recipe }));

        }
        public void RemoveRange(IEnumerable<RecipeBase> recipes)
        {
            foreach (var serialPortRecipe in recipes)
            {
                RemoveRecipe(serialPortRecipe);
            }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, recipes));
        }

        private void RemoveRecipe(RecipeBase recipe)
        {
            switch (recipe)
            {
                case InstancePathRecipe instancePathRecipe:
                    instancePathRecipes.Remove(instancePathRecipe);
                    break;
                case VidPidRecipe vidPidRecipe:
                    vidPidRecipes.Remove(vidPidRecipe);
                    break;
                case VidRecipe vidIdRecipe:
                    vidRecipes.Remove(vidIdRecipe);
                    break;
                case ComportRecipe comportRecipe:
                    comportRecipes.Remove(comportRecipe);
                    break;
                case UniversalRecipe universalRecipe:
                    universalRecipes.Remove(universalRecipe);
                    break;
                default:
                    throw new ArgumentException("Recipe not supported");

            }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<RecipeBase>() { recipe }));
        }

        public new IEnumerator<RecipeBase> GetEnumerator()
        {
            return this.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            
            return GetEnumerator();
        }
    }
}
