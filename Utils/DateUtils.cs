using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.tschmid.scooring.utils
{
    public class DateUtils
    {
        public string Expand(string date)
        {
            if ("all".Equals(date))
                return "";

            if (string.IsNullOrWhiteSpace(date))
                return "";

            if ("today".Equals(date))
                return DateTime.Now.ToString("yyyy-MM-dd");

            if ("yesterday".Equals(date))
                return DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            return date;
        }

        public bool Matches(string filter, string date)
        {
            filter = this.Expand(filter);

            if (string.IsNullOrWhiteSpace(filter))
                return true;

            if (filter.Equals(date))
                return true;

            return false;
        }

    }
}
