namespace AP.Forms
{
    partial class Main
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
            this.btn_Quit = new System.Windows.Forms.Button();
            this.btn_Refresh = new System.Windows.Forms.Button();
            this.btn_Host = new System.Windows.Forms.Button();
            this.lst_Servers = new System.Windows.Forms.ListBox();
            this.btn_Join = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Quit
            // 
            this.btn_Quit.Location = new System.Drawing.Point(138, 311);
            this.btn_Quit.Name = "btn_Quit";
            this.btn_Quit.Size = new System.Drawing.Size(75, 23);
            this.btn_Quit.TabIndex = 0;
            this.btn_Quit.Text = "Quit";
            this.btn_Quit.UseVisualStyleBackColor = true;
            this.btn_Quit.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_Refresh
            // 
            this.btn_Refresh.Location = new System.Drawing.Point(93, 282);
            this.btn_Refresh.Name = "btn_Refresh";
            this.btn_Refresh.Size = new System.Drawing.Size(120, 23);
            this.btn_Refresh.TabIndex = 1;
            this.btn_Refresh.Text = "Refresh List";
            this.btn_Refresh.UseVisualStyleBackColor = true;
            this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
            // 
            // btn_Host
            // 
            this.btn_Host.Location = new System.Drawing.Point(12, 12);
            this.btn_Host.Name = "btn_Host";
            this.btn_Host.Size = new System.Drawing.Size(75, 23);
            this.btn_Host.TabIndex = 2;
            this.btn_Host.Text = "Host Game";
            this.btn_Host.UseVisualStyleBackColor = true;
            this.btn_Host.Click += new System.EventHandler(this.btn_Host_Click);
            // 
            // lst_Servers
            // 
            this.lst_Servers.FormattingEnabled = true;
            this.lst_Servers.Location = new System.Drawing.Point(93, 12);
            this.lst_Servers.Name = "lst_Servers";
            this.lst_Servers.Size = new System.Drawing.Size(120, 264);
            this.lst_Servers.TabIndex = 3;
            this.lst_Servers.DoubleClick += new System.EventHandler(this.lst_Servers_DoubleClick);
            // 
            // btn_Join
            // 
            this.btn_Join.Location = new System.Drawing.Point(12, 282);
            this.btn_Join.Name = "btn_Join";
            this.btn_Join.Size = new System.Drawing.Size(75, 23);
            this.btn_Join.TabIndex = 4;
            this.btn_Join.Text = "Join Game";
            this.btn_Join.UseVisualStyleBackColor = true;
            this.btn_Join.Click += new System.EventHandler(this.btn_Join_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 343);
            this.Controls.Add(this.btn_Join);
            this.Controls.Add(this.lst_Servers);
            this.Controls.Add(this.btn_Host);
            this.Controls.Add(this.btn_Refresh);
            this.Controls.Add(this.btn_Quit);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Main Lobby";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Quit;
        private System.Windows.Forms.Button btn_Refresh;
        private System.Windows.Forms.Button btn_Host;
        private System.Windows.Forms.ListBox lst_Servers;
        private System.Windows.Forms.Button btn_Join;
    }
}