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
            this.components = new System.ComponentModel.Container();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.tvData = new System.Windows.Forms.TreeView();
            this.btnShowData = new System.Windows.Forms.Button();
            this.tMetricPing = new System.Windows.Forms.Timer(this.components);
            this.tRefreshSession = new System.Windows.Forms.Timer(this.components);
            this.cBrowser = new CefSharp.WinForms.ChromiumWebBrowser();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusEvoSessionId = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnExtractSessionId = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstLog
            // 
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.Location = new System.Drawing.Point(0, 0);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(743, 223);
            this.lstLog.TabIndex = 0;
            // 
            // tvData
            // 
            this.tvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvData.Location = new System.Drawing.Point(0, 0);
            this.tvData.Name = "tvData";
            this.tvData.Size = new System.Drawing.Size(373, 624);
            this.tvData.TabIndex = 5;
            // 
            // btnShowData
            // 
            this.btnShowData.Location = new System.Drawing.Point(251, 12);
            this.btnShowData.Name = "btnShowData";
            this.btnShowData.Size = new System.Drawing.Size(75, 23);
            this.btnShowData.TabIndex = 6;
            this.btnShowData.Text = "Show data";
            this.btnShowData.UseVisualStyleBackColor = true;
            this.btnShowData.Click += new System.EventHandler(this.btnShowData_Click);
            // 
            // tMetricPing
            // 
            this.tMetricPing.Interval = 20000;
            this.tMetricPing.Tick += new System.EventHandler(this.tMetricPing_Tick);
            // 
            // tRefreshSession
            // 
            this.tRefreshSession.Interval = 60000;
            this.tRefreshSession.Tick += new System.EventHandler(this.tRefreshSession_Tick);
            // 
            // cBrowser
            // 
            this.cBrowser.ActivateBrowserOnCreation = false;
// TODO: Code generation for '' failed because of Exception 'Invalid Primitive Type: System.IntPtr. Consider using CodeObjectCreateExpression.'.
            this.cBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cBrowser.Location = new System.Drawing.Point(0, 0);
            this.cBrowser.Name = "cBrowser";
            this.cBrowser.Size = new System.Drawing.Size(743, 397);
            this.cBrowser.TabIndex = 7;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusMain,
            this.toolStripStatusEvoSessionId});
            this.statusStrip1.Location = new System.Drawing.Point(0, 665);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1120, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusMain
            // 
            this.toolStripStatusMain.Name = "toolStripStatusMain";
            this.toolStripStatusMain.Size = new System.Drawing.Size(48, 17);
            this.toolStripStatusMain.Text = "Waiting";
            // 
            // toolStripStatusEvoSessionId
            // 
            this.toolStripStatusEvoSessionId.Name = "toolStripStatusEvoSessionId";
            this.toolStripStatusEvoSessionId.Size = new System.Drawing.Size(0, 17);
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(12, 12);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(105, 23);
            this.btnLogin.TabIndex = 9;
            this.btnLogin.Text = "Login & Fetch";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnExtractSessionId
            // 
            this.btnExtractSessionId.Enabled = false;
            this.btnExtractSessionId.Location = new System.Drawing.Point(123, 12);
            this.btnExtractSessionId.Name = "btnExtractSessionId";
            this.btnExtractSessionId.Size = new System.Drawing.Size(122, 23);
            this.btnExtractSessionId.TabIndex = 11;
            this.btnExtractSessionId.Text = "Extract EVO session";
            this.btnExtractSessionId.UseVisualStyleBackColor = true;
            this.btnExtractSessionId.Click += new System.EventHandler(this.btnExtractSessionId_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 41);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvData);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1120, 624);
            this.splitContainer1.SplitterDistance = 373;
            this.splitContainer1.TabIndex = 12;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.cBrowser);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lstLog);
            this.splitContainer2.Size = new System.Drawing.Size(743, 624);
            this.splitContainer2.SplitterDistance = 397;
            this.splitContainer2.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(332, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1120, 687);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnExtractSessionId);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnShowData);
            this.Name = "fMain";
            this.Text = "BCR";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fMain_FormClosing);
            this.Load += new System.EventHandler(this.fMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.TreeView tvData;
        private System.Windows.Forms.Button btnShowData;
        private System.Windows.Forms.Timer tMetricPing;
        private System.Windows.Forms.Timer tRefreshSession;
        private CefSharp.WinForms.ChromiumWebBrowser cBrowser;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnExtractSessionId;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusEvoSessionId;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button button1;
    }
}

