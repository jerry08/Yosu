using System;

namespace Yosu.Utils;

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

    public Age Count(DateTime Bday)
    {
        var PresentYear = DateTime.Now;
        var ts = PresentYear - Bday;
        var days = ts.Days;
        if (ts.Days <= 0)
            days = 0;

        var Age = DateTime.MinValue.AddDays(days);

        Years = Age.Year - 1;
        Months = Age.Month - 1;
        Days = Age.Day - 1;
        Hours = ts.Hours;
        Minutes = ts.Minutes;
        Seconds = ts.Seconds;

        TotalMonths = Months + (Years * 12);

        return this;
    }
}