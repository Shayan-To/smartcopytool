namespace SmartCopyTool
{
    partial class FormDateFilters
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
            this.dateTimePickerOlder = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerNewer = new System.Windows.Forms.DateTimePicker();
            this.buttonSetDateFilters = new System.Windows.Forms.Button();
            this.cbFilterIfOlder = new System.Windows.Forms.CheckBox();
            this.cbFilterIfNewer = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dateTimePickerModifiedBefore = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerModifiedSince = new System.Windows.Forms.DateTimePicker();
            this.cbFilterIfModifiedBefore = new System.Windows.Forms.CheckBox();
            this.cbFilterIfModifiedSince = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // dateTimePickerOlder
            // 
            this.dateTimePickerOlder.Enabled = false;
            this.dateTimePickerOlder.Location = new System.Drawing.Point(56, 43);
            this.dateTimePickerOlder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dateTimePickerOlder.Name = "dateTimePickerOlder";
            this.dateTimePickerOlder.Size = new System.Drawing.Size(265, 22);
            this.dateTimePickerOlder.TabIndex = 1;
            // 
            // dateTimePickerNewer
            // 
            this.dateTimePickerNewer.Enabled = false;
            this.dateTimePickerNewer.Location = new System.Drawing.Point(56, 118);
            this.dateTimePickerNewer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dateTimePickerNewer.Name = "dateTimePickerNewer";
            this.dateTimePickerNewer.Size = new System.Drawing.Size(265, 22);
            this.dateTimePickerNewer.TabIndex = 1;
            // 
            // buttonSetDateFilters
            // 
            this.buttonSetDateFilters.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSetDateFilters.Location = new System.Drawing.Point(226, 322);
            this.buttonSetDateFilters.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSetDateFilters.Name = "buttonSetDateFilters";
            this.buttonSetDateFilters.Size = new System.Drawing.Size(100, 28);
            this.buttonSetDateFilters.TabIndex = 2;
            this.buttonSetDateFilters.Text = "Filter files";
            this.buttonSetDateFilters.UseVisualStyleBackColor = true;
            this.buttonSetDateFilters.Click += new System.EventHandler(this.buttonSetDateFilters_Click);
            // 
            // cbFilterIfOlder
            // 
            this.cbFilterIfOlder.AutoSize = true;
            this.cbFilterIfOlder.Location = new System.Drawing.Point(21, 15);
            this.cbFilterIfOlder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbFilterIfOlder.Name = "cbFilterIfOlder";
            this.cbFilterIfOlder.Size = new System.Drawing.Size(158, 21);
            this.cbFilterIfOlder.TabIndex = 3;
            this.cbFilterIfOlder.Text = "Filter files older than";
            this.cbFilterIfOlder.UseVisualStyleBackColor = true;
            this.cbFilterIfOlder.CheckedChanged += new System.EventHandler(this.cbFilterIfOlder_CheckedChanged);
            // 
            // cbFilterIfNewer
            // 
            this.cbFilterIfNewer.AutoSize = true;
            this.cbFilterIfNewer.Location = new System.Drawing.Point(21, 90);
            this.cbFilterIfNewer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbFilterIfNewer.Name = "cbFilterIfNewer";
            this.cbFilterIfNewer.Size = new System.Drawing.Size(164, 21);
            this.cbFilterIfNewer.TabIndex = 3;
            this.cbFilterIfNewer.Text = "Filter files newer than";
            this.cbFilterIfNewer.UseVisualStyleBackColor = true;
            this.cbFilterIfNewer.CheckedChanged += new System.EventHandler(this.cbFilterIfNewer_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(118, 322);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // dateTimePickerModifiedBefore
            // 
            this.dateTimePickerModifiedBefore.Enabled = false;
            this.dateTimePickerModifiedBefore.Location = new System.Drawing.Point(56, 198);
            this.dateTimePickerModifiedBefore.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerModifiedBefore.Name = "dateTimePickerModifiedBefore";
            this.dateTimePickerModifiedBefore.Size = new System.Drawing.Size(265, 22);
            this.dateTimePickerModifiedBefore.TabIndex = 1;
            // 
            // dateTimePickerModifiedSince
            // 
            this.dateTimePickerModifiedSince.Enabled = false;
            this.dateTimePickerModifiedSince.Location = new System.Drawing.Point(56, 273);
            this.dateTimePickerModifiedSince.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerModifiedSince.Name = "dateTimePickerModifiedSince";
            this.dateTimePickerModifiedSince.Size = new System.Drawing.Size(265, 22);
            this.dateTimePickerModifiedSince.TabIndex = 1;
            // 
            // cbFilterIfModifiedBefore
            // 
            this.cbFilterIfModifiedBefore.AutoSize = true;
            this.cbFilterIfModifiedBefore.Location = new System.Drawing.Point(21, 170);
            this.cbFilterIfModifiedBefore.Margin = new System.Windows.Forms.Padding(4);
            this.cbFilterIfModifiedBefore.Name = "cbFilterIfModifiedBefore";
            this.cbFilterIfModifiedBefore.Size = new System.Drawing.Size(163, 21);
            this.cbFilterIfModifiedBefore.TabIndex = 3;
            this.cbFilterIfModifiedBefore.Text = "Filter modified before";
            this.cbFilterIfModifiedBefore.UseVisualStyleBackColor = true;
            this.cbFilterIfModifiedBefore.CheckedChanged += new System.EventHandler(this.cbFilterIfModifiedBefore_CheckedChanged);
            // 
            // cbFilterIfModifiedSince
            // 
            this.cbFilterIfModifiedSince.AutoSize = true;
            this.cbFilterIfModifiedSince.Location = new System.Drawing.Point(21, 245);
            this.cbFilterIfModifiedSince.Margin = new System.Windows.Forms.Padding(4);
            this.cbFilterIfModifiedSince.Name = "cbFilterIfModifiedSince";
            this.cbFilterIfModifiedSince.Size = new System.Drawing.Size(155, 21);
            this.cbFilterIfModifiedSince.TabIndex = 3;
            this.cbFilterIfModifiedSince.Text = "Filter modified since";
            this.cbFilterIfModifiedSince.UseVisualStyleBackColor = true;
            this.cbFilterIfModifiedSince.CheckedChanged += new System.EventHandler(this.cbFilterIfModifiedSince_CheckedChanged);
            // 
            // FormDateFilters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 361);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cbFilterIfModifiedSince);
            this.Controls.Add(this.cbFilterIfModifiedBefore);
            this.Controls.Add(this.cbFilterIfNewer);
            this.Controls.Add(this.cbFilterIfOlder);
            this.Controls.Add(this.dateTimePickerModifiedSince);
            this.Controls.Add(this.buttonSetDateFilters);
            this.Controls.Add(this.dateTimePickerModifiedBefore);
            this.Controls.Add(this.dateTimePickerNewer);
            this.Controls.Add(this.dateTimePickerOlder);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormDateFilters";
            this.Text = "Filter by date";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePickerOlder;
        private System.Windows.Forms.DateTimePicker dateTimePickerNewer;
        private System.Windows.Forms.Button buttonSetDateFilters;
        private System.Windows.Forms.CheckBox cbFilterIfOlder;
        private System.Windows.Forms.CheckBox cbFilterIfNewer;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DateTimePicker dateTimePickerModifiedBefore;
        private System.Windows.Forms.DateTimePicker dateTimePickerModifiedSince;
        private System.Windows.Forms.CheckBox cbFilterIfModifiedBefore;
        private System.Windows.Forms.CheckBox cbFilterIfModifiedSince;
    }
}