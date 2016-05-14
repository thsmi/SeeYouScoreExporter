namespace net.tschmid.scooring.htmlviewer
{
    partial class HighDpiWebViewer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.loadingThrobber = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.loadingThrobber)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.Margin = new System.Windows.Forms.Padding(6);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(37, 37);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.ScrollBarsEnabled = false;
            this.webBrowser1.Size = new System.Drawing.Size(311, 306);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.TabStop = false;
            this.webBrowser1.Url = new System.Uri("", System.UriKind.Relative);
            this.webBrowser1.WebBrowserShortcutsEnabled = false;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // loadingThrobber
            // 
            this.loadingThrobber.BackgroundImage = global::net.tschmid.scooring.Properties.Resources.darknoise;
            this.loadingThrobber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingThrobber.Image = global::net.tschmid.scooring.Properties.Resources.hourglass;
            this.loadingThrobber.Location = new System.Drawing.Point(0, 0);
            this.loadingThrobber.Name = "loadingThrobber";
            this.loadingThrobber.Size = new System.Drawing.Size(311, 306);
            this.loadingThrobber.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.loadingThrobber.TabIndex = 2;
            this.loadingThrobber.TabStop = false;
            // 
            // HighDpiWebViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.loadingThrobber);
            this.Controls.Add(this.webBrowser1);
            this.Name = "HighDpiWebViewer";
            this.Size = new System.Drawing.Size(311, 306);
            ((System.ComponentModel.ISupportInitialize)(this.loadingThrobber)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.PictureBox loadingThrobber;
    }
}
