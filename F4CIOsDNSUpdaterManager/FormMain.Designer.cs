namespace F4CIOsDNSUpdaterManager
{
    partial class FormMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.btnCancel = new System.Windows.Forms.Button();
			this.tbUrl = new System.Windows.Forms.TextBox();
			this.btnTest = new System.Windows.Forms.Button();
			this.wbResponse = new System.Windows.Forms.WebBrowser();
			this.nudInterval = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.scMain = new System.ServiceProcess.ServiceController();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.tbServiceStatus = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tmrStatusRefresher = new System.Windows.Forms.Timer(this.components);
			this.niMain = new System.Windows.Forms.NotifyIcon(this.components);
			this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiShow = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
			this.label4 = new System.Windows.Forms.Label();
			this.tbMailServerAddress = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.tbMailServerUsername = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.cbMailServerIsOffice365 = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tbMailTo = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbMailServerPassword = new System.Windows.Forms.MaskedTextBox();
			this.btnTestMail = new System.Windows.Forms.Button();
			this.tbMailSubject = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.nudMailServerPort = new System.Windows.Forms.NumericUpDown();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.nudRetryAfterXMinutes = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.nudInterval)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.cmsMain.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMailServerPort)).BeginInit();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudRetryAfterXMinutes)).BeginInit();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(545, 339);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// tbUrl
			// 
			this.tbUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbUrl.Location = new System.Drawing.Point(9, 19);
			this.tbUrl.Name = "tbUrl";
			this.tbUrl.Size = new System.Drawing.Size(512, 20);
			this.tbUrl.TabIndex = 1;
			// 
			// btnTest
			// 
			this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTest.Location = new System.Drawing.Point(527, 16);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(75, 23);
			this.btnTest.TabIndex = 2;
			this.btnTest.Text = "Test";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// wbResponse
			// 
			this.wbResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.wbResponse.Location = new System.Drawing.Point(9, 45);
			this.wbResponse.MinimumSize = new System.Drawing.Size(20, 20);
			this.wbResponse.Name = "wbResponse";
			this.wbResponse.Size = new System.Drawing.Size(593, 86);
			this.wbResponse.TabIndex = 3;
			// 
			// nudInterval
			// 
			this.nudInterval.Location = new System.Drawing.Point(51, 12);
			this.nudInterval.Maximum = new decimal(new int[] {
            10080,
            0,
            0,
            0});
			this.nudInterval.Name = "nudInterval";
			this.nudInterval.Size = new System.Drawing.Size(120, 20);
			this.nudInterval.TabIndex = 4;
			this.nudInterval.ValueChanged += new System.EventHandler(this.nudInterval_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Interval";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(177, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "minutes";
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Location = new System.Drawing.Point(464, 339);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 8;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// scMain
			// 
			this.scMain.ServiceName = "F4CIOsDNSUpdater";
			// 
			// btnStop
			// 
			this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStop.Location = new System.Drawing.Point(527, 22);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 9;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnStart
			// 
			this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStart.Location = new System.Drawing.Point(446, 22);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 10;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 32);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 13);
			this.label5.TabIndex = 12;
			this.label5.Text = "Status";
			// 
			// tbServiceStatus
			// 
			this.tbServiceStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbServiceStatus.Location = new System.Drawing.Point(49, 25);
			this.tbServiceStatus.Name = "tbServiceStatus";
			this.tbServiceStatus.ReadOnly = true;
			this.tbServiceStatus.Size = new System.Drawing.Size(391, 20);
			this.tbServiceStatus.TabIndex = 13;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.tbServiceStatus);
			this.groupBox1.Controls.Add(this.btnStop);
			this.groupBox1.Controls.Add(this.btnStart);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Location = new System.Drawing.Point(12, 282);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(608, 51);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Service F4CIOsDNSUpdater";
			// 
			// tmrStatusRefresher
			// 
			this.tmrStatusRefresher.Interval = 3000;
			this.tmrStatusRefresher.Tick += new System.EventHandler(this.tmrStatusRefresher_Tick);
			// 
			// niMain
			// 
			this.niMain.BalloonTipTitle = "F4CIOsDNSUpdaterManager";
			this.niMain.ContextMenuStrip = this.cmsMain;
			this.niMain.Icon = ((System.Drawing.Icon)(resources.GetObject("niMain.Icon")));
			this.niMain.Text = "F4CIOsDNSUpdaterManager";
			this.niMain.Visible = true;
			this.niMain.BalloonTipShown += new System.EventHandler(this.niF4CIOsDNSUpdaterManager_BalloonTipShown);
			this.niMain.DoubleClick += new System.EventHandler(this.niF4CIOsDNSUpdaterManager_DoubleClick);
			// 
			// cmsMain
			// 
			this.cmsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShow,
            this.tsmiAbout,
            this.tsmiExit});
			this.cmsMain.Name = "cmsMain";
			this.cmsMain.Size = new System.Drawing.Size(108, 70);
			this.cmsMain.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsMain_ItemClicked);
			// 
			// tsmiShow
			// 
			this.tsmiShow.Name = "tsmiShow";
			this.tsmiShow.Size = new System.Drawing.Size(107, 22);
			this.tsmiShow.Text = "Show";
			this.tsmiShow.Click += new System.EventHandler(this.tsmiShow_Click);
			// 
			// tsmiAbout
			// 
			this.tsmiAbout.Name = "tsmiAbout";
			this.tsmiAbout.Size = new System.Drawing.Size(107, 22);
			this.tsmiAbout.Text = "About";
			this.tsmiAbout.Visible = false;
			// 
			// tsmiExit
			// 
			this.tsmiExit.Name = "tsmiExit";
			this.tsmiExit.Size = new System.Drawing.Size(107, 22);
			this.tsmiExit.Text = "Exit";
			this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(25, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(75, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Use Office365";
			// 
			// tbMailServerAddress
			// 
			this.tbMailServerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbMailServerAddress.Location = new System.Drawing.Point(106, 32);
			this.tbMailServerAddress.Name = "tbMailServerAddress";
			this.tbMailServerAddress.Size = new System.Drawing.Size(138, 20);
			this.tbMailServerAddress.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(103, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(79, 13);
			this.label6.TabIndex = 7;
			this.label6.Text = "Server Address";
			// 
			// tbMailServerUsername
			// 
			this.tbMailServerUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbMailServerUsername.Location = new System.Drawing.Point(313, 32);
			this.tbMailServerUsername.Name = "tbMailServerUsername";
			this.tbMailServerUsername.Size = new System.Drawing.Size(97, 20);
			this.tbMailServerUsername.TabIndex = 1;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(244, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(60, 13);
			this.label7.TabIndex = 7;
			this.label7.Text = "Server Port";
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(310, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(55, 13);
			this.label8.TabIndex = 7;
			this.label8.Text = "Username";
			// 
			// cbMailServerIsOffice365
			// 
			this.cbMailServerIsOffice365.AutoSize = true;
			this.cbMailServerIsOffice365.Location = new System.Drawing.Point(85, 35);
			this.cbMailServerIsOffice365.Name = "cbMailServerIsOffice365";
			this.cbMailServerIsOffice365.Size = new System.Drawing.Size(15, 14);
			this.cbMailServerIsOffice365.TabIndex = 15;
			this.cbMailServerIsOffice365.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.tbMailTo);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.tbMailServerPassword);
			this.groupBox2.Controls.Add(this.btnTestMail);
			this.groupBox2.Controls.Add(this.tbMailSubject);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.nudMailServerPort);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.cbMailServerIsOffice365);
			this.groupBox2.Controls.Add(this.tbMailServerAddress);
			this.groupBox2.Controls.Add(this.tbMailServerUsername);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Location = new System.Drawing.Point(12, 181);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(608, 95);
			this.groupBox2.TabIndex = 16;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Mail Alert";
			// 
			// tbMailTo
			// 
			this.tbMailTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbMailTo.Location = new System.Drawing.Point(527, 32);
			this.tbMailTo.Name = "tbMailTo";
			this.tbMailTo.Size = new System.Drawing.Size(75, 20);
			this.tbMailTo.TabIndex = 23;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(524, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(20, 13);
			this.label3.TabIndex = 22;
			this.label3.Text = "To";
			// 
			// tbMailServerPassword
			// 
			this.tbMailServerPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbMailServerPassword.HidePromptOnLeave = true;
			this.tbMailServerPassword.Location = new System.Drawing.Point(416, 32);
			this.tbMailServerPassword.Name = "tbMailServerPassword";
			this.tbMailServerPassword.PasswordChar = '*';
			this.tbMailServerPassword.Size = new System.Drawing.Size(105, 20);
			this.tbMailServerPassword.TabIndex = 21;
			// 
			// btnTestMail
			// 
			this.btnTestMail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTestMail.Location = new System.Drawing.Point(527, 58);
			this.btnTestMail.Name = "btnTestMail";
			this.btnTestMail.Size = new System.Drawing.Size(75, 23);
			this.btnTestMail.TabIndex = 20;
			this.btnTestMail.Text = "Test";
			this.btnTestMail.UseVisualStyleBackColor = true;
			this.btnTestMail.Click += new System.EventHandler(this.btnTestMail_Click);
			// 
			// tbMailSubject
			// 
			this.tbMailSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbMailSubject.Location = new System.Drawing.Point(85, 61);
			this.tbMailSubject.Name = "tbMailSubject";
			this.tbMailSubject.Size = new System.Drawing.Size(436, 20);
			this.tbMailSubject.TabIndex = 19;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(36, 68);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(43, 13);
			this.label10.TabIndex = 18;
			this.label10.Text = "Subject";
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(413, 16);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(53, 13);
			this.label9.TabIndex = 17;
			this.label9.Text = "Password";
			// 
			// nudMailServerPort
			// 
			this.nudMailServerPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudMailServerPort.Location = new System.Drawing.Point(250, 32);
			this.nudMailServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.nudMailServerPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudMailServerPort.Name = "nudMailServerPort";
			this.nudMailServerPort.Size = new System.Drawing.Size(57, 20);
			this.nudMailServerPort.TabIndex = 16;
			this.nudMailServerPort.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.tbUrl);
			this.groupBox3.Controls.Add(this.btnTest);
			this.groupBox3.Controls.Add(this.wbResponse);
			this.groupBox3.Location = new System.Drawing.Point(12, 38);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(608, 137);
			this.groupBox3.TabIndex = 17;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Target Url (to HealthStatus.aspx)";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(497, 19);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(123, 13);
			this.label11.TabIndex = 20;
			this.label11.Text = "minutes (set 0 to disable)";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(273, 19);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(158, 13);
			this.label12.TabIndex = 19;
			this.label12.Text = "On negative response retry after";
			// 
			// nudRetryAfterXMinutes
			// 
			this.nudRetryAfterXMinutes.Location = new System.Drawing.Point(437, 12);
			this.nudRetryAfterXMinutes.Maximum = new decimal(new int[] {
            36000,
            0,
            0,
            0});
			this.nudRetryAfterXMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudRetryAfterXMinutes.Name = "nudRetryAfterXMinutes";
			this.nudRetryAfterXMinutes.Size = new System.Drawing.Size(51, 20);
			this.nudRetryAfterXMinutes.TabIndex = 18;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 374);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.nudRetryAfterXMinutes);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nudInterval);
			this.Controls.Add(this.btnCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(648, 413);
			this.Name = "FormMain";
			this.ShowInTaskbar = false;
			this.Text = "F4CIOsDNSUpdaterManager";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.Load += new System.EventHandler(this.F4CIOsDNSUpdaterManager_Load);
			this.VisibleChanged += new System.EventHandler(this.F4CIOsDNSUpdaterManager_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.nudInterval)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.cmsMain.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMailServerPort)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudRetryAfterXMinutes)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.WebBrowser wbResponse;
        private System.Windows.Forms.NumericUpDown nudInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOk;
        private System.ServiceProcess.ServiceController scMain;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbServiceStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer tmrStatusRefresher;
        private System.Windows.Forms.NotifyIcon niMain;
        private System.Windows.Forms.ContextMenuStrip cmsMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiShow;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbMailServerAddress;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbMailServerUsername;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox cbMailServerIsOffice365;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnTestMail;
		private System.Windows.Forms.TextBox tbMailSubject;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown nudMailServerPort;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.MaskedTextBox tbMailServerPassword;
		private System.Windows.Forms.TextBox tbMailTo;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.NumericUpDown nudRetryAfterXMinutes;
	}
}

