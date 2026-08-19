using System;
using System.Globalization;

namespace SadrScales.Integration.Sales
{
    /// <summary>Common Sadr Scales sales-period presets.</summary>
    public enum SadrSalesPeriodPreset
    {
        /// <summary>The reference Gregorian date only.</summary>
        Today = 0,
        /// <summary>The Saturday-through-Friday week containing the reference date.</summary>
        CurrentWeek = 1,
        /// <summary>The Persian-calendar month containing the reference date.</summary>
        CurrentMonth = 2
    }

    /// <summary>Inclusive/exclusive date range used by sales queries and reports.</summary>
    public sealed class SadrSalesDateRange
    {
        /// <summary>Gets the inclusive start.</summary>
        public DateTime StartDateInclusive { get; internal set; }
        /// <summary>Gets the exclusive end.</summary>
        public DateTime EndDateExclusive { get; internal set; }
    }

    /// <summary>
    /// Reproduces Sadr Scales 5.2.1 Today, Saturday-based current-week and Persian current-month semantics.
    /// </summary>
    public static class SadrSalesPeriod
    {
        /// <summary>Calculates the selected period around the supplied local reference date.</summary>
        public static SadrSalesDateRange GetRange(SadrSalesPeriodPreset preset, DateTime referenceDate)
        {
            DateTime day = referenceDate.Date;

            switch (preset)
            {
                case SadrSalesPeriodPreset.CurrentWeek:
                    int daysSinceSaturday = ((int)day.DayOfWeek + 1) % 7;
                    DateTime weekStart = day.AddDays(-daysSinceSaturday);
                    return new SadrSalesDateRange
                    {
                        StartDateInclusive = weekStart,
                        EndDateExclusive = weekStart.AddDays(7)
                    };

                case SadrSalesPeriodPreset.CurrentMonth:
                    var calendar = new PersianCalendar();
                    int year = calendar.GetYear(day);
                    int month = calendar.GetMonth(day);
                    DateTime monthStart = calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
                    DateTime nextMonthStart = month == 12
                        ? calendar.ToDateTime(year + 1, 1, 1, 0, 0, 0, 0)
                        : calendar.ToDateTime(year, month + 1, 1, 0, 0, 0, 0);

                    return new SadrSalesDateRange
                    {
                        StartDateInclusive = monthStart,
                        EndDateExclusive = nextMonthStart
                    };

                default:
                    return new SadrSalesDateRange
                    {
                        StartDateInclusive = day,
                        EndDateExclusive = day.AddDays(1)
                    };
            }
        }
    }
}
