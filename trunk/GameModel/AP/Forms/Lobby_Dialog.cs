using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AP.Forms
{
    public partial class Lobby_Dialog : Form
    {
        public string Name;
        public Lobby_Dialog()
        {
            InitializeComponent();
            txt_Name.Select();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            validName();
        }

        private void validName()
        {
            if (txt_Name.Text.Length > 0)
            {
                this.DialogResult = DialogResult.OK;
                Name = txt_Name.Text;
                this.Close();
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void txt_Name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                validName();
            }
        }

    }
}
