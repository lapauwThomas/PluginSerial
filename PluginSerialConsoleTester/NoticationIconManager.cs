using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PluginSerialLib;

namespace PluginSerialConsoleTester
{
    internal class NoticationIconManager
    {
        public ContextMenu menu;
        public MenuItem mnuExit;
        public NotifyIcon notificationIcon;

        public Action OnExitClicked;

        private RecipeManager recipeManager;
        private SerialPortManager portManager;
        public NoticationIconManager(RecipeManager recipeManager, SerialPortManager portManager)
        {
            this.recipeManager = recipeManager;
            this.portManager = portManager;

            recipeManager.RecipeCollection.CollectionChanged +=RecipeCollectionOnCollectionChanged;
            portManager.OnPortAdded += OnPortAdded;
            portManager.OnPortRemoved += OnPortRemoved;

            Thread notifyThread = new Thread(
                delegate ()
                {

                    notificationIcon = new NotifyIcon()
                    {
                        Icon = SystemIcons.Hand,
                        ContextMenu = menu,
                        Text = "Plugin Serial"
                    };
                    notificationIcon.Visible = true;
                    notificationIcon.MouseDown += NotificationIcon_MouseDown;
                    Application.Run();
                }
            );
            notifyThread.SetApartmentState(ApartmentState.STA);
            notifyThread.Start();
        }

        private void NotificationIcon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RebuildContextMenu();
            }
        }

        private void OnPortRemoved(object sender, SerialPortManager.PortChangedEventArgs e)
        {
            //RebuildContextMenu();
        }

        private void OnPortAdded(object sender, SerialPortManager.PortChangedEventArgs e)
        {
            notificationIcon.ShowBalloonTip(1000, "New Serial port", $"New port: \"{e.Port.Port}\"", ToolTipIcon.Info);
        }

        private void RecipeCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //RebuildContextMenu();

        }

        public void RebuildContextMenu()
        {

            menu = new ContextMenu();
            mnuExit = new MenuItem("Exit");
            mnuExit.Click += (s,e) => OnExitClicked?.Invoke();

            if (portManager.AvailablePorts.Count > 0)
            {
                foreach (var portInst in portManager.AvailablePorts)
                {
                    menu.MenuItems.Add(ContextMenuForPort(portInst));
                }
            }
            else
            {
                menu.MenuItems.Add(new MenuItem("No Ports Available"){Enabled = false});
            }

            menu.MenuItems.Add("-");
            menu.MenuItems.Add(mnuExit);

            notificationIcon.ContextMenu = menu;

        }

        private MenuItem ContextMenuForPort(SerialPortInst port)
        {

            var menu = new MenuItem(port.Port);

            menu.MenuItems.Add(new MenuItem(port.FriendlyName) { Enabled = false });
            menu.MenuItems.Add("-");
            foreach (var portRecipe in recipeManager.GetAvailableRecipesForPort(port))
            {

                var recipeItem = new MenuItem(portRecipe.Name);
                recipeItem.Click += (s, e) =>
                {
                    if (!recipeManager.ExecuteRecipe(portRecipe, port))
                    {
                        notificationIcon.ShowBalloonTip(1000, "Could not invoke recipe", $"Error invoking recipe \"{portRecipe.Name}\"", ToolTipIcon.Error);
                    }
                };
                menu.MenuItems.Add(recipeItem);
            }

            menu.MenuItems.Add("-");
            var runningRecipe = recipeManager.GetRunningRecipe(port) ;
            if (runningRecipe != null)
            {
                var killRecipe = new MenuItem($"Kill running recipe: \n{runningRecipe.Name}");
                killRecipe.Click += (s, e) => { recipeManager.KillRecipe(port); };
                menu.MenuItems.Add(killRecipe);
            }

            return menu;

        }
    }
}
