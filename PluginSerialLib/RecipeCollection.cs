using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSerialLib.Recipes;

namespace PluginSerialLib
{
    internal class RecipeCollection: IEnumerable<SerialPortRecipe>
    {
        private List<InstancePathRecipe> instancePathRecipes;
        private List<VidPidRecipe> vidPidRecipes;
        private List<VidRecipe> vidRecipes;
        private List<ComportRecipe> comportRecipes;
        private List<UniversalRecipe> universalRecipes;

        public RecipeCollection(IEnumerable<SerialPortRecipe> recipes): this()
        {
            foreach (var serialPortRecipe in recipes)
            {
                this.Add(serialPortRecipe);
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

        public List<SerialPortRecipe> ToList()
        {
            var recipelists = new List<IEnumerable>(){ instancePathRecipes, vidPidRecipes, vidRecipes, comportRecipes, universalRecipes };

            var output = new List<SerialPortRecipe>();
            foreach (var list in recipelists)
            {
                output.Concat(list.Cast<SerialPortRecipe>());
            }

            return output;
        }

        public void Add(SerialPortRecipe recipe)
        {
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
        public void Remove(SerialPortRecipe recipe)
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
        }

        public IEnumerator<SerialPortRecipe> GetEnumerator()
        {
            return this.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            
            return GetEnumerator();
        }
    }
}
