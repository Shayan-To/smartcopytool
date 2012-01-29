namespace SmartCopyTool
{
    partial class FormProgressDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cancelButton = new System.Windows.Forms.Button();
            this.labelTimeTaken = new System.Windows.Forms.Label();
            this.labelTimeRemaining = new System.Windows.Forms.Label();
            this.pauseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Performing some long operation";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Currently doing this";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 67);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(373, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(307, 110);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // labelTimeTaken
            // 
            this.labelTimeTaken.AutoSize = true;
            this.labelTimeTaken.Location = new System.Drawing.Point(13, 107);
            this.labelTimeTaken.Name = "labelTimeTaken";
            this.labelTimeTaken.Size = new System.Drawing.Size(60, 13);
            this.labelTimeTaken.TabIndex = 4;
            this.labelTimeTaken.Text = "Time taken";
            this.labelTimeTaken.Visible = false;
            // 
            // labelTimeRemaining
            // 
            this.labelTimeRemaining.AutoSize = true;
            this.labelTimeRemaining.Location = new System.Drawing.Point(12, 130);
            this.labelTimeRemaining.Name = "labelTimeRemaining";
            this.labelTimeRemaining.Size = new System.Drawing.Size(78, 13);
            this.labelTimeRemaining.TabIndex = 5;
            this.labelTimeRemaining.Text = "Time remaining";
            this.labelTimeRemaining.Visible = false;
            // 
            // pauseButton
            // 
            this.pauseButton.Enabled = false;
            this.pauseButton.Location = new System.Drawing.Point(226, 110);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(75, 23);
            this.pauseButton.TabIndex = 6;
            this.pauseButton.Text = "Pause";
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // FormProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(394, 152);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.labelTimeRemaining);
            this.Controls.Add(this.labelTimeTaken);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormProgressDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Busy";
            this.Load += new System.EventHandler(this.FormProgressDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.ProgressBar progressBar1;
        internal System.Windows.Forms.Button cancelButton;
        internal System.Windows.Forms.Label labelTimeTaken;
        internal System.Windows.Forms.Label labelTimeRemaining;
        public System.Windows.Forms.Button pauseButton;
    }
}