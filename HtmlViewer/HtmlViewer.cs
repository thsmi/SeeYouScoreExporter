using net.tschmid.scooring.utils;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace net.tschmid.scooring.htmlviewer
{


    public partial class HtmlViewer : Form
    {

        private int idx;
        private int postScrollTimer;
        private int preScrollTimer;

        public HtmlViewer()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // We skip in case the document is currently loading.
            if (!highDpiWebViewer.IsReady())
                return;

            // There is a pre scrolling timeout...
            if (preScrollTimer > 0)
            {
                preScrollTimer--;
                return;
            }

            // Then scoll untill we reach the end.
            if (highDpiWebViewer.ScrollBy(1))
                return;

            // finally the post scroll timeout
            if (postScrollTimer > 0)
            {
                postScrollTimer--;
                return;
            }

            postScrollTimer = Properties.Settings.Default.SlideshowPostScrollDelay * 10;
            preScrollTimer = Properties.Settings.Default.SlideshowPreScrollDelay * 10;

            LoadNextFile();
        }

        private void LoadNextFile()
        {
            string[] files = { };

            if (Properties.Settings.Default.SlideshowTemplates == null)
                return;

            string workdir = Properties.Settings.Default.SlideshowWorkDirectory;
            string day = (new DateUtils()).Expand(Properties.Settings.Default.SlideshowFilter);

            foreach (string template in Properties.Settings.Default.SlideshowTemplates)
            {
                files = files.Union(Directory.GetFiles(workdir, day + "*_" + template)).ToArray();
            }

            if (files.Length == 0)
            {
                highDpiWebViewer.ShowThrobber();
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " Slideshow] No files in directory " + workdir);
                return;
            }

            if (files.Length <= idx)
                idx = 0;

            string url = Path.GetFullPath(files[idx]);

            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " Slideshow] Loading file "+url);

            highDpiWebViewer.Navigate(url);

            idx++;

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.timer1_Tick(sender, e);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
                return;
            }


            if ((e.KeyCode) == Keys.F4 && (e.Alt))
            {
                Close();
                return;
            }

            if ((e.KeyCode) == Keys.C && (e.Control)) 
            {
                Close();
                return;
            }
        }
        
    }

}
