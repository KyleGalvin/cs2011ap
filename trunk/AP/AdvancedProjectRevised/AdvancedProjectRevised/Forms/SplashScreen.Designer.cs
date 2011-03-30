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
            this.btn_Multiplayer = new System.Windows.Forms.Button();
            this.btn_Singleplayer = new System.Windows.Forms.Button();
            this.btn_Client = new System.Windows.Forms.Button();
            this.btn_Server = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Multiplayer
            // 
            this.btn_Multiplayer.Location = new System.Drawing.Point(301, 121);
            this.btn_Multiplayer.Name = "btn_Multiplayer";
            this.btn_Multiplayer.Size = new System.Drawing.Size(75, 23);
            this.btn_Multiplayer.TabIndex = 0;
            this.btn_Multiplayer.Text = "Multiplayer";
            this.btn_Multiplayer.UseVisualStyleBackColor = true;
            this.btn_Multiplayer.Visible = false;
            this.btn_Multiplayer.Click += new System.EventHandler(this.btn_Multiplayer_Click);
            // 
            // btn_Singleplayer
            // 
            this.btn_Singleplayer.Location = new System.Drawing.Point(129, 121);
            this.btn_Singleplayer.Name = "btn_Singleplayer";
            this.btn_Singleplayer.Size = new System.Drawing.Size(75, 23);
            this.btn_Singleplayer.TabIndex = 1;
            this.btn_Singleplayer.Text = "Singleplayer";
            this.btn_Singleplayer.UseVisualStyleBackColor = true;
            this.btn_Singleplayer.Visible = false;
            this.btn_Singleplayer.Click += new System.EventHandler(this.btn_Singleplayer_Click);
            // 
            // btn_Client
            // 
            this.btn_Client.Location = new System.Drawing.Point(217, 160);
            this.btn_Client.Name = "btn_Client";
            this.btn_Client.Size = new System.Drawing.Size(75, 23);
            this.btn_Client.TabIndex = 2;
            this.btn_Client.Text = "Client";
            this.btn_Client.UseVisualStyleBackColor = true;
            this.btn_Client.Click += new System.EventHandler(this.btn_Client_Click);
            // 
            // btn_Server
            // 
            this.btn_Server.Location = new System.Drawing.Point(217, 83);
            this.btn_Server.Name = "btn_Server";
            this.btn_Server.Size = new System.Drawing.Size(75, 23);
            this.btn_Server.TabIndex = 3;
            this.btn_Server.Text = "Server";
            this.btn_Server.UseVisualStyleBackColor = true;
            this.btn_Server.Click += new System.EventHandler(this.btn_Server_Click);
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 339);
            this.Controls.Add(this.btn_Server);
            this.Controls.Add(this.btn_Client);
            this.Controls.Add(this.btn_Singleplayer);
            this.Controls.Add(this.btn_Multiplayer);
            this.Name = "SplashScreen";
            this.Text = "SplashScreen";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Multiplayer;
        private System.Windows.Forms.Button btn_Singleplayer;
        private System.Windows.Forms.Button btn_Client;
        private System.Windows.Forms.Button btn_Server;
    }
}