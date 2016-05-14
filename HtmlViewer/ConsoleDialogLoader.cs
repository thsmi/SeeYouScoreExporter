using System;
using System.Threading;
using System.Windows.Forms;
using static net.tschmid.scooring.htmlviewer.NativeMethods;

namespace net.tschmid.scooring.htmlviewer
{
    public class HtmlViewerConsoleWrapper 
    {

        public void Show()
        {
            // When opening a dialog from a console we endup on the primary screen
            // Instead we want the slideshow to be opened on the screen where the console is located.
            IntPtr h2 = GetConsoleWindow();
            RECT rect = new RECT();
            GetWindowRect(h2, ref rect);

            HtmlViewer viewer = new HtmlViewer();

            viewer.ShowInTaskbar = false;
            viewer.Show();

            // The window location can only be changed after the dialog is shown...
            viewer.Location = new System.Drawing.Point(rect.left, rect.top);
            viewer.WindowState = FormWindowState.Maximized;

            Cursor.Hide();

            viewer.Activate();
            SetForegroundWindow(viewer.Handle);

            while (!viewer.IsDisposed)
            {
                Application.DoEvents();
                Thread.Sleep(20);
            }

        }

    }


}
