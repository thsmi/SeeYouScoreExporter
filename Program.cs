using System;
using System.Collections.Generic;
using System.IO;

using net.tschmid.scooring.converter;
using System.Threading;
using System.Collections.Specialized;

namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {

            if ((args.Length >=2) && (args[0].Equals("--settings"))) {

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
                    DoTemplateSettings(args);
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
                    DoDebugSettings(args);
                    return;
                }

            }


            if (args.Length >= 1 && args[0].Equals("--run"))
            {
                DoRun(args);
                return;
            }

            Console.WriteLine();
            Console.WriteLine("  --settings credentials <clientId secret>");
            Console.WriteLine("  --settings directories <templatedirectory outputdirectory>");
            Console.WriteLine("  --settings templates <templates...>");
            Console.WriteLine("  --settings refresh <updatecycle>");
            Console.WriteLine("  --settings debug <true|false>");
            Console.WriteLine();
            Console.WriteLine("  --settings contest <id>");
            Console.WriteLine();
            Console.WriteLine(" --run ");
        }

        private static void DoDebugSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.Debug = Boolean.Parse(args[2] );
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Debug Mode set to " + Properties.Settings.Default.Debug);
        }

        private static void DoWaitSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.UpdateCycle = int.Parse(args[2]);
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Update cycle set to " + Properties.Settings.Default.UpdateCycle+" minutes");
        }

        private static void DoCredentialSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.CredentialsClientId = args[2];
                Properties.Settings.Default.CredentialsSecret = args[3];

                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Client Id set to " + Properties.Settings.Default.CredentialsClientId);
            Console.WriteLine("Secret set to " + Properties.Settings.Default.CredentialsSecret);
        }

        private static void DoDirectorySettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.DirectoriesTemplates = args[2];
                Properties.Settings.Default.DirectoriesOutput = args[3];

                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Templates Directory set to " + Properties.Settings.Default.DirectoriesTemplates);
            Console.WriteLine("Output Directory set to " + Properties.Settings.Default.DirectoriesOutput);
        }

        private static void DoTemplateSettings(string[] args)
        {
            if (args.Length > 2)
            {
                if (Properties.Settings.Default.Templates == null)
                    Properties.Settings.Default.Templates = new StringCollection();

                Properties.Settings.Default.Templates.Clear();

                for (int i = 2; i < args.Length; i++)
                {
                    Properties.Settings.Default.Templates.Add(args[i]);
                }

                Properties.Settings.Default.Save();
            }

            string templates = "";
            foreach (String template in Properties.Settings.Default.Templates)
            {
                templates += template + " ";
            }

            Console.WriteLine("Templates set to " + templates);
        }

        private static void DoContestSettings(string[] args)
        {
            if (args.Length > 2)
            {
                Properties.Settings.Default.ContestId = args[2];
                Properties.Settings.Default.Save();
            }

            Console.WriteLine("Contest Id set to " + Properties.Settings.Default.ContestId);

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
            if (String.IsNullOrEmpty(Properties.Settings.Default.CredentialsClientId) || String.IsNullOrEmpty(Properties.Settings.Default.CredentialsSecret))
            {
                Console.WriteLine(" No or invalid credentials");
                return;
            }

            if (!Directory.Exists(Properties.Settings.Default.DirectoriesTemplates))
            {
                Console.WriteLine(" Templates directory " + Properties.Settings.Default.DirectoriesTemplates + " non existant");
                return;
            }

            if (!Directory.Exists(Properties.Settings.Default.DirectoriesOutput))
            {
                Console.WriteLine(" Ouput directory " + Properties.Settings.Default.DirectoriesOutput + " non existant");
                return;
            }


            if (String.IsNullOrEmpty(Properties.Settings.Default.ContestId))
            {
                Console.WriteLine(" Invalid or no Contest Id");
                return;
            }

             
            if ((Properties.Settings.Default.Templates == null) || Properties.Settings.Default.Templates.Count < 1)
            {
                Console.WriteLine(" Error no templates configured, you need to set atleast one template");
                return;
            }

            List<Template> templates = new List<Template>();

            foreach (string template in Properties.Settings.Default.Templates)
            {
                templates.Add(new Template(template));
            }

            while (true)
            {
                try
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Loading Data ...");
                    Contest contest = Contest.LoadContest(Properties.Settings.Default.ContestId);

                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Exporting Data ...");
                    (new TemplateExporter(templates)).Export(contest);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while fetching data");

                    if (Properties.Settings.Default.Debug)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("" + e.Message);
                        Console.WriteLine("" + e.StackTrace);

                        File.AppendAllText(".\\error.log", ""+e.Message);
                        File.AppendAllText(".\\error.log", "" + e.StackTrace);
                    }
                }


                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Waiting ...");
                Thread.Sleep(Properties.Settings.Default.UpdateCycle * 60 * 1000);
            }

        }
    }

}
