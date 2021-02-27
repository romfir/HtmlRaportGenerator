using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HtmlRaportGenerator.Models
{
    public class HourWithQuarter : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
