using System;
using System.Windows.Forms;

namespace net.tschmid.scooring.htmlviewer
{
    public partial class HighDpiWebViewer : UserControl
    {
        private int y;
        private HighDpiUiHandler highDpiUiHandler;

        public HighDpiWebViewer()
        {
            InitializeComponent();
            ((Control)webBrowser1).Enabled = false;

            highDpiUiHandler = new HighDpiUiHandler(webBrowser1); 
        }

        public void ShowThrobber()
        {
            this.loadingThrobber.Enabled = true;
            this.loadingThrobber.BringToFront();
        }

        public void HideThrobber()
        {
            this.loadingThrobber.Enabled = false;
            this.loadingThrobber.SendToBack();
        }

        public bool ScrollBy(int value)
        {
            // In theory we could use webbrowser.Document.* to access the innerHeight or the offsetHeight
            // But in reality since it returns the non dpi aware height, thus we need to call javascript. 

            var height = webBrowser1.Document.InvokeScript("eval", new object[] { "\"innerHeight\" in window ? window.innerHeight : document.documentElement.offsetHeight" });
            if (height == null)
                return false;

            var scroll = webBrowser1.Document.Body.ScrollRectangle.Height - (int)height;

            // if scroll is negative everything fits perfectly...
            if (scroll < 0)
                scroll = 0;

            if (y >= scroll)
                return false;

            // we use here javascript's scrollBy, because it scroll much smoother, than calling scrollTo from c#
            webBrowser1.Document.InvokeScript("eval", new object[] { "window.scrollBy(0,"+value+")" });

            y += value;

            return true;
        }

        public bool IsReady()
        {
            // wait until the webbrowser is ready. Otherwise we run into 
            // exceptions cause by a partial loaded website.
            if (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                return false;

            return true;
        }

        public void Navigate(string url)
        {
            y = 0;
            webBrowser1.Navigate(url);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            highDpiUiHandler.Inject();
            this.HideThrobber();
        }
    }


}
