using HtmlRaportGenerator.Tools.Mapper;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HtmlRaportGenerator.Models
{
    public class HourWithQuarter : INotifyPropertyChanged, IMapFrom<HourWithQuarter>, IEquatable<HourWithQuarter>
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public HourWithQuarter()
        {
        }

        public HourWithQuarter(double? time)
        {
            if (time is object)
            {
                Hour = (int)time.Value;

                int quarer = (int)Math.Floor((time.Value - Hour.Value) * 4);

                Quarter = quarer;
            }
        }

        public HourWithQuarter(DateTime date)
        {
            Hour = date.Hour;

            Quarter = date.Minute / 15;
        }

        private int? _hour;

        [Required, Range(0, 23)]
        public int? Hour
        {
            get => _hour;
            set
            {
                _hour = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hour)));
            }
        }

        private int? _quarter;

        [Required, Range(0, 3)]
        public int? Quarter
        {
            get => _quarter;
            set
            {
                _quarter = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quarter)));
            }
        }

        public double? GetHourWithQuarterSum()
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

        public bool Equals(HourWithQuarter? other)
        {
            if (other is null)
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object? obj)
            => Equals(obj as HourWithQuarter);

        public override int GetHashCode()
            => HashCode.Combine(Hour, Quarter);

        public void Clear()
        {
            Quarter = null;
            Hour = null;
        }
    }
}
