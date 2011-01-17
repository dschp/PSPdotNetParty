/*
Copyright (C) 2010,2011 monte

This file is part of PSP NetParty.

PSP NetParty is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArenaClient
{
    public partial class PasswordDialog : Form
    {
        public string Password;

        public PasswordDialog()
        {
            InitializeComponent();
        }

        void PasswordEntered()
        {
            this.Password = PasswordTextBox.Text;
            if (Password != string.Empty)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            this.Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            PasswordEntered();
        }

        private void PasswordTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                PasswordEntered();
            }
            else if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
        }

        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            PasswordTextBox.Text = PasswordTextBox.Text.Replace(" ", "");
        }
    }
}
