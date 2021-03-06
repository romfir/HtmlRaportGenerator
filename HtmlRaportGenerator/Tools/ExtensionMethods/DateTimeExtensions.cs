using System;

namespace HtmlRaportGenerator.Tools.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        public static string ToYearMonth(this DateTime date)
            => date.ToString(StaticHelpers.YearMonthFormat);
    }
}
