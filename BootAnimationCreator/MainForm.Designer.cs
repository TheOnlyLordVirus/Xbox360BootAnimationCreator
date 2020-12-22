namespace BootAnimationCreator
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.carbonTheme = new CS_ClassLibraryTester.CarbonFiberTheme();
            this.crcTextBox = new CS_ClassLibraryTester.CarbonFiberTextBox();
            this.crcButton = new CS_ClassLibraryTester.CarbonFiberButton();
            this.crcLabel = new CS_ClassLibraryTester.CarbonFiberLabel();
            this.minimizeButton = new CS_ClassLibraryTester.CarbonFiberControlButton();
            this.videoFileSelectorLabel = new CS_ClassLibraryTester.CarbonFiberLabel();
            this.loadVideoFileButton = new CS_ClassLibraryTester.CarbonFiberButton();
            this.backgroundPicture = new System.Windows.Forms.PictureBox();
            this.closeButton = new CS_ClassLibraryTester.CarbonFiberControlButton();
            this.carbonTheme.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // openFile
            // 
            this.openFile.FileName = "openFile";
            // 
            // carbonTheme
            // 
            this.carbonTheme.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.carbonTheme.BorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.carbonTheme.Colors = new CS_ClassLibraryTester.Bloom[0];
            this.carbonTheme.Controls.Add(this.crcTextBox);
            this.carbonTheme.Controls.Add(this.crcButton);
            this.carbonTheme.Controls.Add(this.crcLabel);
            this.carbonTheme.Controls.Add(this.minimizeButton);
            this.carbonTheme.Controls.Add(this.videoFileSelectorLabel);
            this.carbonTheme.Controls.Add(this.loadVideoFileButton);
            this.carbonTheme.Controls.Add(this.backgroundPicture);
            this.carbonTheme.Controls.Add(this.closeButton);
            this.carbonTheme.Customization = "";
            this.carbonTheme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.carbonTheme.Font = new System.Drawing.Font("Verdana", 8F);
            this.carbonTheme.Icon = null;
            this.carbonTheme.Image = null;
            this.carbonTheme.Location = new System.Drawing.Point(0, 0);
            this.carbonTheme.Movable = true;
            this.carbonTheme.Name = "carbonTheme";
            this.carbonTheme.NoRounding = false;
            this.carbonTheme.ShowIcon = false;
            this.carbonTheme.Sizable = false;
            this.carbonTheme.Size = new System.Drawing.Size(500, 300);
            this.carbonTheme.SmartBounds = true;
            this.carbonTheme.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation;
            this.carbonTheme.TabIndex = 1;
            this.carbonTheme.Text = "Xbox 360 bootanim.xex creator";
            this.carbonTheme.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.carbonTheme.Transparent = false;
            // 
            // crcTextBox
            // 
            this.crcTextBox.Colors = new CS_ClassLibraryTester.Bloom[0];
            this.crcTextBox.Customization = "";
            this.crcTextBox.Font = new System.Drawing.Font("Verdana", 8F);
            this.crcTextBox.Image = null;
            this.crcTextBox.Location = new System.Drawing.Point(61, 195);
            this.crcTextBox.MaxLength = 32767;
            this.crcTextBox.Multiline = false;
            this.crcTextBox.Name = "crcTextBox";
            this.crcTextBox.NoRounding = false;
            this.crcTextBox.ReadOnly = true;
            this.crcTextBox.Size = new System.Drawing.Size(131, 24);
            this.crcTextBox.TabIndex = 14;
            this.crcTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.crcTextBox.Transparent = false;
            this.crcTextBox.UseSystemPasswordChar = false;
            // 
            // crcButton
            // 
            this.crcButton.Colors = new CS_ClassLibraryTester.Bloom[0];
            this.crcButton.Customization = "";
            this.crcButton.Font = new System.Drawing.Font("Verdana", 5.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.crcButton.Image = null;
            this.crcButton.Location = new System.Drawing.Point(198, 195);
            this.crcButton.Name = "crcButton";
            this.crcButton.NoRounding = false;
            this.crcButton.Size = new System.Drawing.Size(58, 24);
            this.crcButton.TabIndex = 13;
            this.crcButton.Text = "Re-Calculate";
            this.crcButton.Transparent = false;
            this.crcButton.Click += new System.EventHandler(this.crcButton_Click);
            // 
            // crcLabel
            // 
            this.crcLabel.BackColor = System.Drawing.Color.Transparent;
            this.crcLabel.Colors = new CS_ClassLibraryTester.Bloom[0];
            this.crcLabel.Customization = "";
            this.crcLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.crcLabel.Image = null;
            this.crcLabel.Location = new System.Drawing.Point(61, 168);
            this.crcLabel.Name = "crcLabel";
            this.crcLabel.NoRounding = false;
            this.crcLabel.Size = new System.Drawing.Size(57, 14);
            this.crcLabel.TabIndex = 12;
            this.crcLabel.Text = "CRC-32: ";
            this.crcLabel.Transparent = true;
            // 
            // minimizeButton
            // 
            this.minimizeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimizeButton.Colors = new CS_ClassLibraryTester.Bloom[0];
            this.minimizeButton.Customization = "";
            this.minimizeButton.Font = new System.Drawing.Font("Verdana", 8F);
            this.minimizeButton.Image = null;
            this.minimizeButton.Location = new System.Drawing.Point(431, 7);
            this.minimizeButton.Name = "minimizeButton";
            this.minimizeButton.NoRounding = false;
            this.minimizeButton.Size = new System.Drawing.Size(26, 20);
            this.minimizeButton.StateClose = false;
            this.minimizeButton.StateMaximize = false;
            this.minimizeButton.StateMinimize = true;
            this.minimizeButton.TabIndex = 11;
            this.minimizeButton.Transparent = false;
            // 
            // videoFileSelectorLabel
            // 
            this.videoFileSelectorLabel.BackColor = System.Drawing.Color.Transparent;
            this.videoFileSelectorLabel.Colors = new CS_ClassLibraryTester.Bloom[0];
            this.videoFileSelectorLabel.Customization = "";
            this.videoFileSelectorLabel.Font = new System.Drawing.Font("Verdana", 8F);
            this.videoFileSelectorLabel.Image = null;
            this.videoFileSelectorLabel.Location = new System.Drawing.Point(61, 82);
            this.videoFileSelectorLabel.Name = "videoFileSelectorLabel";
            this.videoFileSelectorLabel.NoRounding = false;
            this.videoFileSelectorLabel.Size = new System.Drawing.Size(195, 14);
            this.videoFileSelectorLabel.TabIndex = 9;
            this.videoFileSelectorLabel.Text = "Please select your boot animation";
            this.videoFileSelectorLabel.Transparent = true;
            // 
            // loadVideoFileButton
            // 
            this.loadVideoFileButton.Colors = new CS_ClassLibraryTester.Bloom[0];
            this.loadVideoFileButton.Customization = "";
            this.loadVideoFileButton.Font = new System.Drawing.Font("Verdana", 8F);
            this.loadVideoFileButton.Image = null;
            this.loadVideoFileButton.Location = new System.Drawing.Point(61, 111);
            this.loadVideoFileButton.Name = "loadVideoFileButton";
            this.loadVideoFileButton.NoRounding = false;
            this.loadVideoFileButton.Size = new System.Drawing.Size(195, 29);
            this.loadVideoFileButton.TabIndex = 8;
            this.loadVideoFileButton.Text = "Load Video File";
            this.loadVideoFileButton.Transparent = false;
            this.loadVideoFileButton.Click += new System.EventHandler(this.loadVideoFileButton_Click);
            // 
            // backgroundPicture
            // 
            this.backgroundPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.backgroundPicture.Image = global::BootAnimationCreator.Properties.Resources.xboxTheme;
            this.backgroundPicture.InitialImage = null;
            this.backgroundPicture.Location = new System.Drawing.Point(12, 33);
            this.backgroundPicture.Name = "backgroundPicture";
            this.backgroundPicture.Size = new System.Drawing.Size(479, 255);
            this.backgroundPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.backgroundPicture.TabIndex = 7;
            this.backgroundPicture.TabStop = false;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Colors = new CS_ClassLibraryTester.Bloom[0];
            this.closeButton.Customization = "";
            this.closeButton.Font = new System.Drawing.Font("Verdana", 8F);
            this.closeButton.Image = null;
            this.closeButton.Location = new System.Drawing.Point(463, 7);
            this.closeButton.Name = "closeButton";
            this.closeButton.NoRounding = false;
            this.closeButton.Size = new System.Drawing.Size(26, 20);
            this.closeButton.StateClose = true;
            this.closeButton.StateMaximize = false;
            this.closeButton.StateMinimize = false;
            this.closeButton.TabIndex = 5;
            this.closeButton.Transparent = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Controls.Add(this.carbonTheme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Xbox 360 bootanim.xex creator";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.carbonTheme.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.backgroundPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private CS_ClassLibraryTester.CarbonFiberTheme carbonTheme;
        private CS_ClassLibraryTester.CarbonFiberControlButton closeButton;
        private CS_ClassLibraryTester.CarbonFiberLabel videoFileSelectorLabel;
        private CS_ClassLibraryTester.CarbonFiberButton loadVideoFileButton;
        private System.Windows.Forms.PictureBox backgroundPicture;
        private CS_ClassLibraryTester.CarbonFiberControlButton minimizeButton;
        private CS_ClassLibraryTester.CarbonFiberLabel crcLabel;
        private CS_ClassLibraryTester.CarbonFiberButton crcButton;
        private CS_ClassLibraryTester.CarbonFiberTextBox crcTextBox;
    }
}

