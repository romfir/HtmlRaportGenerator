using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlRaportGenerator.Tools
{
    public static class StaticHelpers
    {

        public const string YearMonthFormat = "yyyy-MM";

        public const string DataStoreTypeKey = "DataStore";

        public const string HttpClientName = "GoogleAuthClient";

        public const string ProjectName = "Raport Generator";

        public static string FormatDoubleToTime(this double hour)
            => TimeSpan.FromHours(hour).ToString(@"hh\:mm");

        /// <summary>
        /// use only when comparing collections of the same type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static int GetCollectionHashCode<T>(this ICollection<T>? collection)
        {
            if (collection is null)
            {
                return 0;
            }

            return collection.Aggregate(0, (current, item) => current ^ (item?.GetHashCode() ?? 0));
        }
    }
}
