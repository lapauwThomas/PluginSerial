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
            RecipeManager recipeManager = new RecipeManager();


            ComportRecipe recipe = new ComportRecipe
            {
                Name = "TestRecipe",
                Description = "This is a test description",
                RunType = RecipeRuntype.AutoRunIfOnly,
                Icon = SystemIcons.Hand,
                ProcessPath = @"PuTTY.exe",
                ProcessArguments = new List<string>{@"-serial {PORT}", @"-sercfg 57600,8,n,1,N"},
                Port = "COM3",
                KillOnDisconnect = true
            };
            recipeManager.AddRecipe(recipe);

            Thread notifyThread = new Thread(
                delegate ()
                {
                    menu = new ContextMenu();
                    mnuExit = new MenuItem("Exit");
                    menu.MenuItems.Add(0, mnuExit);

                    notificationIcon = new NotifyIcon()
                    {
                        Icon = SystemIcons.Hand,
                        ContextMenu = menu,
                        Text = "Main"
                    };
                    mnuExit.Click += new EventHandler(mnuExit_Click);

                    notificationIcon.Visible = true;
                    Application.Run();
                }
            );

            notifyThread.Start();


            


            Console.ReadLine();




        }

        static void mnuExit_Click(object sender, EventArgs e)
        {
            notificationIcon.Dispose();
            Application.Exit();
        }
    }
}
