namespace AP.Forms
{
    partial class Lobby
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
            this.btn_Start = new System.Windows.Forms.Button();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.grp_Players = new System.Windows.Forms.GroupBox();
            this.lst_Players = new System.Windows.Forms.ListBox();
            this.btn_Close = new System.Windows.Forms.Button();
            this.grp_Players.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Start
            // 
            this.btn_Start.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Start.Location = new System.Drawing.Point(156, 341);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(75, 23);
            this.btn_Start.TabIndex = 0;
            this.btn_Start.Text = "Start Game";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // lbl_Name
            // 
            this.lbl_Name.AutoSize = true;
            this.lbl_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Name.Location = new System.Drawing.Point(13, 13);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(51, 20);
            this.lbl_Name.TabIndex = 1;
            this.lbl_Name.Text = "label1";
            // 
            // grp_Players
            // 
            this.grp_Players.Controls.Add(this.lst_Players);
            this.grp_Players.Location = new System.Drawing.Point(17, 36);
            this.grp_Players.Name = "grp_Players";
            this.grp_Players.Size = new System.Drawing.Size(214, 299);
            this.grp_Players.TabIndex = 2;
            this.grp_Players.TabStop = false;
            this.grp_Players.Text = "Players Connected";
            // 
            // listBox1
            // 
            this.lst_Players.FormattingEnabled = true;
            this.lst_Players.Location = new System.Drawing.Point(6, 19);
            this.lst_Players.Name = "listBox1";
            this.lst_Players.Size = new System.Drawing.Size(202, 277);
            this.lst_Players.TabIndex = 0;
            this.lst_Players.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // btn_Close
            // 
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Close.Location = new System.Drawing.Point(17, 341);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(75, 23);
            this.btn_Close.TabIndex = 3;
            this.btn_Close.Text = "Close Lobby";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // Lobby
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 376);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.grp_Players);
            this.Controls.Add(this.lbl_Name);
            this.Controls.Add(this.btn_Start);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Lobby";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Lobby";
            this.grp_Players.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.GroupBox grp_Players;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.ListBox lst_Players;
    }
}