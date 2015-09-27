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
            this.SuspendLayout();
            // 
            // dateTimePickerOlder
            // 
            this.dateTimePickerOlder.Enabled = false;
            this.dateTimePickerOlder.Location = new System.Drawing.Point(42, 35);
            this.dateTimePickerOlder.Name = "dateTimePickerOlder";
            this.dateTimePickerOlder.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerOlder.TabIndex = 1;
            // 
            // dateTimePickerNewer
            // 
            this.dateTimePickerNewer.Enabled = false;
            this.dateTimePickerNewer.Location = new System.Drawing.Point(42, 96);
            this.dateTimePickerNewer.Name = "dateTimePickerNewer";
            this.dateTimePickerNewer.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerNewer.TabIndex = 1;
            // 
            // buttonSetDateFilters
            // 
            this.buttonSetDateFilters.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSetDateFilters.Location = new System.Drawing.Point(167, 122);
            this.buttonSetDateFilters.Name = "buttonSetDateFilters";
            this.buttonSetDateFilters.Size = new System.Drawing.Size(75, 23);
            this.buttonSetDateFilters.TabIndex = 2;
            this.buttonSetDateFilters.Text = "Filter files";
            this.buttonSetDateFilters.UseVisualStyleBackColor = true;
            this.buttonSetDateFilters.Click += new System.EventHandler(this.buttonSetDateFilters_Click);
            // 
            // cbFilterIfOlder
            // 
            this.cbFilterIfOlder.AutoSize = true;
            this.cbFilterIfOlder.Location = new System.Drawing.Point(16, 12);
            this.cbFilterIfOlder.Name = "cbFilterIfOlder";
            this.cbFilterIfOlder.Size = new System.Drawing.Size(119, 17);
            this.cbFilterIfOlder.TabIndex = 3;
            this.cbFilterIfOlder.Text = "Filter files older than";
            this.cbFilterIfOlder.UseVisualStyleBackColor = true;
            this.cbFilterIfOlder.CheckedChanged += new System.EventHandler(this.cbFilterIfOlder_CheckedChanged);
            // 
            // cbFilterIfNewer
            // 
            this.cbFilterIfNewer.AutoSize = true;
            this.cbFilterIfNewer.Location = new System.Drawing.Point(16, 73);
            this.cbFilterIfNewer.Name = "cbFilterIfNewer";
            this.cbFilterIfNewer.Size = new System.Drawing.Size(125, 17);
            this.cbFilterIfNewer.TabIndex = 3;
            this.cbFilterIfNewer.Text = "Filter files newer than";
            this.cbFilterIfNewer.UseVisualStyleBackColor = true;
            this.cbFilterIfNewer.CheckedChanged += new System.EventHandler(this.cbFilterIfNewer_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(86, 122);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // FormDateFilters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 154);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cbFilterIfNewer);
            this.Controls.Add(this.cbFilterIfOlder);
            this.Controls.Add(this.buttonSetDateFilters);
            this.Controls.Add(this.dateTimePickerNewer);
            this.Controls.Add(this.dateTimePickerOlder);
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
    }
}