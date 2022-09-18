using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PluginSerialFormsGui;
using PluginSerialFormsGui.Properties;
using PluginSerialLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Foundation.Collections;

namespace PluginSerialFormsGui
{
    public partial class BaseForm : Form
    {
        private RecipeManager recipeManager;
        private RecipeFolderMonitor folderMonitor;
        private NoticationIconManager notificationIconManager;

        public NotifyIcon notificationIcon;

        private const string AppName = "PluginSerial";
        string exePath => Path.GetDirectoryName(Application.ExecutablePath);
        private string tempDir => Path.Combine(exePath, "temp_data");

        private Dictionary<string, ToastCallback> pendingToastCallbacks =
            new Dictionary<string, ToastCallback>();

        public BaseForm()
        {
            InitializeComponent();

            string recipePath = Properties.Settings.Default.RecipePath;


            if (string.IsNullOrEmpty(recipePath))
            {
                recipePath = Path.Combine(exePath, "Recipes");
            }

            Directory.CreateDirectory(tempDir);
            DirectoryInfo dir = new DirectoryInfo(tempDir);
            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }



            SerialPortManager.GetSerialPortManager().SubscribeWMI();

            recipeManager = new RecipeManager();
            recipeManager.StopAllProcessesOnDispose = true;
            folderMonitor = new RecipeFolderMonitor(recipePath, recipeManager.RecipeCollection);
            notificationIconManager = new NoticationIconManager(this, recipeManager, SerialPortManager.GetSerialPortManager());
            notificationIconManager.OnExitClicked += () => Environment.Exit(0);
            notificationIconManager.OnStartupClicked += OnStartupClicked;
            notificationIconManager.ChangeRecipePath += ChangeRecipePath;

            notificationIconManager.CreateRecipe += CreateRecipe;

            recipeManager.OnRecipeStarted += RecipeManagerOnOnRecipeStarted;
            recipeManager.OnQueryRecipeExecution += RecipeManagerQuery;
            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;



            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            new ToastContentBuilder()
                .AddText($"Started Plugin Serial")
                .AddAppLogoOverride(new Uri(Path.Combine(exePath,"icons8-rs-232-female-100.png")))
                .SetToastDuration(ToastDuration.Short)
                .Show(); 


        }

        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            // Obtain the arguments from the notification
            ToastArguments args = ToastArguments.Parse(e.Argument);
            // Obtain any user input (text boxes, menu selections) from the notification
            ValueSet userInput = e.UserInput;
            foreach (var toastArgument in args)
            {
                if (pendingToastCallbacks.ContainsKey(toastArgument.Value))
                {
                    pendingToastCallbacks[toastArgument.Value]?.Invoke(e);
                    pendingToastCallbacks.Remove(toastArgument.Value);
                }
            }
           
            //if(userInput.ContainsKey("run"))

        }

        private void RecipeManagerOnOnRecipeStarted(object sender, RecipeManager.RecipeExecutedEventArgs e)
        {
            // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
            string filepath = Path.Combine(tempDir, Path.ChangeExtension(e.Recipe.Name, ".bmp"));
            e.Recipe.Icon.Save(filepath);

            new ToastContentBuilder()
                .AddArgument("action", "RunningRecipe")
                .SetToastDuration(ToastDuration.Short)
                .AddAppLogoOverride(new Uri(filepath))
                .AddText($"Running recipe: {e.Recipe.Name} on Port: {e.Port.Port}")
                .Show(); // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 5, your TFM must be net5.0-windows10.0.17763.0 or greater

        }

        private void RecipeManagerQuery(object sender, RecipeManager.QueryRecipeEventArgs e)
        {

            var toastSelectionBox = new ToastSelectionBox("recipe")
            {
                DefaultSelectionBoxItemId = "0",
            };

            IList<ToastSelectionBoxItem> items = new List<ToastSelectionBoxItem>();
            int index = 0;
            foreach (var recipe in e.availableRecipes)
            {
                toastSelectionBox.Items.Add(new ToastSelectionBoxItem($"{index++}", recipe.Name));
                if(index == 5) break;
            }

            pendingToastCallbacks.Add("RunRecipe", new RecipeQueryToastCallback(e.availableRecipes, e.portinst, recipeManager));


            new ToastContentBuilder()
                .AddText($"Multiple recipes are available for this port")
                .AddAppLogoOverride(new Uri(Path.Combine(exePath, "icons8-rs-232-female-100.png")))
                .SetToastDuration(ToastDuration.Long)
                .AddToastInput(toastSelectionBox)
                .AddButton(new ToastButton()
                    .SetContent("Run")
                    .AddArgument("action", "RunRecipe")
                    .SetBackgroundActivation())

                .AddText($"For more recipes use the tray icon menu")
                .Show(); // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 5, your TFM must be net5.0-windows10.0.17763.0 or greater

        }

        private class RecipeQueryToastCallback : ToastCallback
        {
            private List<RecipeBase> recipes;
            private SerialPortInst port;
            private RecipeManager recipeManager;
            public RecipeQueryToastCallback(List<RecipeBase> recipes, SerialPortInst port, RecipeManager recipeManager)
            {
                this.recipes = recipes;
                this.port = port;
                this.recipeManager = recipeManager;
            }
            public override void Invoke(ToastNotificationActivatedEventArgsCompat e)
            {
                int recipeIndex = int.Parse( (string)e.UserInput["recipe"]);
                recipeManager.ExecuteRecipe(recipes[recipeIndex], port);

            }
        }



        private void CreateRecipe()
        {
            var creator = new RecipeCreator(recipeManager, folderMonitor);
            creator.Show();
        }

        private void ChangeRecipePath()
        {
            this.Invoke((MethodInvoker)delegate
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();

                dialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                dialog.IsFolderPicker = true;
                var res = dialog.ShowDialog(this.Handle);
                if (res == CommonFileDialogResult.Ok)
                {
                    string recipePath = dialog.FileName;
                    Properties.Settings.Default.RecipePath = recipePath;
                    Properties.Settings.Default.Save();
                    folderMonitor = new RecipeFolderMonitor(recipePath, recipeManager.RecipeCollection);
                }
            });
        }

        private void OnStartupClicked(bool autostart)
        {


            Settings.Default.AutoStart = autostart;
            Settings.Default.Save();

            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (autostart)
                    rk.SetValue(AppName, Application.ExecutablePath);
                else
                    rk.DeleteValue(AppName, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting start with windows", ex.Message, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {
            CreateRecipe();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ToastNotificationManagerCompat.Uninstall();
            DirectoryInfo dir = new DirectoryInfo(tempDir);
            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

        }
    }
}
