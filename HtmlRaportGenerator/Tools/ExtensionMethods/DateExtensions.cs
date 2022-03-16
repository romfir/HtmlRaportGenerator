using System;
using System.Collections.Generic;
using System.Globalization;

namespace HtmlRaportGenerator.Tools.ExtensionMethods;

public static class DateExtensions
{
    public static string ToYearMonth(this DateOnly date)
        => date.ToString(StaticHelpers.YearMonthFormat, DateTimeFormatInfo.InvariantInfo);

    public static IEnumerable<DateTime> AllDatesInMonth(this DateTime month)
    {
        int days = DateTime.DaysInMonth(month.Year, month.Month);
        for (int day = 1; day <= days; day++)
        {
            yield return new DateTime(month.Year, month.Month, day);
        }
    }

    public static string ToYearMonth(this DateTime date)
        => date.ToString(StaticHelpers.YearMonthFormat, DateTimeFormatInfo.InvariantInfo);
}