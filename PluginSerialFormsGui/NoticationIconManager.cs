using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using PluginSerialFormsGui.Properties;
using PluginSerialLib;

namespace PluginSerialFormsGui
{
    internal class NoticationIconManager
    {
        public ContextMenu menu;

        public MenuItem mnuExit;
        public MenuItem mnuStartup;
        public MenuItem mnuRecipePath;

        public MenuItem mnuCreateRecipe;

        public NotifyIcon notificationIcon;

        public Action OnExitClicked;
        public Action<bool> OnStartupClicked;
        public Action ChangeRecipePath;
        public Action CreateRecipe;

        private RecipeManager recipeManager;
        private SerialPortManager portManager;
        public NoticationIconManager(BaseForm baseform, RecipeManager recipeManager, SerialPortManager portManager)
        {
            this.recipeManager = recipeManager;
            this.portManager = portManager;

            mnuExit = new MenuItem("Exit");
            mnuExit.Click += (s, e) => OnExitClicked?.Invoke();
            mnuStartup = new MenuItem("Start with windows");
            mnuStartup.Checked = Settings.Default.AutoStart;
            mnuStartup.Click += (s, e) => OnStartupClicked?.Invoke(mnuStartup.Checked);

            mnuStartup.Click += (s, e) => { mnuStartup.Checked = !mnuStartup.Checked; };

            mnuRecipePath = new MenuItem("Change recipe folder");
            mnuRecipePath.Click += (s, e) => ChangeRecipePath?.Invoke();
            mnuCreateRecipe = new MenuItem("Create recipe");
            mnuCreateRecipe.Click += (s, e) => CreateRecipe?.Invoke();

            recipeManager.RecipeCollection.CollectionChanged += RecipeCollectionOnCollectionChanged;
            portManager.OnPortAdded += OnPortAdded;
            portManager.OnPortRemoved += OnPortRemoved;



            baseform.notificationIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Hand,
                ContextMenu = menu,
                Text = "Plugin Serial"
            };
            baseform.notificationIcon.Visible = true;
            baseform.notificationIcon.MouseDown += NotificationIcon_MouseDown;


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

        }




        private void RecipeCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //RebuildContextMenu();

        }

        public void RebuildContextMenu()
        {

            menu = new ContextMenu();

            menu.MenuItems.Add(mnuCreateRecipe);

            menu.MenuItems.Add(mnuStartup);
            menu.MenuItems.Add(mnuRecipePath);
            menu.MenuItems.Add("-");

            if (portManager.AvailablePorts.Count > 0)
            {
                foreach (var portInst in portManager.AvailablePorts)
                {
                    menu.MenuItems.Add(ContextMenuForPort(portInst));
                }
            }
            else
            {
                menu.MenuItems.Add(new MenuItem("No Ports Available") { Enabled = false });
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
            var runningRecipe = recipeManager.GetRunningRecipe(port);
            if (runningRecipe != null)
            {
                var killRecipe = new MenuItem($"Kill running recipe: \n{runningRecipe.Recipe.Name}");
                killRecipe.Click += (s, e) => { recipeManager.KillRecipe(port); };
                menu.MenuItems.Add(killRecipe);
            }

            return menu;

        }
    }
}
