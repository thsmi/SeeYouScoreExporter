using System;
using System.Collections.Generic;
using System.IO;

using System.Threading;
using System.Collections.Specialized;
using net.tschmid.scooring.exporter;
using net.tschmid.scooring.htmlviewer;

namespace net.tschmid.scooring
{
    class Program
    {
        
        static void Main(string[] args)
        {

            if ((args.Length >=2) && (args[0].Equals("--export"))) {

                if (args[1].Equals("enabled"))
                {
                    DoExportEnabledSettings(args);
                    return;
                }

                if ( args[1].Equals("credentials"))
                {
                    DoCredentialSettings(args);
                    return;
                }

                if ( args[1].Equals("directories"))
                {
                    DoDirectorySettings(args);
                    return;
                }

                if ( args[1].Equals("templates"))
                {
                    DoExportTemplateSettings(args);
                    return;
                }

                if (args[1].Equals("day"))
                {
                    DoExportDayFilterSettings(args);
                    return;
                } 

                if ( args[1].Equals("contest"))
                {
                    DoContestSettings(args);
                    return;
                }

                if (args[1].Equals("refresh"))
                {
                    DoWaitSettings(args);
                    return;
                }

                if (args[1].Equals("debug"))
                {
                    DoExportDebugSettings(args);
                    return;
                }

            }

            if ((args.Length >= 2) && (args[0].Equals("--slideshow")))
            {
                if (args[1].Equals("enabled"))
                {
                    DoSlideshowEnabledSettings(args);
                    return;
                }

                if (args[1].Equals("day"))
                {
                    DoSlideshowDayFilterSettings(args);
                    return;
                }

                if (args[1].Equals("templates"))
                {
                    DoSlideshowTemplatesSettings(args);
                    return;
                }

                if (args[1].Equals("duration"))
                {
                    DoSlideshowDurationSettings(args);
                    return;
                }

                if (args[1].Equals("directory"))
                {
                    DoSlideshowWorkDirectorySettings(args);
                    return;
                }

                if (args[1].Equals("debug"))
                {
                    DoSlideshowDebugSettings(args);
                    return;
                }
            }


            if (args.Length >= 1 && args[0].Equals("--run"))
            {
                DoRun(args);
                return;
            }

            Console.WriteLine();
            Console.WriteLine("  --export enabled <true|false>");
            Console.WriteLine("  --export credentials <clientId secret>");
            Console.WriteLine("  --export directories <templatedirectory workdirectory>");
            Console.WriteLine("  --export templates <templates...>");
            Console.WriteLine("  --export day <all | yyyy-mm-dd | today");
            Console.WriteLine("  --export refresh <updatecycle>");
            Console.WriteLine("  --export debug <true|false>");
            Console.WriteLine("  --export contest <id>");
            Console.WriteLine();
            Console.WriteLine("  --slideshow enabled <true|false>");
            Console.WriteLine("  --slideshow directory <workdirectory>");
            Console.WriteLine("  --slideshow day <all|yyyy-mm-dd|today>");
            Console.WriteLine("  --slideshow templates <templates>");
            Console.WriteLine("  --slideshow duration <pre> <post>");
            Console.WriteLine("    Specifies how long the hmtl page should stationary (before and after scrolling)");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(" --run ");
        }

        private static void DoExportDayFilterSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.ExportFilter = args[2];
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Export only day: " + Properties.Settings.Default.ExportFilter);
        }

        private static void DoSlideshowWorkDirectorySettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.SlideshowWorkDirectory = args[2];

                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Slideshow Work Directory set to " + Properties.Settings.Default.SlideshowWorkDirectory);
        }

        private static void DoExportEnabledSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.ExportEnabled = bool.Parse(args[2]);
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Result Export is " + (Properties.Settings.Default.ExportEnabled ? "enabled" : "disabled"));
        }

        private static void DoSlideshowEnabledSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.SlideshowEnabled = bool.Parse(args[2]);
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Slideshow is " + (Properties.Settings.Default.SlideshowEnabled ? "enabled":"disabled") );
        }

        private static void DoSlideshowDurationSettings(string[] args)
        {
            if (args.Length > 3)
            {
                Properties.Settings.Default.SlideshowPreScrollDelay = int.Parse(args[2]);
                Properties.Settings.Default.SlideshowPostScrollDelay = int.Parse(args[3]);
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Pre scroll delay set to " + Properties.Settings.Default.SlideshowPreScrollDelay+" seconds ");
            Console.WriteLine("Post scroll delay set to " + Properties.Settings.Default.SlideshowPostScrollDelay+" seconds ");
        }

        private static void DoSlideshowTemplatesSettings(string[] args)
        {
            // Silently repair broken settings...
            if (Properties.Settings.Default.SlideshowTemplates == null)
            {
                Properties.Settings.Default.SlideshowTemplates = new StringCollection();
                Properties.Settings.Default.Save();
            }

            if (args.Length > 2)
            {
                Properties.Settings.Default.SlideshowTemplates.Clear();

                for (int i = 2; i < args.Length; i++)
                {
                    Properties.Settings.Default.SlideshowTemplates.Add(args[i]);
                }

                Properties.Settings.Default.Save();
            }

            string templates = "";
            foreach (String template in Properties.Settings.Default.SlideshowTemplates)
            {
                templates += template + " ";
            }

            Console.WriteLine("Templates to show: " + templates);
        }

        private static void DoSlideshowDayFilterSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.SlideshowFilter = args[2];
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Show only day: " + Properties.Settings.Default.SlideshowFilter);
        }

        private static void DoSlideshowDebugSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.SlideshowDebug = Boolean.Parse(args[2]);
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Slideshow Debug Mode set to " + Properties.Settings.Default.SlideshowDebug);
        }

        private static void DoExportDebugSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.ExportDebug = Boolean.Parse(args[2] );
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Export Debug Mode set to " + Properties.Settings.Default.ExportDebug);
        }

        private static void DoWaitSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.ExportUpdateCycle = int.Parse(args[2]);
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Update cycle set to " + Properties.Settings.Default.ExportUpdateCycle+" minutes");
        }

        private static void DoCredentialSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.ExportCredentialsClientId = args[2];
                Properties.Settings.Default.ExportCredentialsSecret = args[3];

                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Client Id set to " + Properties.Settings.Default.ExportCredentialsClientId);
            Console.WriteLine("Secret set to " + Properties.Settings.Default.ExportCredentialsSecret);
        }

        private static void DoDirectorySettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.ExportTemplateDirectory = args[2];
                Properties.Settings.Default.ExportWorkDirectory = args[3];

                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Templates Directory set to " + Properties.Settings.Default.ExportTemplateDirectory);
            Console.WriteLine("Output Directory set to " + Properties.Settings.Default.ExportWorkDirectory);
        }

        private static void DoExportTemplateSettings(string[] args)
        {
            // Silently repair broken settings...
            if (Properties.Settings.Default.ExportTemplates == null) { 
                Properties.Settings.Default.ExportTemplates = new StringCollection();
                Properties.Settings.Default.Save();
            }

            if (args.Length > 2)
            {
                Properties.Settings.Default.ExportTemplates.Clear();

                for (int i = 2; i < args.Length; i++)
                {
                    Properties.Settings.Default.ExportTemplates.Add(args[i]);
                }

                Properties.Settings.Default.Save();
            }

            string templates = "";
            foreach (String template in Properties.Settings.Default.ExportTemplates)
            {
                templates += template + " ";
            }

            Console.WriteLine("Templates set to " + templates);
        }

        private static void DoContestSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.ExportContestId = args[2];
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Contest Id set to " + Properties.Settings.Default.ExportContestId);

            List<Contest> contests = Contest.LoadContests();

            Console.WriteLine("The following contests are associated to your account");
            Console.WriteLine();

            foreach (Contest contest in contests)
            {

                Console.WriteLine("  Id...... : " + contest.Id);
                Console.WriteLine("  Name.... : " + contest.Name);
                Console.WriteLine("  Start... : " + contest.StartDate);
                Console.WriteLine("  End .... : " + contest.EndDate);
                Console.WriteLine();
            }
        }


        private static void DoRun(string[] args)
        {
            List<Template> templates = new List<Template>();

            if (Properties.Settings.Default.ExportEnabled)
            {
                if (String.IsNullOrEmpty(Properties.Settings.Default.ExportCredentialsClientId) || String.IsNullOrEmpty(Properties.Settings.Default.ExportCredentialsSecret))
                {
                    Console.WriteLine(" No or invalid credentials");
                    return;
                }

                if (!Directory.Exists(Properties.Settings.Default.ExportWorkDirectory))
                {
                    Console.WriteLine(" Work directory " + Properties.Settings.Default.ExportWorkDirectory + " non existant");
                    return;
                }

                if (!Directory.Exists(Properties.Settings.Default.ExportTemplateDirectory))
                {
                    Console.WriteLine(" Templates directory " + Properties.Settings.Default.ExportTemplateDirectory + " non existant");
                    return;
                }

                if (String.IsNullOrEmpty(Properties.Settings.Default.ExportContestId))
                {
                    Console.WriteLine(" Invalid or no Contest Id");
                    return;
                }


                if ((Properties.Settings.Default.ExportTemplates == null) || Properties.Settings.Default.ExportTemplates.Count < 1)
                {
                    Console.WriteLine(" Error no templates configured, you need to set atleast one template");
                    return;
                }

                foreach (string template in Properties.Settings.Default.ExportTemplates)
                {
                    templates.Add(new Template(template));
                }
            }

            if (Properties.Settings.Default.SlideshowEnabled)
            {
                if (Properties.Settings.Default.SlideshowTemplates == null)
                {
                    Console.WriteLine(" Error no Slideshow Templates configured you need to set atleast one template");
                    return;
                }

                if (!Directory.Exists(Properties.Settings.Default.SlideshowWorkDirectory))
                {
                    Console.WriteLine(" Work directory " + Properties.Settings.Default.ExportWorkDirectory + " non existant");
                    return;
                }
            }

            SlideShowRunner slideshow = new SlideShowRunner();
            ExportRunner export = new ExportRunner(templates);

            while (true)
            {

                if (Properties.Settings.Default.ExportEnabled)
                    export.Run();

                if (Properties.Settings.Default.SlideshowEnabled)
                    slideshow.Run();

                //Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Ping");
                Thread.Sleep(1*60*1000);
            }

        }

        public class SlideShowRunner : AbstractRunner
        {

            protected override ApartmentState GetApartmentState()
            {
                return ApartmentState.STA;
            }

            protected override void OnStep()
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " Slideshow] Opening Slideshow Window");

                try
                {
                    (new HtmlViewerConsoleWrapper()).Show();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " Slideshow] Error while showing slideshow");
                    if (Properties.Settings.Default.SlideshowDebug)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("" + e.Message);
                        Console.WriteLine("" + e.StackTrace);

                        File.AppendAllText(".\\error.log", "" + e.Message);
                        File.AppendAllText(".\\error.log", "" + e.StackTrace);
                    }
                }

                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " Slideshow] Closing Slideshow Window");

                // Keep the thread for 30 more seconds alive so that 
                // the user has a chance to close the console window
                Thread.Sleep(30 * 1000);

                /* if (Environment.OSVersion.Version.Major >= 6)
                     SetProcessDPIAware();

                 Application.EnableVisualStyles();
                 Application.SetCompatibleTextRenderingDefault(false);
                 Application.Run(new Form1());*/
                //(new HtmlViewer()).ShowDialog();
            }
        }

        public class ExportRunner : AbstractRunner
        {
            private List<Template> templates;

            public ExportRunner(List<Template> templates)
            {
                this.templates = templates;
            }

            protected override void OnStep()
            {
                try
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " Export] Loading Data ...");
                    Contest contest = Contest.LoadContest(Properties.Settings.Default.ExportContestId);

                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " Export] Exporting Data ...");
                    (new TemplateExporter(templates)).Export(contest);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while fetching data");

                    if (Properties.Settings.Default.ExportDebug)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("" + e.Message);
                        Console.WriteLine("" + e.StackTrace);

                        File.AppendAllText(".\\error.log", "" + e.Message);
                        File.AppendAllText(".\\error.log", "" + e.StackTrace);
                    }
                }

                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " Export] Waiting " + Properties.Settings.Default.ExportUpdateCycle + " min ...");

                Thread.Sleep(Properties.Settings.Default.ExportUpdateCycle * 60);
            }
        }

        public abstract class AbstractRunner
        {
            private Thread thread = null;

            protected abstract void OnStep();

            protected virtual ApartmentState GetApartmentState()
            {
                return ApartmentState.Unknown; 
            }

            public void Run()
            {
                if (thread != null && thread.IsAlive)
                    return;

                thread = new Thread(() =>
                {
                    this.OnStep();
                });

                thread.SetApartmentState(GetApartmentState());
                thread.Start();
            }
        }

    }

}
