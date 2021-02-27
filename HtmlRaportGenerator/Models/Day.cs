using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HtmlRaportGenerator.Models
{
    public class Day : INotifyPropertyChanged
    {
        public Day()
        {
        }

        public Day(int dayNumber)
            => DayNumber = dayNumber;

        public Day(int dayNumber, bool isHoliday) : this(dayNumber)
            => IsHoliday = isHoliday;

        public int DayNumber { get; set; }

        public HourWithQuarter From { get; set; } = null!;

        [Range(0, 23.59), JsonIgnore]
        public double? HourWithQuarterFromParsed
        {
            get => From?.GetHourWithQuarterSum();
            set
            {
                From = new HourWithQuarter(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(From)));
            }
        }

        public HourWithQuarter To { get; set; } = null!;

        [Range(0, 23.59), JsonIgnore]
        public double? HourWithQuarterToParsed
        {
            get => To?.GetHourWithQuarterSum();
            set
            {
                To = new HourWithQuarter(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HourWithQuarterToParsed)));
            }
        }

        [JsonIgnore]
        public bool IsToday { get; set; }

        public bool IsHoliday { get; set; }

        [JsonIgnore]
        public double? HourSum
        {
            get
            {
                if (From?.Hour is null || (To?.Hour is null && !IsToday))
                {
                    return null;
                }

                double from = From.GetHourWithQuarterSum()!.Value;

                double to;

                if (To?.Hour is object)
                {
                    to = To.GetHourWithQuarterSum()!.Value;
                }
                else //IsToday == true
                {
                    to = new HourWithQuarter(DateTime.Now).GetHourWithQuarterSum()!.Value;
                }

                //when shift starts on one day and ends on the other
                if (to < from)
                {
                    to += 24;
                }

                return to - from;
            }
        }

        public bool AnyRelevantValueExists()
            => From?.Hour is object || To?.Hour is object;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
