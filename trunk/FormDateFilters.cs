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
        DateRemover.DateRanges DateRanges { get; set; }

        public DateTime? FilterIfOlder => DateRanges.FilterIfOlderThan;

        public DateTime? FilterIfNewer => DateRanges.FilterIfNewerThan;

        public DateTime? FilterIfModifiedBefore => DateRanges.FilterIfModifiedBefore;

        public DateTime? FilterIfModifiedSince => DateRanges.FilterIfModifiedSince;

        internal FormDateFilters(DateRemover.DateRanges dateRanges)
        {
            DateRanges = dateRanges;

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

            if (FilterIfModifiedBefore.HasValue)
            {
                dateTimePickerModifiedBefore.Value = FilterIfModifiedBefore.Value;
                dateTimePickerModifiedBefore.Enabled = true;
                cbFilterIfModifiedBefore.Checked = true;
            }

            if (FilterIfModifiedSince.HasValue)
            {
                dateTimePickerModifiedSince.Value = FilterIfModifiedSince.Value;
                dateTimePickerModifiedSince.Enabled = true;
                cbFilterIfModifiedSince.Checked = true;
            }
        }

        private void buttonSetDateFilters_Click(object sender, EventArgs e)
        {
            DateRanges = new DateRemover.DateRanges()
            {
                FilterIfOlderThan = dateTimePickerOlder.Enabled ? dateTimePickerOlder.Value as DateTime? : null,
                FilterIfNewerThan = dateTimePickerNewer.Enabled ? dateTimePickerNewer.Value as DateTime? : null,
                FilterIfModifiedBefore = dateTimePickerModifiedBefore.Enabled ? dateTimePickerModifiedBefore.Value as DateTime? : null,
                FilterIfModifiedSince = dateTimePickerModifiedSince.Enabled ? dateTimePickerModifiedSince.Value as DateTime? : null
            };

            Console.WriteLine("Filtering files not created between dates {0} and {1}", 
                FilterIfOlder != null ? FilterIfOlder.ToString() : "any", 
                FilterIfNewer != null ? FilterIfNewer.ToString() : "any");

            Console.WriteLine("Filtering files not modified between dates {0} and {1}",
                FilterIfModifiedBefore != null ? FilterIfModifiedBefore.ToString() : "any",
                FilterIfModifiedSince != null ? FilterIfModifiedSince.ToString() : "any");
        }

        private void cbFilterIfOlder_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerOlder.Enabled = cbFilterIfOlder.Checked;
        }

        private void cbFilterIfNewer_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerNewer.Enabled = cbFilterIfNewer.Checked;
        }

        private void cbFilterIfModifiedBefore_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerModifiedBefore.Enabled = cbFilterIfModifiedBefore.Checked;
        }

        private void cbFilterIfModifiedSince_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerModifiedSince.Enabled = cbFilterIfModifiedSince.Checked;
        }
    }
}
