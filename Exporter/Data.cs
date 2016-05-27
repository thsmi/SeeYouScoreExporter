using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace net.tschmid.scooring.exporter
{
    public class Task
    {
        private List<Result> results = null;
        private List<Image> images = new List<Image>();
        private List<Point> points;
        private Clazz clazz = null;

        public Task(Dictionary<String, dynamic> json)
        {
            DataConverter converter = new DataConverter(json);

            Date = json["task_date"];
            Number = json["task_number"];
            Status = json["result_status"];

            QNH = converter.GetString("qnh");
           
            Type = json["task_type"];
            Value = json["task_value"];

            Name = converter.GetString("task_name");
            Distance = converter.GetDistance("task_distance");
            DistanceMin = converter.GetDistance("task_distance_min");
            DistanceMax = converter.GetDistance("task_distance_max");
            Duration = json["task_duration"];
            NoStart = json["no_start"];
            StartOnEntry = json["start_on_entry"];

            ResultsLink = json["_links"]["http://api.soaringspot.com/rel/results"]["href"];

            dynamic images = json["_embedded"]["http://api.soaringspot.com/rel/images"];
            foreach (Dictionary<String, dynamic> image in images)
            {
                this.images.Add(new Image(image));
            }

            PointsLink = json["_links"]["http://api.soaringspot.com/rel/points"]["href"];

            ClassLink = json["_links"]["http://api.soaringspot.com/rel/class"]["href"];
        }

        public string Date { get; private set; }
        public int Number { get; private set; }
        public dynamic Type { get; private set; }
        public dynamic ResultStatus { get; private set; }
        public dynamic Status { get; private set; }
        public dynamic QNH { get; private set; }
        public dynamic Value { get; private set; }
        public dynamic Name { get; private set; }
        public string Distance { get; private set; }
        public string DistanceMin { get; private set; }
        public string DistanceMax { get; private set; }
        public dynamic Duration { get; private set; }
        public dynamic NoStart { get; private set; }
        public dynamic StartOnEntry { get; private set; }

        public string ResultsLink { get; private set; }
        public string PointsLink { get; private set; }
        public string ClassLink { get; private set; }

        public List<Image> GetImages()
        {
            return this.images;
        }

        public List<Point> GetPoints()
        {
            if (this.points != null)
                return this.points;

            Dictionary<string, dynamic> data = (new Loader(PointsLink)).Load();

            if (data.Count == 0)
                return new List<Point>();

            this.points = new List<Point>();


            dynamic items = data["_embedded"]["http://api.soaringspot.com/rel/points"];

            foreach (Dictionary<string, dynamic> item in items)
            {
                this.points.Add(new Point(item));
            }

            return points;
        }

        public Clazz GetClazz() {

            if (this.clazz != null)
                return this.clazz;

            Dictionary<string, dynamic> data = (new Loader(this.ClassLink)).Load();

            this.clazz = new Clazz(data);

            return this.clazz;
        }

        public List<Result> GetResults()
        {
            if (this.results != null)
                return results;

            this.results = new List<Result>();

            Dictionary<string, dynamic> data = (new Loader(this.ResultsLink)).Load();

            dynamic items = data["_embedded"]["http://api.soaringspot.com/rel/results"];

            foreach (Dictionary<String, dynamic> item in items)
            {
                this.results.Add(new Result(item));
            }

            return this.results;
        }

    }

    public class Image
    {
        private Dictionary<string, dynamic> image;

        public Image(Dictionary<string, dynamic> image)
        {
            this.image = image;

            this.Name = image["name"];
            this.ImageLink = image["_links"]["self"]["href"];
        }

        public dynamic ImageLink { get; private set; }
        public dynamic Name { get; private set; }

        public Stream GetImage()
        {
            return (new Loader(this.ImageLink)).LoadRaw();
        }

        public void Save(string workdir)
        {
            string file = Path.Combine(workdir, this.Name);

            if (File.Exists(file))   
                return;
            
            Stream input = this.GetImage();
            
            Stream output = File.Create(file);

            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            };

            output.Flush();
            output.Close();
        }
    }

    public class Result
    {
        public Result(Dictionary<string, dynamic> result)
        {
            DataConverter converter = new DataConverter(result);

            Points = result["points"];
            PointsTotal = result["points_total"];
            Rank = result["rank"];
            RankTotal = result["rank_total"];
            Penalty = result["penalty"];

            Comment = converter.GetString("comment");

            ScoredDistance = converter.GetDistance("scored_distance");
            ScoredSpeed = converter.GetSpeed("scored_speed");
            ScoredStart = converter.GetTime("scored_start");
            ScoredFinish = converter.GetTime("scored_finish");

            IgcFile = converter.GetString("igc_file");
            Evaluated = result["status_evaluated"];

            dynamic contestant = result["_embedded"]["http://api.soaringspot.com/rel/contestant"];

            converter = new DataConverter(contestant);
            Name =  contestant["name"];
            Club = contestant["club"];
            Aircraft = contestant["aircraft_model"];
            ContestantNumber = contestant["contestant_number"];
            AircraftRegistration = contestant["aircraft_registration"];
            Glider = contestant["pure_glider"];
            Handicap = contestant["handicap"];
            NotCompeting = contestant["not_competing"];
            FlightRecorders = converter.GetString("flight_recorders");

            dynamic pilots = contestant["_embedded"]["http://api.soaringspot.com/rel/pilot"];

            foreach (Dictionary<String, dynamic> pilot in pilots)
            {
                //new Pilot(pilot);

                //Console.WriteLine(pilot["first_name"]);
                //Console.WriteLine(pilot["last_name"]);
                //Console.WriteLine(pilot["email"]);
                //Console.WriteLine(pilot["nationality"]);
                //Console.WriteLine(pilot["igc_id"]);

            }
        }

        public string Aircraft { get; private set; }
        public string AircraftRegistration { get; private set; }
        public string Club { get; private set; }
        public string Comment { get; private set; }
        public string ContestantNumber { get; private set; }
        public dynamic Evaluated { get; private set; }
        public string FlightRecorders { get; private set; }
        public bool Glider { get; private set; }
        public int Handicap { get; private set; }
        public string IgcFile { get; private set; }
        public string Name { get; private set; }
        public bool NotCompeting { get; private set; }
        public int Penalty { get; private set; }
        public int Points { get; private set; }
        public int PointsTotal { get; private set; }
        public int Rank { get; private set; }
        public int RankTotal { get; private set; }
        public string ScoredDistance { get; private set; }
        public string ScoredFinish { get; private set; }
        public string ScoredSpeed { get; private set; }
        public string ScoredStart { get; private set; }
        
    }

    public class Contest
    {
        private List<Clazz> clazzes = new List<Clazz>();

        public int Id { get; private set; }
        public String Name { get; private set; }
        public String StartDate { get; private set; }
        public String EndDate { get; private set; }
        public bool Featured { get; private set; }
        public String TimeZone { get; private set; }
        public String Country { get; private set; }

        public Contest(dynamic data)
        {
            Id = data["id"];
            Name = data["name"];
            StartDate = data["start_date"];
            EndDate = data["end_date"];
            Featured = data["featured"];
            TimeZone = data["time_zone"];
            Country = data["country"];

            var items = data["_embedded"]["http://api.soaringspot.com/rel/classes"];

            foreach (Dictionary<string, dynamic> item in items)
            {
                this.clazzes.Add(new Clazz(item));
            }
        }

        public List<Clazz> GetClazzes()
        {
            return this.clazzes;
        }

        public static Contest LoadContest(string contest)
        {
            string url = "http://api.soaringspot.com/v1/contests/" + contest;
            dynamic data = (new Loader(url)).Load();

            return new Contest(data);
        }

        public static List<Contest> LoadContests()
        {
            Dictionary<string, dynamic> data = (new Loader("http://api.soaringspot.com/v1/")).Load();

            List<Contest> result = new List<Contest>();

            if (!data.ContainsKey("_embedded") || !data["_embedded"].ContainsKey("http://api.soaringspot.com/rel/contests"))
                return result;

            dynamic contests = data["_embedded"]["http://api.soaringspot.com/rel/contests"];

            foreach (Dictionary<string, object> contest in contests)
            {
                result.Add(new Contest(contest));
            }

            return result;

        }
    }

    public class DataConverter
    {
        private Dictionary<string, dynamic> data;

        public DataConverter(Dictionary<string, dynamic> data)
        {
            this.data = data;
        }

        public string GetCoordinate(string key)
        {
            if (!data.ContainsKey(key))
                return "";

            decimal coord = data[key];

            
            coord = decimal.Multiply(coord, (decimal)(180.0 / Math.PI));

            int seconds = ((int)(decimal.Multiply(coord, 3600)) % 60);
            int minute = ((int)(decimal.Multiply(coord, 60)) % 60);
            int degree = (((int)(coord)) % 60);

            return "" + degree.ToString("0") + "° " + minute.ToString("00") + "' " + seconds.ToString("00") + "''";
        }

        public string GetString(string key)
        {
            if (!data.ContainsKey(key))
                return "";

            return ""+data[key];
        }

        public string GetDistance(string key)
        {
            if (!data.ContainsKey(key))
                return "";

            decimal distance = data[key];

            if (distance == 0)
                return "";

            distance = distance / new Decimal(1000);

            return distance.ToString("0.00");
        }

        internal string GetSpeed(string key)
        {
            if (!data.ContainsKey(key))
                return "";
            
            decimal speed = data[key];

            speed = speed * new Decimal(3.6);

            if (speed == 0)
                return "";

            return speed.ToString("0.00");
        }

        internal string GetTime(string key)
        {
            if (!data.ContainsKey(key))
                return "";

            if (String.IsNullOrWhiteSpace(data[key]))
                return "";

            DateTime date = DateTime.ParseExact(data[key], "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

            return date.ToString("HH:mm:ss");
        }

        public string GetDegree(string key)
        {
            if (!data.ContainsKey(key))
                return "";

            
            decimal degree = data[key];

            degree = decimal.Multiply(degree, (decimal)(180.0 / Math.PI));

            return degree.ToString("0.00°");
        }

        public string GetBoolean(string key)
        {
            if (!data.ContainsKey(key))
                return "";

            return "" + data[key];
        }
    }

    public class Point
    {

        public Point(System.Collections.Generic.Dictionary<string, dynamic> data)
        {

            DataConverter converter = new DataConverter(data); 

            Name = converter.GetString("name");
            Latitude = converter.GetCoordinate("latitude");
            Longitude = converter.GetCoordinate("longitude");


            Type = converter.GetString("type");
            Index = converter.GetString("point_index");

            Elevation = converter.GetString("elevation");

            Distance = converter.GetDistance("distance");

            CourseIn = converter.GetDegree("course_in");
            CourseOut = converter.GetDegree("course_out");

            OzAngle1 = converter.GetDegree("oz_angle1");
            OzAngle2 = converter.GetDegree("oz_angle2");
            OzAngle12 = converter.GetDegree("oz_angle12");

            OzRadius1 = converter.GetDistance("oz_radius1");
            OzRadius2 = converter.GetDistance("oz_radius2");

            OzType = converter.GetString("oz_type");
            OzMaxAltitude = converter.GetString("oz_max_altitude");
            OzMove = converter.GetBoolean("oz_move");
            OzLine = converter.GetBoolean("oz_line");
            OzReduce = converter.GetBoolean("oz_reduce");

            MultipleStart = converter.GetBoolean("multiple_start");
        }

        public object CourseIn { get; private set; }
        public object CourseOut { get; private set; }
        public string Distance { get; private set; }
        public string Elevation { get; private set; }
        public string Index { get; private set; }
        public string Latitude { get; private set; }
        public string Longitude { get; private set; }
        public dynamic MultipleStart { get; private set; }
        public string Name { get; private set; }
        public object OzAngle1 { get; private set; }
        public object OzAngle12 { get; private set; }
        public object OzAngle2 { get; private set; }
        public string OzLine { get; private set; }
        public string OzMaxAltitude { get; private set; }
        public string OzMove { get; private set; }
        public string OzRadius1 { get; private set; }
        public string OzRadius2 { get; private set; }
        public string OzReduce { get; private set; }
        public string OzType { get; private set; }
        public string Type { get; private set; }
    }

    public class Clazz
    {
        private List<Task> tasks;

        public Clazz(Dictionary<string, dynamic> json)
        {
            Id = json["id"];
            Category = json["category"];
            Type = json["type"];
            Name = json["name"];

            TaskLink = json["_links"]["http://api.soaringspot.com/rel/tasks"]["href"];

        }

        public int Id { get; private set; }
        public String Category { get; private set; }
        public String Name { get; private set; }
        public String TaskLink { get; private set; }
        public String Type { get; private set; }

        public List<Task> GetTasks()
        {
            if (this.tasks != null)
                return this.tasks;

            this.tasks = new List<Task>();

            Dictionary<string, dynamic> dict = (new Loader(TaskLink)).Load();

            dynamic items = dict["_embedded"]["http://api.soaringspot.com/rel/tasks"];

            foreach (Dictionary<string, dynamic> item in items)
            {
                this.tasks.Add(new Task(item));
            }

            return this.tasks;
        }
    }

}
