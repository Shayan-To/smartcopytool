// Copyright 2011 Simon Booth
// 
// Released under GNU Public License.
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmartCopyTool
{
    internal partial class FormProgressDialog : Form
    {
        private Worker theWorker;

        public FormProgressDialog( Worker worker = null )
        {
            InitializeComponent();
            theWorker = worker;
        }

        private void cancelButton_Click( object sender, EventArgs e )
        {
            // Suppose it should be 'Abort' really... Cancel implies we never started.
            this.DialogResult = DialogResult.Cancel;
            theWorker.RequestResume();
            Close();
        }

        private void pauseButton_Click( object sender, EventArgs e )
        {
            if ( theWorker != null )
            {
                if ( pauseButton.Text == "Pause" )
                {
                    pauseButton.Text = "Resume";
                    theWorker.RequestPause();
                }
                else
                {
                    pauseButton.Text = "Pause";
                    theWorker.RequestResume();
                }                
            }

        }

        private void FormProgressDialog_Load( object sender, EventArgs e )
        {
            // No, we don't want to return CANCEL by default, you tool
            // OK, setting the result makes it think we want the dialog to close... sigh
            //this.DialogResult = DialogResult.OK;
        }
    }
}
