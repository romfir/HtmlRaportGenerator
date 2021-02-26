using System;

namespace HtmlRaportGenerator.Models
{
    public class HourWithQuarter
    {
        public HourWithQuarter()
        {
        }

        public HourWithQuarter(double? time)
        {
            if (time is object)
            {
                Hour = (int)time.Value;

                int quarer = (int)Math.Floor((time.Value - Hour.Value) * 4);

                if (quarer != 0)
                {
                    Quarter = quarer;
                }
            }
        }

        public HourWithQuarter(DateTime date)
        {
            Hour = date.Hour;

            Quarter = date.Minute / 15; //todo check Leap second?
        }

        public int? Hour { get; set; }
        public int? Quarter { get; set; }

        public double? GetHourWithQuarterSum() //add GetHourWithQuarterSumFormatted/ StaticTools.FormatDoubleAstime(dt) which turns 12.75 into 12:45
        {
            if (Hour is null)
            {
                return null;
            }

            double from = Hour.Value;

            if (Quarter is object)
            {
                from += Quarter.Value * 0.25;
            }

            return from;
        }
    }
}
