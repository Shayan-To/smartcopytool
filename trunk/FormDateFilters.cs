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
    public partial class FormDateFilters : Form
    {
        public DateTime? FilterIfOlder { get; set; }

        public DateTime? FilterIfNewer { get; set; }

        public FormDateFilters(DateTime? dtFilterIfOlder, DateTime? dtFilterIfNewer)
        {
            FilterIfOlder = dtFilterIfOlder;
            FilterIfNewer = dtFilterIfNewer;
            InitializeComponent();

            if (FilterIfOlder.HasValue)
            {
                dateTimePickerOlder.Value = FilterIfOlder.Value;
                dateTimePickerOlder.Enabled = true;
                cbFilterIfOlder.Checked = true;
            }

            if (FilterIfNewer.HasValue)
            {
                dateTimePickerNewer.Value = FilterIfNewer.Value;
                dateTimePickerNewer.Enabled = true;
                cbFilterIfNewer.Checked = true;
            }
        }

        private void buttonSetDateFilters_Click(object sender, EventArgs e)
        {
            if (dateTimePickerOlder.Enabled)
            {
                FilterIfOlder = dateTimePickerOlder.Value;
            }
            else
            {
                FilterIfOlder = null;
            }

            if (dateTimePickerNewer.Enabled)
            {
                FilterIfNewer = dateTimePickerNewer.Value;
            }
            else
            {
                FilterIfNewer = null;
            }

            Console.WriteLine("Filtering files not between dates {0} and {1}", 
                FilterIfOlder != null ? FilterIfOlder.ToString() : "any", 
                FilterIfNewer != null ? FilterIfNewer.ToString() : "any");
        }

        private void cbFilterIfOlder_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerOlder.Enabled = cbFilterIfOlder.Checked;
        }

        private void cbFilterIfNewer_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerNewer.Enabled = cbFilterIfNewer.Checked;
        }
    }
}
