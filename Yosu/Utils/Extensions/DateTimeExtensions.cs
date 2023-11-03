using System;
using Yosu.Models;

namespace Yosu.Utils.Extensions;

public static class DateTimeExtensions
{
    public static string PeriodOfTimeFormat(Age tspan)
    {
        if (tspan.Years >= 1)
            return string.Format("{0} {1}", tspan.Years, (tspan.Years > 1) ? "years" : "year");
        else if (tspan.Months >= 1)
            return string.Format("{0} {1}", tspan.Months, (tspan.Months > 1) ? "months" : "month");
        else if (tspan.Days >= 1)
            return string.Format("{0} {1}", tspan.Days, (tspan.Days > 1) ? "days" : "day");
        else if (tspan.Hours >= 1)
            return string.Format("{0} {1}", tspan.Hours, (tspan.Hours > 1) ? "hours" : "hour");
        else if (tspan.Minutes >= 1)
            return string.Format("{0} {1}", tspan.Minutes, (tspan.Minutes > 1) ? "minutes" : "minute");
        else if (tspan.Seconds >= 1)
            return string.Format("{0} {1}", tspan.Seconds, (tspan.Seconds > 1) ? "seconds" : "second");
        else
            return "1 second";
    }

    public static string PeriodOfTimeShortFormat(Age tspan)
    {
        if (tspan.Years >= 1)
            return string.Format("{0}{1}", tspan.Years, "y");
        else if (tspan.Months >= 1)
            return string.Format("{0}{1}", tspan.Months, "m");
        else if (tspan.Days >= 1)
            return string.Format("{0}{1}", tspan.Days, "d");
        else if (tspan.Hours >= 1)
            return string.Format("{0}{1}", tspan.Hours, "h");
        else if (tspan.Minutes >= 1)
            return string.Format("{0}{1}", tspan.Minutes, "m");
        else if (tspan.Seconds >= 1)
            return string.Format("{0}{1}", tspan.Seconds, "s");
        else
            return "1s";
    }

    public static string PeriodOfTimeFormat(this DateTime dateTime) =>
        PeriodOfTimeFormat(new Age(dateTime));

    public static string? PeriodOfTimeFormat(this DateTime? dateTime) =>
        dateTime is not null ? PeriodOfTimeFormat(dateTime) : null;

    public static string PeriodOfTimeShortFormat(this DateTime dateTime) =>
        PeriodOfTimeShortFormat(new Age(dateTime));

    public static string? PeriodOfTimeShortFormat(this DateTime? dateTime) =>
        dateTime is not null ? PeriodOfTimeShortFormat(dateTime) : null;

    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        var diff = dt.DayOfWeek - startOfWeek;
        if (diff < 0)
            diff += 7;
        return dt.AddDays(-1 * diff).Date;
    }
}