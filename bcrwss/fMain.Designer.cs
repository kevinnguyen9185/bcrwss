namespace bcrwss
{
    partial class fMain
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
            this.lstLog = new System.Windows.Forms.ListBox();
            this.btnSyncTables = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnTableUpdate = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lstLog
            // 
            this.lstLog.FormattingEnabled = true;
            this.lstLog.Location = new System.Drawing.Point(481, 12);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(368, 433);
            this.lstLog.TabIndex = 0;
            // 
            // btnSyncTables
            // 
            this.btnSyncTables.Location = new System.Drawing.Point(12, 12);
            this.btnSyncTables.Name = "btnSyncTables";
            this.btnSyncTables.Size = new System.Drawing.Size(133, 23);
            this.btnSyncTables.TabIndex = 1;
            this.btnSyncTables.Text = "Get Lobby Info";
            this.btnSyncTables.UseVisualStyleBackColor = true;
            this.btnSyncTables.Click += new System.EventHandler(this.btnGetLobbyInfo_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 41);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(463, 20);
            this.textBox1.TabIndex = 2;
            // 
            // btnTableUpdate
            // 
            this.btnTableUpdate.Location = new System.Drawing.Point(151, 12);
            this.btnTableUpdate.Name = "btnTableUpdate";
            this.btnTableUpdate.Size = new System.Drawing.Size(106, 23);
            this.btnTableUpdate.TabIndex = 3;
            this.btnTableUpdate.Text = "Update tables";
            this.btnTableUpdate.UseVisualStyleBackColor = true;
            this.btnTableUpdate.Click += new System.EventHandler(this.btnTableUpdate_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 67);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(463, 378);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 450);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnTableUpdate);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnSyncTables);
            this.Controls.Add(this.lstLog);
            this.Name = "fMain";
            this.Text = "BCR";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fMain_FormClosing);
            this.Load += new System.EventHandler(this.fMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button btnSyncTables;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnTableUpdate;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

