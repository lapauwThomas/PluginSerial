using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using PluginSerialLib.Recipes;

using PluginSerialLib;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace PluginSerialConsoleTester
{
    class Program
    {


        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static ContextMenu menu;
        public static MenuItem mnuExit;
        public static NotifyIcon notificationIcon;

        static void Main(string[] args)
        {

            logger.Info("Starting PluginSerial");


            var portnames = System.IO.Ports.SerialPort.GetPortNames(); 
            
            logger.Info("Listing ports for reference");
            foreach (var port in portnames)
            {
                logger.Trace($"Port found {port}");
            }


            SerialPortManager.GetSerialPortManager().SubscribeWMI();


            string exePath = Path.GetDirectoryName(Application.ExecutablePath);
            string recipePath = Path.Combine(exePath, "Recipes");
            RecipeManager recipeManager = new RecipeManager();
            recipeManager.StopAllProcessesOnDispose = true;
            RecipeFolderMonitor folderMonitor = new RecipeFolderMonitor(recipePath, recipeManager.RecipeCollection);
            NoticationIconManager notificationIcon =  new NoticationIconManager(recipeManager, SerialPortManager.GetSerialPortManager());
            notificationIcon.OnExitClicked += () => Environment.Exit(0);

            UniversalRecipe puttyRecipe = new UniversalRecipe
            {
                Name = "Open Putty",
                Description = "This is a test description",
                RunType = RecipeRuntype.Ask,
                Icon = SystemIcons.Hand,
                ProcessPath = @"PuTTY.exe",
                KillOnDisconnect = false
            };
            recipeManager.AddRecipe(puttyRecipe);

            UniversalRecipe CreateRecipe = new UniversalRecipe
            {
                Name = "Create new recipe",
                Description = "This is a test description",
                RunType = RecipeRuntype.Ask,
                Icon = SystemIcons.Hand,
                ProcessPath = @"",
                KillOnDisconnect = false
            };
            recipeManager.AddRecipe(CreateRecipe);


            ComportRecipe recipe = new ComportRecipe
            {
                Name = "TestRecipe",
                Description = "This is a test description",
                RunType = RecipeRuntype.AutoRunIfOnly,
                Icon = SystemIcons.Hand,
                ProcessPath = @"PuTTY.exe",
                ProcessArguments = new List<string>{@"-serial {PORT}", @"-sercfg 57600,8,n,1,N"},
                Filter =new ComportRecipe.ComportFilter("COM6"),
                KillOnDisconnect = true
            };
            recipeManager.AddRecipe(recipe);


            VidPidRecipe vidpidrecipe = new VidPidRecipe
            {
                Name = "VidPidRecipe",
                Description = "This is a test description",
                RunType = RecipeRuntype.AutorunFinal,
                Icon = SystemIcons.Hand,
                ProcessPath = @"PuTTY.exe",
                ProcessArguments = new List<string> { @"-serial {PORT}", @"-sercfg 115200,8,n,1,N" },
                Filter = new VidPidRecipe.VidPidFilter("0403", "6001"),
                KillOnDisconnect = true
            };
            recipeManager.AddRecipe(vidpidrecipe);

            VidPidRecipe Debugger = new VidPidRecipe
            {
                Name = "Debugger",
                Description = "Digilent debugger",
                RunType = RecipeRuntype.AutoRunIfOnly,
                Icon = SystemIcons.Hand,
                ProcessPath = @"PuTTY.exe",
                ProcessArguments = new List<string> { @"-serial {PORT}", @"-sercfg 115200,8,n,1,N" },
                Filter = new VidPidRecipe.VidPidFilter("0403", "6010"),
                KillOnDisconnect = true
            };
            recipeManager.AddRecipe(Debugger);

            InstancePathRecipe CopenHagen = new InstancePathRecipe
            {
                Name = "DEBUG Copenhagen",
                Description = "Digilent debugger",
                RunType = RecipeRuntype.AutoRunIfOnly,
                Icon = SystemIcons.Hand,
                ProcessPath = @"PuTTY.exe",
                ProcessArguments = new List<string> { @"-serial {PORT}", @"-sercfg 115200,8,n,1,N" },
                Filter = new InstancePathRecipe.InstancePathFilter(@"FTDIBUS\VID_0403+PID_6010+210357AEA503B\0000"),
                KillOnDisconnect = true
            };
            recipeManager.AddRecipe(CopenHagen);


            folderMonitor.WriteRecipeToFile(Debugger,folderMonitor.RecipeFolderPath);


            //KnownTypesBinder knownTypesBinder = new KnownTypesBinder();

            //foreach (Type type in Assembly.GetAssembly(typeof(SerialPortRecipe)).GetTypes()
            //.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(SerialPortRecipe))))
            //{
            //    knownTypesBinder.AddType(type);
            //}
            ////knownTypesBinder.AddType(typeof(SerialPortRecipe.ProcessArguments).GetType(), "ProcessArgumentList");

            //RecipeSerializer recipeSerializer = new RecipeSerializer();


            //string filename = Path.ChangeExtension(recipe.Name, ".json");
            //string outputFile = Path.Combine(recipePath, filename);
            //recipeSerializer.RecipeToFile(recipe, outputFile);

            //SerialPortRecipe recipeDeserial = recipeSerializer.RecipeFromFile(outputFile);

            //recipeManager.AddRecipe(recipeDeserial);








            Console.ReadLine();




        }

        static void mnuExit_Click(object sender, EventArgs e)
        {
            notificationIcon.Dispose();
            Application.Exit();
        }

        public class KnownTypesBinder : ISerializationBinder
        {
            public Dictionary<string, Type> KnownTypes { get; set; } = new Dictionary<string,Type>();

            public void AddType(Type type)
            {
                KnownTypes.Add(type.GetType().Name, type);
            }
            public void AddType(Type type, string name )
            {
                KnownTypes.Add(name, type);
            }


            public Type BindToType(string assemblyName, string typeName)
            {
                return KnownTypes.SingleOrDefault(t => t.Key == typeName).Value;
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = null;
                typeName = serializedType.Name;
            }
        }
    }
}
