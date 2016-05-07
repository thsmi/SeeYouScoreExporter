using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace net.tschmid.scooring.converter
{
    public class TemplateExporter
    {
        private List<Template> templates;

        public TemplateExporter(List<Template> templates)
        {
            this.templates = templates;
        }


        public void Export(Contest contest)
        {

            foreach (Clazz clazz in contest.GetClazzes())
            {
                foreach (Task task in clazz.GetTasks())
                {
                    foreach (Template template in templates)
                    {
                        template.Export(Converter.Properties.Settings.Default.DirectoriesOutput, contest, clazz, task);
                    }
                }
            }

        }
    }

    public class Template
    {
        private string filename;

        public Template(string filename)
        {
            this.filename = filename;
        }

        public void Export(string workDir, Contest contest, Clazz clazz, Task task)
        {

            string content = File.ReadAllText(Converter.Properties.Settings.Default.DirectoriesTemplates + filename + ".tpl");
            task.GetClazz();

            content = fillTemplate(content, contest, clazz, task);
            System.IO.File.WriteAllText(workDir + task.Date + "_" + clazz.Type + "_" + filename, content);
        }
        
        private string fillResults(string template, Task task)
        {
            const string SCORE_REG_EX = "(<Score:Results.*?>)(.*?)(</Score:Results>)";
            //_htmlContent = Regex.Replace(_htmlContent, "< style.*?< /style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            MatchCollection matches = Regex.Matches(template, SCORE_REG_EX, RegexOptions.Singleline);

            if (matches.Count == 0)
                return template;

            foreach (Match match in matches) { 

                List<Result> results = task.GetResults();

                string resultTpl = "";
                string resultStr = "";
                string tmp = match.Groups[1].Value;

                string sort = Regex.Match(tmp, @"(?<=\bsort="")[^""]*").Value;

                if ("Rank".Equals(sort))
                    results = results.OrderBy(o => o.Rank).ToList();

                if ("RankTotal".Equals(sort))
                    results = results.OrderBy(o => o.RankTotal).ToList();

                string when = Regex.Match(tmp, @"(?<=\bwhen="")[^""]*").Value;

                foreach (Result result in results)
                {
                    if ("Comment".Equals(when) && String.IsNullOrWhiteSpace(result.Comment)) 
                        continue;

                    resultTpl = match.Groups[2].Value;
                    resultTpl = Regex.Replace(resultTpl, "<Result:RankTotal.*?/>", "" + result.RankTotal);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Rank.*?/>", "" + result.Rank);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Cn.*?/>", result.ContestantNumber);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Name.*?/>", result.Name);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Club.*?/>", result.Club);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Aircraft.*?/>", result.Aircraft);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Start.*?/>", result.ScoredStart);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Finish.*?/>", result.ScoredFinish);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Speed.*?/>", "" + result.ScoredSpeed);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Distance.*?/>", "" + result.ScoredDistance);
                    resultTpl = Regex.Replace(resultTpl, "<Result:PointsTotal.*?/>", "" + result.PointsTotal);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Points.*?/>", "" + result.Points);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Penalty.*?/>", "" + result.Penalty);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Evaluated.*?/>", "" + result.Evaluated);
                    resultTpl = Regex.Replace(resultTpl, "<Result:Comment.*?/>", "" + result.Comment);


                    resultStr += resultTpl;
                }
                Regex regex = new Regex(SCORE_REG_EX, RegexOptions.Singleline);

                template = regex.Replace(template, resultStr, 1);
            }

            return template;
        }

        private string fillTemplate(string template, Contest contest, Clazz clazz, Task task)
        {

            template = Regex.Replace(template, "<Contest:Name.*?/>", contest.Name);
            template = Regex.Replace(template, "<Contest:StartDate.*?/>", contest.StartDate);
            template = Regex.Replace(template, "<Contest:EndDate.*?/>", contest.EndDate);


            template = Regex.Replace(template, "<Task:Class.*?/>", clazz.Name);
            template = Regex.Replace(template, "<Task:Date.*?/>", task.Date);
            template = Regex.Replace(template, "<Task:Name.*?/>", task.Name);

            template = Regex.Replace(template, "<General:DateTime.*?/>", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            template = fillImages(template, task);
            template = fillResults(template, task);
            template = fillPoints(template, task);

            return template;

        }

        private string fillPoints(string template, Task task)
        {

            const string SCORE_REG_EX = "(<Task:Points.*?>)(.*?)(</Task:Points>)";

            MatchCollection matches = Regex.Matches(template, SCORE_REG_EX, RegexOptions.Singleline);

            if (matches.Count == 0)
                return template;

            foreach (Match match in matches)
            {
                string pointTpl = "";
                string pointStr = "";

                foreach (Point point in task.GetPoints())
                {
                    pointTpl = match.Groups[2].Value;
                    pointTpl = Regex.Replace(pointTpl, "<Point:Name.*?/>", "" + point.Name);
                    pointTpl = Regex.Replace(pointTpl, "<Point:Index.*?/>", "" + point.Index);
                    pointTpl = Regex.Replace(pointTpl, "<Point:Distance.*?/>", "" + point.Distance);

                    pointTpl = Regex.Replace(pointTpl, "<Point:Latitude.*?/>", "" + point.Latitude);
                    pointTpl = Regex.Replace(pointTpl, "<Point:Longitude.*?/>", "" + point.Longitude);

                    pointTpl = Regex.Replace(pointTpl, "<Point:Type.*?/>", "" + point.Type);
                    pointTpl = Regex.Replace(pointTpl, "<Point:Elevation.*?/>", "" + point.Elevation);

                    pointTpl = Regex.Replace(pointTpl, "<Point:CourseIn.*?/>", "" + point.CourseIn);
                    pointTpl = Regex.Replace(pointTpl, "<Point:CourseOut.*?/>", "" + point.CourseOut);

                    pointTpl = Regex.Replace(pointTpl, "<Point:OzAngle1.*?/>", "" + point.OzAngle1);
                    pointTpl = Regex.Replace(pointTpl, "<Point:OzAngle2.*?/>", "" + point.OzAngle2);
                    pointTpl = Regex.Replace(pointTpl, "<Point:OzAngle12.*?/>", "" + point.OzAngle12);
                    
                    pointTpl = Regex.Replace(pointTpl, "<Point:OzRadius1.*?/>", "" + point.OzRadius1);
                    pointTpl = Regex.Replace(pointTpl, "<Point:OzRadius2.*?/>", "" + point.OzRadius2);

                    pointStr += pointTpl;
                }

                Regex regex = new Regex(SCORE_REG_EX, RegexOptions.Singleline);
                template = regex.Replace(template, pointStr, 1);
            }
   
            return template;
        }


        private string fillImages(string template, Task task)
        {


            MatchCollection matches = Regex.Matches(template, "(<Task:Images.*?>)(.*)(<\\/Task:Images>)", RegexOptions.Singleline);


            if (matches.Count == 0)
                return template;

            if (matches.Count > 1)
            {
                Console.WriteLine("More than one Task:images Element");
                return template;
            }

            string resultTpl = "";
            string resultStr = "";
            string tmp = matches[0].Groups[1].Value;

            foreach (Image image in task.GetImages())
            {
                image.Save("d:\\");

                resultTpl = matches[0].Groups[2].Value;
                resultTpl = Regex.Replace(resultTpl, "<Image:Src.*?/>", "" + image.Name);

                resultStr += resultTpl;
            }

            return Regex.Replace(template, "(<Task:Images.*?>)(.*)(<\\/Task:Images>)", resultStr, RegexOptions.Singleline);
        }
    }
}
