namespace Kendax
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.ConnectedLbl = new System.Windows.Forms.Label();
            this.AllAccountsChckbx = new System.Windows.Forms.CheckBox();
            this.AvatarPctbx = new System.Windows.Forms.PictureBox();
            this.AccountTxt = new System.Windows.Forms.ComboBox();
            this.LoginBtn = new System.Windows.Forms.Button();
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.StatusTxt = new System.Windows.Forms.Label();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.Title = new System.Windows.Forms.Timer(this.components);
            this.Title3 = new System.Windows.Forms.Timer(this.components);
            this.Title4 = new System.Windows.Forms.Timer(this.components);
            this.password_txt = new System.Windows.Forms.TextBox();
            this.username_txt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Login = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Updates = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.AvatarPctbx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.Login.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.Updates.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectedLbl
            // 
            this.ConnectedLbl.AutoSize = true;
            this.ConnectedLbl.ForeColor = System.Drawing.Color.DarkRed;
            this.ConnectedLbl.Location = new System.Drawing.Point(167, 0);
            this.ConnectedLbl.Name = "ConnectedLbl";
            this.ConnectedLbl.Size = new System.Drawing.Size(32, 13);
            this.ConnectedLbl.TabIndex = 0;
            // 
            // AllAccountsChckbx
            // 
            this.AllAccountsChckbx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AllAccountsChckbx.AutoSize = true;
            this.AllAccountsChckbx.Enabled = false;
            this.AllAccountsChckbx.Location = new System.Drawing.Point(487, -1);
            this.AllAccountsChckbx.Name = "AllAccountsChckbx";
            this.AllAccountsChckbx.Size = new System.Drawing.Size(85, 17);
            this.AllAccountsChckbx.TabIndex = 1;
            this.AllAccountsChckbx.Text = "All Accounts";
            this.AllAccountsChckbx.UseVisualStyleBackColor = true;
            // 
            // AvatarPctbx
            // 
            this.AvatarPctbx.ErrorImage = null;
            this.AvatarPctbx.Image = global::Kendax.Properties.Resources.Avatar;
            this.AvatarPctbx.InitialImage = null;
            this.AvatarPctbx.Location = new System.Drawing.Point(6, 19);
            this.AvatarPctbx.Name = "AvatarPctbx";
            this.AvatarPctbx.Size = new System.Drawing.Size(64, 110);
            this.AvatarPctbx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.AvatarPctbx.TabIndex = 2;
            this.AvatarPctbx.TabStop = false;
            this.AvatarPctbx.Click += new System.EventHandler(this.AvatarPctbx_Click);
            // 
            // AccountTxt
            // 
            this.AccountTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AccountTxt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AccountTxt.Enabled = false;
            this.AccountTxt.FormattingEnabled = true;
            this.AccountTxt.Location = new System.Drawing.Point(76, 19);
            this.AccountTxt.Name = "AccountTxt";
            this.AccountTxt.Size = new System.Drawing.Size(496, 21);
            this.AccountTxt.TabIndex = 1;
            // 
            // LoginBtn
            // 
            this.LoginBtn.Enabled = false;
            this.LoginBtn.Location = new System.Drawing.Point(76, 46);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(146, 23);
            this.LoginBtn.TabIndex = 3;
            this.LoginBtn.Text = "Login/Authenticate";
            this.LoginBtn.UseVisualStyleBackColor = true;
            // 
            // ConnectBtn
            // 
            this.ConnectBtn.Enabled = false;
            this.ConnectBtn.Location = new System.Drawing.Point(228, 46);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(146, 23);
            this.ConnectBtn.TabIndex = 4;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = true;
            // 
            // StatusTxt
            // 
            this.StatusTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusTxt.Location = new System.Drawing.Point(76, 82);
            this.StatusTxt.Name = "StatusTxt";
            this.StatusTxt.Size = new System.Drawing.Size(359, 47);
            this.StatusTxt.TabIndex = 5;
            this.StatusTxt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.StatusTxt.Click += new System.EventHandler(this.StatusTxt_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(420, 147);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(75, 23);
            this.button11.TabIndex = 6;
            this.button11.Text = "Leave room";
            this.button11.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(501, 147);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(75, 23);
            this.button12.TabIndex = 7;
            this.button12.Text = "Panic";
            this.button12.UseVisualStyleBackColor = true;
            // 
            // Title
            // 
            this.Title.Interval = 1000;
            this.Title.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Title3
            // 
            this.Title3.Interval = 1800;
            this.Title3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // Title4
            // 
            this.Title4.Interval = 2100;
            this.Title4.Tick += new System.EventHandler(this.timer4_Tick);
            // 
            // password_txt
            // 
            this.password_txt.Location = new System.Drawing.Point(181, 111);
            this.password_txt.Name = "password_txt";
            this.password_txt.Size = new System.Drawing.Size(219, 20);
            this.password_txt.TabIndex = 0;
            // 
            // username_txt
            // 
            this.username_txt.Location = new System.Drawing.Point(181, 61);
            this.username_txt.Name = "username_txt";
            this.username_txt.Size = new System.Drawing.Size(219, 20);
            this.username_txt.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(176, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(178, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Password:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(181, 151);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(251, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Kendax silver";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-2, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(605, 116);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // Login
            // 
            this.Login.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.Login.Controls.Add(this.tabPage1);
            this.Login.Controls.Add(this.Updates);
            this.Login.Location = new System.Drawing.Point(-2, 108);
            this.Login.Name = "Login";
            this.Login.SelectedIndex = 0;
            this.Login.Size = new System.Drawing.Size(605, 229);
            this.Login.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.password_txt);
            this.tabPage1.Controls.Add(this.username_txt);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(597, 200);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Login";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Updates
            // 
            this.Updates.Controls.Add(this.webBrowser1);
            this.Updates.Location = new System.Drawing.Point(4, 4);
            this.Updates.Name = "Updates";
            this.Updates.Padding = new System.Windows.Forms.Padding(3);
            this.Updates.Size = new System.Drawing.Size(597, 203);
            this.Updates.TabIndex = 1;
            this.Updates.Text = "Updates";
            this.Updates.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(325, 151);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(3, 3);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScrollBarsEnabled = false;
            this.webBrowser1.Size = new System.Drawing.Size(591, 197);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.TabStop = false;
            this.webBrowser1.Url = new System.Uri("http://kendax.esy.es/news.php", System.UriKind.Absolute);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 334);
            this.Controls.Add(this.Login);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Kendax";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AvatarPctbx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.Login.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.Updates.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label ConnectedLbl;
        private System.Windows.Forms.CheckBox AllAccountsChckbx;
        private System.Windows.Forms.PictureBox AvatarPctbx;
        private System.Windows.Forms.ComboBox AccountTxt;
        private System.Windows.Forms.Button LoginBtn;
        private System.Windows.Forms.Button ConnectBtn;
        private System.Windows.Forms.Label StatusTxt;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Timer Title;
        private System.Windows.Forms.Timer Title3;
        private System.Windows.Forms.Timer Title4;
        private System.Windows.Forms.TextBox password_txt;
        private System.Windows.Forms.TextBox username_txt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl Login;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabPage Updates;
        private System.Windows.Forms.WebBrowser webBrowser1;

    }
}