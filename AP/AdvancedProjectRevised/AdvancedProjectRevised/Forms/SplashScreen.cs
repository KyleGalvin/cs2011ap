using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AP
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void btn_Client_Click(object sender, EventArgs e)
        {
            btn_Multiplayer.Visible = true;
            btn_Singleplayer.Visible = true;
            btn_Client.Visible = false;
            btn_Server.Visible = false;
        }

        private void btn_Server_Click(object sender, EventArgs e)
        {
            ServerProgram server = new ServerProgram();
            this.Visible = false;
        }

        private void btn_Singleplayer_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            using (ClientProgram client = new ClientProgram(false))
            {
                client.Run(28.0);
            }
            Application.Exit();
        }

        private void btn_Multiplayer_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            using (ClientProgram client = new ClientProgram(true))
            {
                client.Run(28.0);
            }
            Application.Exit();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
