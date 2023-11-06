using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Yosu.Converters;

public class TimeSpanToHumanReadableFormatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan duration)
        {
            if (duration.TotalSeconds < 3600)
            {
                //return string.Format("{0}:{1:0#}.{2:000}", duration.Minutes, duration.Seconds, duration.Milliseconds);
                return string.Format("{0}:{1:0#}", duration.Minutes, duration.Seconds);
            }
            else
            {
                //return string.Format("{0}:{1:0#}:{2:0#}.{3:000}", (int)duration.TotalHours, duration.Minutes, duration.Seconds, duration.Milliseconds);
                return string.Format(
                    "{0}:{1:0#}:{2:0#}",
                    (int)duration.TotalHours,
                    duration.Minutes,
                    duration.Seconds
                );
            }
        }

        return null;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
