using System;

namespace Yosu.Models;

public class Age
{
    public int Years { get; private set; }
    public int Months { get; private set; }
    public int TotalMonths => Months + (Years * 12);
    public int Weeks { get; private set; }
    public int Days { get; private set; }
    public int Hours { get; private set; }
    public int Minutes { get; private set; }
    public int Seconds { get; private set; }

    public Age() { }

    public Age(DateTime start) => From(start);

    public static Age From(DateTime start)
    {
        var timeSpan = DateTime.Now - start;
        var days = timeSpan.Days;
        if (timeSpan.Days <= 0)
            days = 0;

        var dateTime = DateTime.MinValue.AddDays(days);

        return new Age()
        {
            Years = dateTime.Year - 1,
            Months = dateTime.Month - 1,
            Days = dateTime.Day - 1,
            Hours = timeSpan.Hours,
            Minutes = timeSpan.Minutes,
            Seconds = timeSpan.Seconds,
        };
    }
}
