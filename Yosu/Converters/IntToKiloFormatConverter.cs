using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Yosu.Utils.Extensions;

namespace Yosu.Converters;

public class IntToKiloFormatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        int.TryParse(value?.ToString(), out var count) ? count.ToKiloFormat(false) : null;

    public object ConvertBack(
        object? value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
