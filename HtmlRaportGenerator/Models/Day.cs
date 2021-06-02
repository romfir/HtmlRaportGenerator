using AutoMapper;
using HtmlRaportGenerator.Tools.Mapper;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HtmlRaportGenerator.Models
{
    public class Day : INotifyPropertyChanged, IMapFrom<Day>, IEquatable<Day>
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Day()
        {
        }

        public Day(int dayNumber)
            => DayNumber = dayNumber;

        public Day(int dayNumber, bool isHoliday) : this(dayNumber)
            => IsHoliday = isHoliday;

        public int DayNumber { get; set; }

        public HourWithQuarter From { get; set; } = new();

        [Range(0, 23.99), JsonIgnore]
        public double? HourWithQuarterFromParsed
        {
            get => From?.GetHourWithQuarterSum();
            set
            {
                From = new HourWithQuarter(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(From)));
            }
        }

        public HourWithQuarter To { get; set; } = new();

        [Range(0, 23.99), JsonIgnore]
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

                if (To?.Hour is not null)
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
            => From?.Hour is not null || To?.Hour is not null;

        public bool Equals(Day? other)
        {
            if (other is null)
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object? obj)
            => Equals(obj as Day);

        public override int GetHashCode()
            => HashCode.Combine(DayNumber, To, From);

        public void Mapping(Profile profile)
            => profile.CreateMap<Day, Day>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(dest => dest.IsToday, opt => opt.Ignore())
                .ForMember(dest => dest.HourWithQuarterFromParsed, opt => opt.Ignore())
                .ForMember(dest => dest.HourWithQuarterToParsed, opt => opt.Ignore());
    }
}
