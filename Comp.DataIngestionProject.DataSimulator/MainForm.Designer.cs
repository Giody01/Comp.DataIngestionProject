namespace Comp.DataIngestionProject.DataSimulator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SendDataButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SendDataButton
            // 
            this.SendDataButton.Location = new System.Drawing.Point(290, 185);
            this.SendDataButton.Name = "SendDataButton";
            this.SendDataButton.Size = new System.Drawing.Size(230, 60);
            this.SendDataButton.TabIndex = 0;
            this.SendDataButton.Text = "Send Data to IoTHub";
            this.SendDataButton.UseVisualStyleBackColor = true;
            this.SendDataButton.Click += new System.EventHandler(this.SendDataButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SendDataButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Button SendDataButton;
    }
}