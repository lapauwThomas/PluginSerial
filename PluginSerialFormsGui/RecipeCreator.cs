using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using PluginSerialFormsGui;
using PluginSerialLib;
using PluginSerialLib.Recipes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PluginSerialFormsGui
{
    public partial class RecipeCreator : Form
    {

        private Color[] _BackgroundColours =
        {
            Color.DarkCyan, Color.Coral, Color.Crimson, Color.DarkViolet, Color.Green, Color.IndianRed, Color.MidnightBlue,
            Color.Gold, Color.MediumSeaGreen
        };
       Size _defaultIconSize = new Size(48, 48);


        private bool AllValid = true;

        private RecipeManager recipeManager;
        private RecipeFolderMonitor recipeFolderMonitor;
        SerialPortManager serialPortManager;
        public RecipeCreator(RecipeManager recipeManager, RecipeFolderMonitor recipeFolderMonitor)
        {
            InitializeComponent();
            this.recipeManager = recipeManager;
            this.recipeFolderMonitor = recipeFolderMonitor;

            cb_processpolicy.DataSource = Enum.GetValues(typeof(ProcessKillPolicy));
            cb_runtype.DataSource = Enum.GetValues(typeof(RecipeRuntype));

            serialPortManager = SerialPortManager.GetSerialPortManager();
            serialPortManager.OnPortsChanged += SerialPortManager_OnPortsChanged;
        }

        private void SerialPortManager_OnPortsChanged(object sender, SerialPortManager.AvailablePortsChangedEventArgs e)
        {
    
        }

        private void tb_Name_Leave(object sender, EventArgs e)
        {

        }

        public Bitmap GenerateCircle(string name)
        {
            char first = string.IsNullOrEmpty(name) ? '\0' : name[0];
            var avatarString = string.Format("{0}", first).ToUpper();

            var randomIndex = new Random().Next(0, _BackgroundColours.Length - 1);
            var bgColour = _BackgroundColours[randomIndex];

            var bmp = new Bitmap(192, 192);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            var font = new Font("Arial", 72, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            using (Brush b = new SolidBrush(bgColour))
            {
                graphics.FillEllipse(b, new Rectangle(0, 0, 192, 192));
            }
            graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), 95, 100, sf);
            graphics.Flush();


            return bmp;
            //var ms = new MemoryStream();
            //bmp.Save(ms, ImageFormat.Png);
            //return ms;
        }

        private char NameFirstLetter;
        private void tb_Name_TextChanged(object sender, EventArgs e)
        {
            if (!customIconSet)
            {
                char first = string.IsNullOrEmpty(tb_Name.Text) ? '\0': tb_Name.Text[0];
                if (first != NameFirstLetter)
                {
                    NameFirstLetter = first;
                    pb_icon.Image = GenerateCircle(tb_Name.Text);
                    //pb_icon.SizeMode = PictureBoxSizeMode.StretchImage;

                }
            
            }
        }

        private bool customIconSet = false;
        private void lbl_changeIcon_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image files (*.bmp, *.png, *.ico)|*.bmp;*.png;*.ico|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    try
                    {
                        loadIconFromFile(filePath);
                    }
                    catch
                    {
                        MessageBox.Show("Error loading icon",
                            "There was an issue loading this icon. Check if the filetype is correct.",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void loadIconFromFile(string filePath)
        {
            pb_icon.Image =  ResizeImage(new Bitmap(filePath), _defaultIconSize);
        }

        private Type GetRecipeType()
        {
            if (rb_Universal.Checked) return typeof(UniversalRecipe);
            if (rb_portSpecific.Checked) return typeof(ComportRecipe);
            if (rb_vid.Checked) return typeof(VidRecipe);
            if (rb_vidpid.Checked) return typeof(VidPidRecipe);
            if (rb_path.Checked) return typeof(InstancePathRecipe);
            return null;
        }

        private RecipeRuntype getRuntype()
        {
            RecipeRuntype runtype;
            Enum.TryParse<RecipeRuntype>(cb_runtype.SelectedValue.ToString(), out runtype);
            return runtype; 
        }
        private ProcessKillPolicy getPolicyType()
        {
            ProcessKillPolicy processKillPolicy;
            Enum.TryParse<ProcessKillPolicy>(cb_processpolicy.SelectedValue.ToString(), out processKillPolicy);
            return processKillPolicy;
        }

        private Bitmap getIcon()
        {
            return ResizeImage(pb_icon.Image, _defaultIconSize);
        }
        private void btn_save_Click(object sender, EventArgs e)
        {
            if (AllValid)
            {
                RecipeBase recipe = (RecipeBase)Activator.CreateInstance(GetRecipeType());
                recipe.Name = tb_Name.Text;
                recipe.Description = tb_Desc.Text;
                recipe.RunType = getRuntype();
                recipe.Icon = getIcon();
                recipe.ProcessPath = tb_processpath.Text;
                recipe.ProcessArguments = tb_arguments.Text;
                recipe.ProcessPolicy = getPolicyType();

                switch(recipe)
                {
                    case ComportRecipe rcp:
                        rcp.Filter = new ComportRecipe.ComportFilter(tb_port.Text);
                        break;
                    case VidRecipe rcp:
                        rcp.Filter = new VidRecipe.VidFilter(tb_vid.Text);
                        break;
                    case VidPidRecipe rcp:
                        rcp.Filter = new VidPidRecipe.VidPidFilter(tb_vid.Text, tb_pid.Text);
                        break;
                    case InstancePathRecipe rcp:
                        rcp.Filter = new InstancePathRecipe.InstancePathFilter(tb_instancePath.Text);
                        break;
                    default:
                        break;

                }

                if (!recipeFolderMonitor.WriteRecipeToFile(recipe)) //try writ recipe
                {
                    if (MessageBox.Show(
                            "There is already a recipe with this name in the recipe folder. Do you want to overwrite?", "Recipe with this name already exists",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        recipeFolderMonitor.WriteRecipeToFile(recipe, true);

                        this.Close();
                    }
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void lblRefreshports_Click(object sender, EventArgs e)
        {
            UpdatePortList();
        }

        private void UpdatePortList()
        {
            cb_device.DataSource = null;
            cb_device.DataSource = SerialPortManager.GetSerialPortManager().AvailablePorts;
            cb_device.DisplayMember = "DisplayString";
        }

        private void lbl_BrowseProcess_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    try
                    {
                        tb_processpath.Text = filePath;
                    }
                    catch
                    {
                        MessageBox.Show("Error loading icon",
                            "There was an issue loading this icon. Check if the filetype is correct.",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void lbl_InstertPlaceholder_Click(object sender, EventArgs e)
        {
            tb_arguments.Paste(RecipeBase.serialPlaceholderString);
        }

        private void cb_device_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshCb_Device();
        }

        private void refreshCb_Device()
        {
            SerialPortInst inst = cb_device.SelectedItem as SerialPortInst;
            if (inst != null)
            {
                tb_port.Text = inst.Port;
                if (inst is UsbSerialPortInst usb)
                {
                    tb_pid.Text = usb.PID;
                    tb_vid.Text = usb.VID;
                    tb_instancePath.Text = usb.InstancePath;
                }
                else
                {
                    tb_pid.Text = "";
                    tb_vid.Text = "";
                    tb_instancePath.Text = "";
                }
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, Size newSize)
        {
            int width = newSize.Width;
            int height = newSize.Height;

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void RecipeCreator_Load(object sender, EventArgs e)
        {
            UpdatePortList();
            refreshCb_Device();
            pb_icon.Image = GenerateCircle(" ");
            //pb_icon.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void rb_RecipeType_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb)
            {
                if (rb.Checked)
                {
                    if (rb == rb_Universal)
                    {
                        tb_port.Enabled = false;
                        tb_vid.Enabled = false;
                        tb_pid.Enabled = false;
                        tb_instancePath.Enabled = false;
                        return;
                    }
                    if (rb == rb_portSpecific)
                    {
                        tb_port.Enabled = true;
                        tb_vid.Enabled = false;
                        tb_pid.Enabled = false;
                        tb_instancePath.Enabled = false;
                        return;
                    }
                    if (rb == rb_vid)
                    {
                        tb_port.Enabled = false;
                        tb_vid.Enabled = true;
                        tb_pid.Enabled = false;
                        tb_instancePath.Enabled = false;
                        return;
                    }
                    if (rb == rb_vidpid)
                    {
                        tb_port.Enabled = false;
                        tb_vid.Enabled = true;
                        tb_pid.Enabled = true;
                        tb_instancePath.Enabled = false;
                        return;
                    }
                    if (rb == rb_path)
                    {
                        tb_port.Enabled = false;
                        tb_vid.Enabled = false;
                        tb_pid.Enabled = false;
                        tb_instancePath.Enabled = true;
                        return;
                    }
                }
            }
        }
    }
}
