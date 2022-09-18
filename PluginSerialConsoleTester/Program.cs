using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Toolkit.Uwp.Notifications;

namespace PluginSerialFormsGui
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



            // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText("Andrew sent you a picture")
                .AddText("Check this out, The Enchantments in Washington!")
                .Show(); // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 5, your TFM must be net5.0-windows10.0.17763.0 or greater

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
                Icon = SystemIcons.Hand.ToBitmap(),
                ProcessPath = @"PuTTY.exe",
                ProcessPolicy = ProcessKillPolicy.KillOnRemoval
            };
            recipeManager.AddRecipe(puttyRecipe);

            UniversalRecipe CreateRecipe = new UniversalRecipe
            {
                Name = "Create new recipe",
                Description = "This is a test description",
                RunType = RecipeRuntype.Ask,
                Icon = SystemIcons.Hand.ToBitmap(),
                ProcessPath = @"",
                ProcessPolicy = ProcessKillPolicy.FireAndForget
            };
            recipeManager.AddRecipe(CreateRecipe);


            ComportRecipe recipe = new ComportRecipe
            {
                Name = "TestRecipe",
                Description = "This is a test description",
                RunType = RecipeRuntype.AutoRunIfOnly,
                Icon = SystemIcons.Hand.ToBitmap(),
                ProcessPath = @"PuTTY.exe",
                ProcessArguments = @"-serial {@PORT} -sercfg 57600,8,n,1,N",
                Filter =new ComportRecipe.ComportFilter("COM6"),
                ProcessPolicy = ProcessKillPolicy.FireAndForget
            };
            recipeManager.AddRecipe(recipe);


            VidPidRecipe vidpidrecipe = new VidPidRecipe
            {
                Name = "VidPidRecipe",
                Description = "This is a test description",
                RunType = RecipeRuntype.AutorunFinal,
                Icon = SystemIcons.Hand.ToBitmap(),
                ProcessPath = @"PuTTY.exe",
                ProcessArguments =  @"-serial {@PORT} -sercfg 115200,8,n,1,N" ,
                Filter = new VidPidRecipe.VidPidFilter("0403", "6001"),
                ProcessPolicy = ProcessKillPolicy.KillOnRemoval
            };
            recipeManager.AddRecipe(vidpidrecipe);

            VidPidRecipe debuggerrecipe = new VidPidRecipe
            {
                Name = "Debugger",
                Description = "Digilent debugger",
                RunType = RecipeRuntype.AutoRunIfOnly,
                Icon = SystemIcons.Hand.ToBitmap(),
                ProcessPath = @"PuTTY.exe",
                ProcessArguments =  @"-serial {@PORT} -sercfg 115200,8,n,1,N" ,
                Filter = new VidPidRecipe.VidPidFilter("0403", "6010"),
                ProcessPolicy = ProcessKillPolicy.Keep
            };
            recipeManager.AddRecipe(debuggerrecipe);

            InstancePathRecipe CopenHagen = new InstancePathRecipe
            {
                Name = "DEBUG Copenhagen",
                Description = "Digilent debugger",
                RunType = RecipeRuntype.AutoRunIfOnly,
                Icon = SystemIcons.Hand.ToBitmap(),
                ProcessPath = @"PuTTY.exe",
                ProcessArguments =  @"-serial {PORT} -sercfg 115200,8,n,1,N" ,
                Filter = new InstancePathRecipe.InstancePathFilter(@"FTDIBUS\VID_0403+PID_6010+210357AEA503B\0000"),
                ProcessPolicy = ProcessKillPolicy.FireAndForget
            };
            recipeManager.AddRecipe(CopenHagen);


            folderMonitor.WriteRecipeToFile(debuggerrecipe, folderMonitor.RecipeFolderPath);


            Console.ReadLine();
            ToastNotificationManagerCompat.Uninstall();

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
