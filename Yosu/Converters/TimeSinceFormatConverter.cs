using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Yosu.Utils.Extensions;

namespace Yosu.Converters;

public class TimeSinceFormatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        DateTime.TryParse(value?.ToString(), out var dateTime)
            ? dateTime.PeriodOfTimeShortFormat()
            : null;

    public object ConvertBack(
        object? value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
