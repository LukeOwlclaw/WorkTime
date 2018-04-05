using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using WorkTime.Properties;
using Microsoft.Win32;

namespace Network_Monitor_Sample
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class FormMain : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label LabelTotalTime;
        private System.Windows.Forms.Label LabelActiveTime;
        private System.Windows.Forms.Label LabelTotalTimeValue;
        private System.Windows.Forms.Label LabelActiveTimeValue;
        private System.Windows.Forms.Timer TimerCounter;
        private System.ComponentModel.IContainer components;


        public const int WM_NCLBUTTONDOWN = 0xA1;
        private TextBox TextBoxLog;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]

        public static extern bool ReleaseCapture();

        public DateTime Started { get; set; }
        public DateTime? StartOfLastPause { get; set; } = null;
        public DateTime? StartOfLastActivity { get; set; } = null;
        public TimeSpan ActiveTime { get; set; }

        public FormMain()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Started = DateTime.Now;
            StartOfLastActivity = DateTime.Now;

            LabelActiveTime_Click(null, null);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.LabelTotalTime = new System.Windows.Forms.Label();
            this.LabelActiveTime = new System.Windows.Forms.Label();
            this.LabelTotalTimeValue = new System.Windows.Forms.Label();
            this.LabelActiveTimeValue = new System.Windows.Forms.Label();
            this.TimerCounter = new System.Windows.Forms.Timer(this.components);
            this.TextBoxLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LabelTotalTime
            // 
            this.LabelTotalTime.Location = new System.Drawing.Point(0, 22);
            this.LabelTotalTime.Name = "LabelTotalTime";
            this.LabelTotalTime.Size = new System.Drawing.Size(41, 23);
            this.LabelTotalTime.TabIndex = 1;
            this.LabelTotalTime.Text = "Total:";
            this.LabelTotalTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LabelActiveTime
            // 
            this.LabelActiveTime.Location = new System.Drawing.Point(0, 0);
            this.LabelActiveTime.Name = "LabelActiveTime";
            this.LabelActiveTime.Size = new System.Drawing.Size(41, 23);
            this.LabelActiveTime.TabIndex = 2;
            this.LabelActiveTime.Text = "Active:";
            this.LabelActiveTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LabelActiveTime.Click += new System.EventHandler(this.LabelActiveTime_Click);
            // 
            // LabelTotalTimeValue
            // 
            this.LabelTotalTimeValue.Location = new System.Drawing.Point(47, 23);
            this.LabelTotalTimeValue.Name = "LabelTotalTimeValue";
            this.LabelTotalTimeValue.Size = new System.Drawing.Size(100, 23);
            this.LabelTotalTimeValue.TabIndex = 3;
            this.LabelTotalTimeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelActiveTimeValue
            // 
            this.LabelActiveTimeValue.Location = new System.Drawing.Point(47, 0);
            this.LabelActiveTimeValue.Name = "LabelActiveTimeValue";
            this.LabelActiveTimeValue.Size = new System.Drawing.Size(100, 23);
            this.LabelActiveTimeValue.TabIndex = 4;
            this.LabelActiveTimeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TimerCounter
            // 
            this.TimerCounter.Interval = 1000;
            this.TimerCounter.Tick += new System.EventHandler(this.TimerCounter_Tick);
            // 
            // TextBoxLog
            // 
            this.TextBoxLog.Location = new System.Drawing.Point(153, 0);
            this.TextBoxLog.Multiline = true;
            this.TextBoxLog.Name = "TextBoxLog";
            this.TextBoxLog.Size = new System.Drawing.Size(205, 92);
            this.TextBoxLog.TabIndex = 5;
            this.TextBoxLog.TextChanged += new System.EventHandler(this.TextBoxLog_TextChanged);
            // 
            // FormMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(370, 104);
            this.ControlBox = false;
            this.Controls.Add(this.TextBoxLog);
            this.Controls.Add(this.LabelActiveTimeValue);
            this.Controls.Add(this.LabelTotalTimeValue);
            this.Controls.Add(this.LabelActiveTime);
            this.Controls.Add(this.LabelTotalTime);
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "WorkTime";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMain_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new FormMain());
        }

        
        private void FormMain_Load(object sender, System.EventArgs e)
        {
            // Set window location
            if (Settings.Default.WindowLocation != null)
            {
                this.Location = Settings.Default.WindowLocation;
            }

            this.TimerCounter.Start();

            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }
        void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                if (StartOfLastActivity.HasValue)
                {
                    ActiveTime += (DateTime.Now - StartOfLastActivity.Value);
                }
                TextBoxLog.Text += DateTime.Now + " PAUSE" + Environment.NewLine;
                StartOfLastPause = DateTime.Now;
                StartOfLastActivity = null;
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                TextBoxLog.Text += DateTime.Now + " RESUME" + Environment.NewLine;
                StartOfLastPause = null;
                StartOfLastActivity = DateTime.Now;
            }
        }
        void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
        }

        string format = "HH:mm:ss";
        private void TimerCounter_Tick(object sender, System.EventArgs e)
        {
            var total = DateTime.Now - Started;
            this.LabelTotalTimeValue.Text = new DateTime(total.Ticks).ToString(format);
            if (StartOfLastActivity.HasValue)
            {
                var active = ActiveTime + (DateTime.Now - StartOfLastActivity.Value);
                this.LabelActiveTimeValue.Text = new DateTime(active.Ticks).ToString(format);
            }
        }

        private void LabelActiveTime_Click(object sender, EventArgs e)
        {
            if (!this.TopMost)
            {
                var l = this.Left;
                var t = this.Top ;

                this.LabelTotalTime.Visible = false;
                this.LabelTotalTimeValue.Visible = false;
                this.TextBoxLog.Visible = false;
                format = "HH:mm";
                LabelActiveTimeValue.AutoSize = true;
                LabelActiveTime.AutoSize = true;

                this.TopMost = true;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.ControlBox = false;
                this.ShowInTaskbar = false;
            }
            else
            {
                this.LabelTotalTime.Visible = true;
                this.LabelTotalTimeValue.Visible = true;
                this.TextBoxLog.Visible = true;
                format = "HH:mm:ss";
                LabelActiveTimeValue.AutoSize = false;
                LabelActiveTime.AutoSize = false;

                this.TopMost = false;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.ControlBox = true;
                this.ShowInTaskbar = true;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                //HIDE WINDOW FROM ALT-TAB-LISTING:
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Copy window location to app settings
            Settings.Default.WindowLocation = this.Location;

            // Save settings
            Settings.Default.Save();
        }

        private void TextBoxLog_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(TextBoxLog.Text, TextBoxLog.Font);
            TextBoxLog.Width = size.Width + 20;
            TextBoxLog.Height = size.Height + 10;
        }
    }
}
