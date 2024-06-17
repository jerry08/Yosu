using System;
using Yosu.Models;

namespace Yosu.Utils.Extensions;

public static class DateTimeExtensions
{
    public static string PeriodOfTimeFormat(Age age)
    {
        if (age.Years >= 1)
            return string.Format("{0} {1}", age.Years, (age.Years > 1) ? "years" : "year");
        else if (age.Months >= 1)
            return string.Format("{0} {1}", age.Months, (age.Months > 1) ? "months" : "month");
        else if (age.Days >= 1)
            return string.Format("{0} {1}", age.Days, (age.Days > 1) ? "days" : "day");
        else if (age.Hours >= 1)
            return string.Format("{0} {1}", age.Hours, (age.Hours > 1) ? "hours" : "hour");
        else if (age.Minutes >= 1)
            return string.Format("{0} {1}", age.Minutes, (age.Minutes > 1) ? "minutes" : "minute");
        else if (age.Seconds >= 1)
            return string.Format("{0} {1}", age.Seconds, (age.Seconds > 1) ? "seconds" : "second");
        else
            return "1 second";
    }

    public static string PeriodOfTimeShortFormat(Age age)
    {
        if (age.Years >= 1)
            return string.Format("{0}{1}", age.Years, "y");
        else if (age.Months >= 1)
            return string.Format("{0}{1}", age.Months, "mo");
        else if (age.Days >= 1)
            return string.Format("{0}{1}", age.Days, "d");
        else if (age.Hours >= 1)
            return string.Format("{0}{1}", age.Hours, "h");
        else if (age.Minutes >= 1)
            return string.Format("{0}{1}", age.Minutes, "m");
        else if (age.Seconds >= 1)
            return string.Format("{0}{1}", age.Seconds, "s");
        else
            return "1s";
    }

    public static string PeriodOfTimeFormat(this DateTime dateTime) =>
        PeriodOfTimeFormat(Age.From(dateTime));

    public static string? PeriodOfTimeFormat(this DateTime? dateTime) =>
        dateTime is not null ? PeriodOfTimeFormat(dateTime) : null;

    public static string PeriodOfTimeShortFormat(this DateTime dateTime) =>
        PeriodOfTimeShortFormat(Age.From(dateTime));

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
