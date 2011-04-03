namespace AP
{
    partial class SplashScreen
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
            this.btn_Server = new System.Windows.Forms.Button();
            this.btn_Client = new System.Windows.Forms.Button();
            this.btn_Singleplayer = new System.Windows.Forms.Button();
            this.btn_Multiplayer = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Server
            // 
            this.btn_Server.Image = global::AdvancedProjectRevised.Properties.Resources.ServerButton;
            this.btn_Server.Location = new System.Drawing.Point(469, 481);
            this.btn_Server.Name = "btn_Server";
            this.btn_Server.Size = new System.Drawing.Size(128, 32);
            this.btn_Server.TabIndex = 3;
            this.btn_Server.UseVisualStyleBackColor = true;
            this.btn_Server.Click += new System.EventHandler(this.btn_Server_Click);
            // 
            // btn_Client
            // 
            this.btn_Client.Image = global::AdvancedProjectRevised.Properties.Resources.Clientbutton;
            this.btn_Client.Location = new System.Drawing.Point(469, 558);
            this.btn_Client.Name = "btn_Client";
            this.btn_Client.Size = new System.Drawing.Size(128, 32);
            this.btn_Client.TabIndex = 2;
            this.btn_Client.UseVisualStyleBackColor = true;
            this.btn_Client.Click += new System.EventHandler(this.btn_Client_Click);
            // 
            // btn_Singleplayer
            // 
            this.btn_Singleplayer.Image = global::AdvancedProjectRevised.Properties.Resources.SingleButton;
            this.btn_Singleplayer.Location = new System.Drawing.Point(381, 519);
            this.btn_Singleplayer.Name = "btn_Singleplayer";
            this.btn_Singleplayer.Size = new System.Drawing.Size(128, 32);
            this.btn_Singleplayer.TabIndex = 1;
            this.btn_Singleplayer.UseVisualStyleBackColor = true;
            this.btn_Singleplayer.Visible = false;
            this.btn_Singleplayer.Click += new System.EventHandler(this.btn_Singleplayer_Click);
            // 
            // btn_Multiplayer
            // 
            this.btn_Multiplayer.BackColor = System.Drawing.Color.Transparent;
            this.btn_Multiplayer.Image = global::AdvancedProjectRevised.Properties.Resources.MultiButton;
            this.btn_Multiplayer.Location = new System.Drawing.Point(553, 519);
            this.btn_Multiplayer.Name = "btn_Multiplayer";
            this.btn_Multiplayer.Size = new System.Drawing.Size(128, 32);
            this.btn_Multiplayer.TabIndex = 0;
            this.btn_Multiplayer.UseVisualStyleBackColor = false;
            this.btn_Multiplayer.Visible = false;
            this.btn_Multiplayer.Click += new System.EventHandler(this.btn_Multiplayer_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::AdvancedProjectRevised.Properties.Resources.BG;
            this.pictureBox1.InitialImage = global::AdvancedProjectRevised.Properties.Resources.Splash_title;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 600);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 746);
            this.Controls.Add(this.btn_Server);
            this.Controls.Add(this.btn_Client);
            this.Controls.Add(this.btn_Singleplayer);
            this.Controls.Add(this.btn_Multiplayer);
            this.Controls.Add(this.pictureBox1);
            this.Name = "SplashScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Multiplayer;
        private System.Windows.Forms.Button btn_Singleplayer;
        private System.Windows.Forms.Button btn_Client;
        private System.Windows.Forms.Button btn_Server;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}