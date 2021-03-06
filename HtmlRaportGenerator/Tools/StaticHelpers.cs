using System;
using System.Collections.Generic;

namespace HtmlRaportGenerator.Tools
{
    public static class StaticHelpers
    {
        public static IEnumerable<DateTime> AllDatesInMonth(this DateTime month)
        {
            int days = DateTime.DaysInMonth(month.Year, month.Month);
            for (int day = 1; day <= days; day++)
            {
                yield return new DateTime(month.Year, month.Month, day);
            }
        }

        public const string YearMonthFormat = "yyyy-MM";

        public const string InputKey = "input";

        public const string DataStoreTypeKey = "DataStore";

        public const string HttpClientName = "GoogleAuthClient";

        public const string ProjectName = "Raport Generator";

        public static string FormatDoubleToTime(this double hour)
            => TimeSpan.FromHours(hour).ToString(@"hh\:mm");
    }
}
