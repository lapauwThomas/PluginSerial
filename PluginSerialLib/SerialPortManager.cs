using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using PluginSerial;

namespace PluginSerialFW
{
    public sealed class SerialPortManager
    {
        private static SerialPortManager instance;

        private Dictionary<string, SerialPortInst> currentPorts;

        private List<SerialPortRecipe> recipeCollection = new List<SerialPortRecipe>();

        public EventHandler<QueryRecipeEventartargs> OnQueryRecipeExecution;

        public void AddRecipe(SerialPortRecipe recipe)
        {
            recipeCollection.Add(recipe);
        }
        private SerialPortManager()
        {
            currentPorts = SerialPortSearcher.FindSerialPortsWMI(); //init currentPorts  
        }

        public void Refresh()
        {
            currentPorts = SerialPortSearcher.FindSerialPortsWMI(); //init currentPorts  
        }

        public void Scan()
        {
            Dictionary<string, SerialPortInst> newPorts = SerialPortSearcher.FindSerialPortsWMI(); //init currentPorts

            var removedPorts = currentPorts.Except(newPorts);
            var addedPorts = newPorts.Except(currentPorts);

            foreach (var removedPort in removedPorts)
            {
                HandleRemovedPort(removedPort.Value);
            }

            foreach (var addedPort in addedPorts)
            {
                HandleNewPort(addedPort.Value);
            }

            currentPorts = newPorts;

        }

        private void HandleRemovedPort(SerialPortInst removedPort)
        {

        }

        private void HandleNewPort(SerialPortInst addedPort)
        {
            List<SerialPortRecipe> availableRecipes = new List<SerialPortRecipe>();
            foreach (SerialPortRecipe serialPortRecipe in recipeCollection)
            {
                if (serialPortRecipe.RecipeIsValid(addedPort))
                {
                    if (serialPortRecipe.runType == RecipeRuntype.AutorunFinal)
                    {
                        serialPortRecipe.TryInvokeRecipe(addedPort);
                        return;
                    }

                    if (serialPortRecipe.runType != RecipeRuntype.Disabled)
                    {
                        availableRecipes.Add(serialPortRecipe);
                    }

                }
            }

            if (availableRecipes.Count == 1)
            {
                SerialPortRecipe recipe = availableRecipes.First();
                if (recipe.runType == RecipeRuntype.AutoRunIfOnly)
                {
                    recipe.TryInvokeRecipe(addedPort);
                }
            }




        }

        public static SerialPortManager GetSerialPortManager()
        {
            if (instance == null)
            {
                instance = new SerialPortManager();
            }

            return instance;
        }


    }

    public class QueryRecipeEventartargs : EventArgs
    {
        public readonly List<SerialPortRecipe> availableRecipes;
        public readonly SerialPortInst portinst;
        public QueryRecipeEventartargs(List<SerialPortRecipe> availableRecipes, SerialPortInst port)
        {
            this.availableRecipes = availableRecipes;
            portinst = port;
        }
    }
}
