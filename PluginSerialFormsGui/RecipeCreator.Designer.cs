namespace PluginSerialFormsGui
{
    partial class RecipeCreator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.gb_RecipeType = new System.Windows.Forms.GroupBox();
            this.rb_Universal = new System.Windows.Forms.RadioButton();
            this.rb_path = new System.Windows.Forms.RadioButton();
            this.rb_vidpid = new System.Windows.Forms.RadioButton();
            this.rb_vid = new System.Windows.Forms.RadioButton();
            this.rb_portSpecific = new System.Windows.Forms.RadioButton();
            this.gb_Info = new System.Windows.Forms.GroupBox();
            this.lbl_changeIcon = new System.Windows.Forms.Label();
            this.pb_icon = new System.Windows.Forms.PictureBox();
            this.tb_Desc = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_Name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gb_DeviceFilter = new System.Windows.Forms.GroupBox();
            this.lbl_refreshports = new System.Windows.Forms.Label();
            this.tb_port = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_instancePath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cb_device = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_pid = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_vid = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.gb_Process = new System.Windows.Forms.GroupBox();
            this.lbl_InstertPlaceholder = new System.Windows.Forms.Label();
            this.tb_arguments = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lblBrowseProcess = new System.Windows.Forms.Label();
            this.tb_processpath = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cb_processpolicy = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cb_runtype = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_save = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.gb_RecipeType.SuspendLayout();
            this.gb_Info.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_icon)).BeginInit();
            this.gb_DeviceFilter.SuspendLayout();
            this.gb_Process.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.gb_RecipeType);
            this.flowLayoutPanel1.Controls.Add(this.gb_Info);
            this.flowLayoutPanel1.Controls.Add(this.gb_DeviceFilter);
            this.flowLayoutPanel1.Controls.Add(this.gb_Process);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(526, 681);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // gb_RecipeType
            // 
            this.gb_RecipeType.Controls.Add(this.rb_Universal);
            this.gb_RecipeType.Controls.Add(this.rb_path);
            this.gb_RecipeType.Controls.Add(this.rb_vidpid);
            this.gb_RecipeType.Controls.Add(this.rb_vid);
            this.gb_RecipeType.Controls.Add(this.rb_portSpecific);
            this.gb_RecipeType.Dock = System.Windows.Forms.DockStyle.Top;
            this.gb_RecipeType.Location = new System.Drawing.Point(3, 3);
            this.gb_RecipeType.Name = "gb_RecipeType";
            this.gb_RecipeType.Padding = new System.Windows.Forms.Padding(15);
            this.gb_RecipeType.Size = new System.Drawing.Size(517, 72);
            this.gb_RecipeType.TabIndex = 0;
            this.gb_RecipeType.TabStop = false;
            this.gb_RecipeType.Text = "Recipe Type";
            // 
            // rb_Universal
            // 
            this.rb_Universal.AutoSize = true;
            this.rb_Universal.Checked = true;
            this.rb_Universal.Location = new System.Drawing.Point(14, 31);
            this.rb_Universal.Margin = new System.Windows.Forms.Padding(5);
            this.rb_Universal.Name = "rb_Universal";
            this.rb_Universal.Size = new System.Drawing.Size(69, 17);
            this.rb_Universal.TabIndex = 1;
            this.rb_Universal.TabStop = true;
            this.rb_Universal.Text = "Universal";
            this.rb_Universal.UseVisualStyleBackColor = true;
            this.rb_Universal.CheckedChanged += new System.EventHandler(this.rb_RecipeType_CheckedChanged);
            // 
            // rb_path
            // 
            this.rb_path.AutoSize = true;
            this.rb_path.Location = new System.Drawing.Point(353, 31);
            this.rb_path.Margin = new System.Windows.Forms.Padding(5);
            this.rb_path.Name = "rb_path";
            this.rb_path.Size = new System.Drawing.Size(84, 17);
            this.rb_path.TabIndex = 0;
            this.rb_path.Text = "Device Path";
            this.rb_path.UseVisualStyleBackColor = true;
            this.rb_path.CheckedChanged += new System.EventHandler(this.rb_RecipeType_CheckedChanged);
            // 
            // rb_vidpid
            // 
            this.rb_vidpid.AutoSize = true;
            this.rb_vidpid.Location = new System.Drawing.Point(245, 31);
            this.rb_vidpid.Margin = new System.Windows.Forms.Padding(5);
            this.rb_vidpid.Name = "rb_vidpid";
            this.rb_vidpid.Size = new System.Drawing.Size(98, 17);
            this.rb_vidpid.TabIndex = 0;
            this.rb_vidpid.Text = "USB VID && PID";
            this.rb_vidpid.UseVisualStyleBackColor = true;
            this.rb_vidpid.CheckedChanged += new System.EventHandler(this.rb_RecipeType_CheckedChanged);
            // 
            // rb_vid
            // 
            this.rb_vid.AutoSize = true;
            this.rb_vid.Location = new System.Drawing.Point(167, 31);
            this.rb_vid.Margin = new System.Windows.Forms.Padding(5);
            this.rb_vid.Name = "rb_vid";
            this.rb_vid.Size = new System.Drawing.Size(68, 17);
            this.rb_vid.TabIndex = 0;
            this.rb_vid.Text = "USB VID";
            this.rb_vid.UseVisualStyleBackColor = true;
            this.rb_vid.CheckedChanged += new System.EventHandler(this.rb_RecipeType_CheckedChanged);
            // 
            // rb_portSpecific
            // 
            this.rb_portSpecific.AutoSize = true;
            this.rb_portSpecific.Location = new System.Drawing.Point(93, 31);
            this.rb_portSpecific.Margin = new System.Windows.Forms.Padding(5);
            this.rb_portSpecific.Name = "rb_portSpecific";
            this.rb_portSpecific.Size = new System.Drawing.Size(64, 17);
            this.rb_portSpecific.TabIndex = 0;
            this.rb_portSpecific.Text = "Comport";
            this.rb_portSpecific.UseVisualStyleBackColor = true;
            this.rb_portSpecific.CheckedChanged += new System.EventHandler(this.rb_RecipeType_CheckedChanged);
            // 
            // gb_Info
            // 
            this.gb_Info.Controls.Add(this.lbl_changeIcon);
            this.gb_Info.Controls.Add(this.pb_icon);
            this.gb_Info.Controls.Add(this.tb_Desc);
            this.gb_Info.Controls.Add(this.label2);
            this.gb_Info.Controls.Add(this.tb_Name);
            this.gb_Info.Controls.Add(this.label1);
            this.gb_Info.Dock = System.Windows.Forms.DockStyle.Top;
            this.gb_Info.Location = new System.Drawing.Point(3, 81);
            this.gb_Info.Name = "gb_Info";
            this.gb_Info.Padding = new System.Windows.Forms.Padding(10);
            this.gb_Info.Size = new System.Drawing.Size(517, 206);
            this.gb_Info.TabIndex = 1;
            this.gb_Info.TabStop = false;
            this.gb_Info.Text = "Recipe Info";
            // 
            // lbl_changeIcon
            // 
            this.lbl_changeIcon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_changeIcon.Location = new System.Drawing.Point(342, 150);
            this.lbl_changeIcon.Name = "lbl_changeIcon";
            this.lbl_changeIcon.Size = new System.Drawing.Size(50, 13);
            this.lbl_changeIcon.TabIndex = 6;
            this.lbl_changeIcon.Text = "Set Icon";
            this.lbl_changeIcon.Click += new System.EventHandler(this.lbl_changeIcon_Click);
            // 
            // pb_icon
            // 
            this.pb_icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pb_icon.Location = new System.Drawing.Point(342, 97);
            this.pb_icon.Name = "pb_icon";
            this.pb_icon.Size = new System.Drawing.Size(50, 50);
            this.pb_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_icon.TabIndex = 5;
            this.pb_icon.TabStop = false;
            // 
            // tb_Desc
            // 
            this.tb_Desc.Location = new System.Drawing.Point(13, 88);
            this.tb_Desc.Multiline = true;
            this.tb_Desc.Name = "tb_Desc";
            this.tb_Desc.Size = new System.Drawing.Size(299, 93);
            this.tb_Desc.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description:";
            // 
            // tb_Name
            // 
            this.tb_Name.Location = new System.Drawing.Point(13, 39);
            this.tb_Name.Name = "tb_Name";
            this.tb_Name.Size = new System.Drawing.Size(392, 20);
            this.tb_Name.TabIndex = 1;
            this.tb_Name.TextChanged += new System.EventHandler(this.tb_Name_TextChanged);
            this.tb_Name.Leave += new System.EventHandler(this.tb_Name_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // gb_DeviceFilter
            // 
            this.gb_DeviceFilter.Controls.Add(this.lbl_refreshports);
            this.gb_DeviceFilter.Controls.Add(this.tb_port);
            this.gb_DeviceFilter.Controls.Add(this.label7);
            this.gb_DeviceFilter.Controls.Add(this.tb_instancePath);
            this.gb_DeviceFilter.Controls.Add(this.label6);
            this.gb_DeviceFilter.Controls.Add(this.cb_device);
            this.gb_DeviceFilter.Controls.Add(this.label5);
            this.gb_DeviceFilter.Controls.Add(this.tb_pid);
            this.gb_DeviceFilter.Controls.Add(this.label4);
            this.gb_DeviceFilter.Controls.Add(this.tb_vid);
            this.gb_DeviceFilter.Controls.Add(this.label3);
            this.gb_DeviceFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.gb_DeviceFilter.Location = new System.Drawing.Point(3, 293);
            this.gb_DeviceFilter.Name = "gb_DeviceFilter";
            this.gb_DeviceFilter.Padding = new System.Windows.Forms.Padding(10);
            this.gb_DeviceFilter.Size = new System.Drawing.Size(517, 145);
            this.gb_DeviceFilter.TabIndex = 2;
            this.gb_DeviceFilter.TabStop = false;
            this.gb_DeviceFilter.Text = "Device Filter";
            // 
            // lbl_refreshports
            // 
            this.lbl_refreshports.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_refreshports.Location = new System.Drawing.Point(445, 23);
            this.lbl_refreshports.Name = "lbl_refreshports";
            this.lbl_refreshports.Size = new System.Drawing.Size(50, 13);
            this.lbl_refreshports.TabIndex = 24;
            this.lbl_refreshports.Text = "Refresh";
            this.lbl_refreshports.Click += new System.EventHandler(this.lblRefreshports_Click);
            // 
            // tb_port
            // 
            this.tb_port.Enabled = false;
            this.tb_port.Location = new System.Drawing.Point(53, 60);
            this.tb_port.Name = "tb_port";
            this.tb_port.Size = new System.Drawing.Size(104, 20);
            this.tb_port.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(13, 63);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Port";
            // 
            // tb_instancePath
            // 
            this.tb_instancePath.Enabled = false;
            this.tb_instancePath.Location = new System.Drawing.Point(53, 102);
            this.tb_instancePath.Name = "tb_instancePath";
            this.tb_instancePath.Size = new System.Drawing.Size(384, 20);
            this.tb_instancePath.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(13, 105);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Path";
            // 
            // cb_device
            // 
            this.cb_device.FormattingEnabled = true;
            this.cb_device.Location = new System.Drawing.Point(166, 20);
            this.cb_device.Name = "cb_device";
            this.cb_device.Size = new System.Drawing.Size(273, 21);
            this.cb_device.TabIndex = 12;
            this.cb_device.SelectedIndexChanged += new System.EventHandler(this.cb_device_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "From existing port:";
            // 
            // tb_pid
            // 
            this.tb_pid.Enabled = false;
            this.tb_pid.Location = new System.Drawing.Point(333, 60);
            this.tb_pid.Name = "tb_pid";
            this.tb_pid.Size = new System.Drawing.Size(104, 20);
            this.tb_pid.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(304, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "PID";
            // 
            // tb_vid
            // 
            this.tb_vid.Enabled = false;
            this.tb_vid.Location = new System.Drawing.Point(194, 60);
            this.tb_vid.Name = "tb_vid";
            this.tb_vid.Size = new System.Drawing.Size(104, 20);
            this.tb_vid.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(163, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "VID";
            // 
            // gb_Process
            // 
            this.gb_Process.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_Process.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gb_Process.Controls.Add(this.lbl_InstertPlaceholder);
            this.gb_Process.Controls.Add(this.tb_arguments);
            this.gb_Process.Controls.Add(this.label12);
            this.gb_Process.Controls.Add(this.lblBrowseProcess);
            this.gb_Process.Controls.Add(this.tb_processpath);
            this.gb_Process.Controls.Add(this.label10);
            this.gb_Process.Controls.Add(this.cb_processpolicy);
            this.gb_Process.Controls.Add(this.label9);
            this.gb_Process.Controls.Add(this.cb_runtype);
            this.gb_Process.Controls.Add(this.label8);
            this.gb_Process.Location = new System.Drawing.Point(3, 444);
            this.gb_Process.Name = "gb_Process";
            this.gb_Process.Padding = new System.Windows.Forms.Padding(10);
            this.gb_Process.Size = new System.Drawing.Size(517, 145);
            this.gb_Process.TabIndex = 3;
            this.gb_Process.TabStop = false;
            this.gb_Process.Text = "Process";
            // 
            // lbl_InstertPlaceholder
            // 
            this.lbl_InstertPlaceholder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_InstertPlaceholder.Location = new System.Drawing.Point(443, 105);
            this.lbl_InstertPlaceholder.Name = "lbl_InstertPlaceholder";
            this.lbl_InstertPlaceholder.Size = new System.Drawing.Size(72, 16);
            this.lbl_InstertPlaceholder.TabIndex = 23;
            this.lbl_InstertPlaceholder.Text = "Placeholder";
            this.lbl_InstertPlaceholder.Click += new System.EventHandler(this.lbl_InstertPlaceholder_Click);
            // 
            // tb_arguments
            // 
            this.tb_arguments.Location = new System.Drawing.Point(128, 102);
            this.tb_arguments.Name = "tb_arguments";
            this.tb_arguments.Size = new System.Drawing.Size(311, 20);
            this.tb_arguments.TabIndex = 22;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(14, 105);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(97, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Process arguments";
            // 
            // lblBrowseProcess
            // 
            this.lblBrowseProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBrowseProcess.Location = new System.Drawing.Point(443, 79);
            this.lblBrowseProcess.Name = "lblBrowseProcess";
            this.lblBrowseProcess.Size = new System.Drawing.Size(50, 13);
            this.lblBrowseProcess.TabIndex = 7;
            this.lblBrowseProcess.Text = "Browse";
            this.lblBrowseProcess.Click += new System.EventHandler(this.lbl_BrowseProcess_Click);
            // 
            // tb_processpath
            // 
            this.tb_processpath.Location = new System.Drawing.Point(128, 76);
            this.tb_processpath.Name = "tb_processpath";
            this.tb_processpath.Size = new System.Drawing.Size(309, 20);
            this.tb_processpath.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(14, 79);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(69, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "Process path";
            // 
            // cb_processpolicy
            // 
            this.cb_processpolicy.FormattingEnabled = true;
            this.cb_processpolicy.Location = new System.Drawing.Point(167, 49);
            this.cb_processpolicy.Name = "cb_processpolicy";
            this.cb_processpolicy.Size = new System.Drawing.Size(270, 21);
            this.cb_processpolicy.TabIndex = 20;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Process policy";
            // 
            // cb_runtype
            // 
            this.cb_runtype.FormattingEnabled = true;
            this.cb_runtype.Location = new System.Drawing.Point(167, 22);
            this.cb_runtype.Name = "cb_runtype";
            this.cb_runtype.Size = new System.Drawing.Size(270, 21);
            this.cb_runtype.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Run type";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_save);
            this.panel1.Location = new System.Drawing.Point(3, 595);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(517, 71);
            this.panel1.TabIndex = 4;
            // 
            // btn_save
            // 
            this.btn_save.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_save.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save.Location = new System.Drawing.Point(0, 0);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(517, 71);
            this.btn_save.TabIndex = 0;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // RecipeCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 681);
            this.Controls.Add(this.flowLayoutPanel1);
            this.MaximizeBox = false;
            this.Name = "RecipeCreator";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Recipe Creator";
            this.Load += new System.EventHandler(this.RecipeCreator_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.gb_RecipeType.ResumeLayout(false);
            this.gb_RecipeType.PerformLayout();
            this.gb_Info.ResumeLayout(false);
            this.gb_Info.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_icon)).EndInit();
            this.gb_DeviceFilter.ResumeLayout(false);
            this.gb_DeviceFilter.PerformLayout();
            this.gb_Process.ResumeLayout(false);
            this.gb_Process.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox gb_RecipeType;
        private System.Windows.Forms.RadioButton rb_Universal;
        private System.Windows.Forms.RadioButton rb_path;
        private System.Windows.Forms.RadioButton rb_vidpid;
        private System.Windows.Forms.RadioButton rb_vid;
        private System.Windows.Forms.RadioButton rb_portSpecific;
        private System.Windows.Forms.GroupBox gb_Info;
        private System.Windows.Forms.TextBox tb_Desc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_Name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_changeIcon;
        private System.Windows.Forms.PictureBox pb_icon;
        private System.Windows.Forms.GroupBox gb_DeviceFilter;
        private System.Windows.Forms.TextBox tb_instancePath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cb_device;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_pid;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_vid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_port;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gb_Process;
        private System.Windows.Forms.Label lbl_InstertPlaceholder;
        private System.Windows.Forms.TextBox tb_arguments;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblBrowseProcess;
        private System.Windows.Forms.TextBox tb_processpath;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cb_processpolicy;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cb_runtype;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Label lbl_refreshports;
    }
}