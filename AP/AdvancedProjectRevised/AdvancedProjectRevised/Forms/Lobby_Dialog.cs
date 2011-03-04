using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AP.Forms
{
    /// <summary>
    /// The user will see this form when they click the host button. It is used to obtain the server name.
    /// </summary>
    public partial class Lobby_Dialog : Form
    {
		#region Fields (1) 

        public string Name;

		#endregion Fields 

		#region Constructors (1) 

        public Lobby_Dialog()
        {
            InitializeComponent();
            txt_Name.Select();
        }

		#endregion Constructors 

		#region Methods (4) 

		// Private Methods (4) 

        /// <summary>
        /// Handles the Click event of the btn_Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        /// <summary>
        /// Handles the Click event of the btn_Ok control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btn_Ok_Click(object sender, EventArgs e)
        {
            validName();
        }

        /// <summary>
        /// Handles the KeyPress event of the txt_Name control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
        private void txt_Name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                validName();
            }
        }

        /// <summary>
        /// Checks if the name is valid
        /// </summary>
        private void validName()
        {
            if (txt_Name.Text.Length > 0)
            {
                this.DialogResult = DialogResult.OK;
                Name = txt_Name.Text;
                this.Close();
            }
        }

		#endregion Methods 
    }
}
