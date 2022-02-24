using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSerial;
using System.Drawing;

namespace PluginSerialFW
{

    public enum RecipeRuntype{ Ask, AutoRunIfOnly, AutorunFinal, Disabled}
    public abstract class SerialPortRecipe
    {


        public const string serialPlaceholderString = "{PORT}";


        public abstract bool RecipeIsValid(SerialPortInst port);

        public abstract bool TryInvokeRecipe(SerialPortInst port);

        public RecipeRuntype runType;


        public string Name;
        public string Description;
        public Icon Icon;



        public string ProcessPath { get; private set; }
        public List<string>  ProcessArguments { get; private set; }
        public abstract void Invoke(string currentPort);

        //public abstract void OnDisconnect();

    }
}
