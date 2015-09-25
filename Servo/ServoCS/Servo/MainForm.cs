using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServoCS.Servo
{
    class MainForm : Form
    {
        readonly IService _svc;

        public MainForm(IService svc)
        {
            InitializeComponent();

            _svc = svc;
            Start();
        }

        private async void stopBtn_Click(object sender, EventArgs e)
        {
            await Stop();
        }

        async void Start()
        {
            _stopBtn.Enabled = false;
            await Task.Factory.StartNew(() => _svc.OnStart(Environment.GetCommandLineArgs()), TaskCreationOptions.LongRunning);
            _stopBtn.Enabled = true;
        }

        async Task Stop()
        {
            _stopBtn.Enabled = false;
            await Task.Factory.StartNew(() => _svc.OnStop(), TaskCreationOptions.LongRunning);
            Close();
        }

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
            this._stopBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // stopBtn
            // 
            this._stopBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._stopBtn.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._stopBtn.Location = new System.Drawing.Point(12, 12);
            this._stopBtn.Name = "_stopBtn";
            this._stopBtn.Size = new System.Drawing.Size(218, 72);
            this._stopBtn.TabIndex = 0;
            this._stopBtn.Text = "Stop";
            this._stopBtn.UseVisualStyleBackColor = true;
            this._stopBtn.Click += new System.EventHandler(this.stopBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 96);
            this.Controls.Add(this._stopBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Servo App";
            this.ResumeLayout(false);
        }

        #endregion

        private Button _stopBtn;
    }
}