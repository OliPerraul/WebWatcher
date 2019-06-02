using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace WebWatcher
{
    public class WebWatcherForm : System.Windows.Forms.Form
    {
        private NotifyIcon _notifyIcon1;

        private Firefox _browser;

        private FileSystemWatcher _watcher;

        private Dictionary<string, int> _counter;

        private long _lastTime;

        public WebWatcherForm()
        {
            InitializeComponent();
            SetBalloonTip();

            _browser = new Firefox();

            _watcher = new FileSystemWatcher();

            _watcher.IncludeSubdirectories = true;

            _watcher.Path = _browser.BrowserPath;

  
            _counter = new Dictionary<string, int>();

            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            _watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // Only watch text files.
            _watcher.Filter = "*.sqlite";

            // Add event handlers.
            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Deleted += OnChanged;

            // Begin watching.
            _watcher.EnableRaisingEvents = true;


            // Hide on startup
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

        }


        private void OnChanged(object sender, FileSystemEventArgs args)
        {
            var urls = _browser.GetHistory();

            if (urls.Count() != 0)
            {
                var url = urls.First();

                if (url.time == _lastTime)
                    return;

                _lastTime = url.time;

                if (_counter.ContainsKey(url.host))
                {
                    _counter[url.host]++;
                }
                else
                {
                    _counter.Add(urls.First().host, 1);
                }   

                _notifyIcon1.Visible = true;
                _notifyIcon1.ShowBalloonTip(20000, $"WebWatcher: Visting {url.host} '{_counter[url.host]}' times this session.", $"Enjoy '{url.title}'",
                    ToolTipIcon.Info);
            }
        }


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new WebWatcherForm());
        }

        private void SetBalloonTip()
        {
            _notifyIcon1.Icon = SystemIcons.Exclamation;
            _notifyIcon1.BalloonTipTitle = "Balloon Tip Title";
            _notifyIcon1.BalloonTipText = "Balloon Tip Text.";
            _notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
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
            this.components = new System.ComponentModel.Container();
            this._notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);

            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this._notifyIcon1.Visible = true;
            // 

            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

    }
}