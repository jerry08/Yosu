using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Yosu.Youtube.Core.Downloading;

namespace Yosu.Converters.Youtube;

public class VideoQualityPreferenceToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is VideoQualityPreference preference)
            return preference.GetDisplayName();

        return default(string);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotImplementedException();
}
