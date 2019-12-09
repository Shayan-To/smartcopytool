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
    public partial class FormSetFilters : Form
    {
        public string filterString = "";

        public FormSetFilters()
        {
            InitializeComponent();
            filtersTextbox.Text = filterString;

        }

        private void FormSetFilters_Shown(object sender, EventArgs e)
        {
            filtersTextbox.Text = filterString;
            filtersTextbox.SelectAll();
        }

        private void buttonOK_Click( object sender, EventArgs e )
        {
            filterString = filtersTextbox.Text;
        }

    }
}
