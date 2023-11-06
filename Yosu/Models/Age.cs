using System;

namespace Yosu.Models;

public class Age
{
    public int Years;
    public int Months;
    public int TotalMonths;
    public int Weeks;
    public int Days;
    public int Hours;
    public int Minutes;
    public int Seconds;

    public Age(DateTime Bday)
    {
        Count(Bday);
    }

    public Age Count(DateTime bday)
    {
        var timeSpan = DateTime.Now - bday;
        var days = timeSpan.Days;
        if (timeSpan.Days <= 0)
            days = 0;

        var age = DateTime.MinValue.AddDays(days);

        Years = age.Year - 1;
        Months = age.Month - 1;
        Days = age.Day - 1;
        Hours = timeSpan.Hours;
        Minutes = timeSpan.Minutes;
        Seconds = timeSpan.Seconds;

        TotalMonths = Months + (Years * 12);

        return this;
    }
}
