﻿using System;
using System.Collections.Generic;

namespace HtmlRaportGenerator.Shared
{
    public static class StaticHelpers
    {
        //public static IEnumerable<DateTime> AllDatesInMonth(int year, int month)
        public static IEnumerable<DateTime> AllDatesInMonth(this DateTime month)
        {
            int days = DateTime.DaysInMonth(month.Year, month.Month);
            for (int day = 1; day <= days; day++)
            {
                yield return new DateTime(month.Year, month.Month, day);
            }
        }
    }
}
