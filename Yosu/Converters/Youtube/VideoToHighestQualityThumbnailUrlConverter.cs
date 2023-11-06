using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using YoutubeExplode.Videos;

/* Unmerged change from project 'Yosu (net7.0-android)'
Before:
using YoutubeExplode.Common;
After:
using YoutubeExplode.Common;
using Yosu;
using Yosu.Converters;
using Yosu.Converters.Youtube;
*/
using YoutubeExplode.Common;

namespace Yosu.Converters.Youtube;

public class VideoToHighestQualityThumbnailUrlConverter : IValueConverter
{
    public static VideoToHighestQualityThumbnailUrlConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is IVideo video ? video.Thumbnails.TryGetWithHighestResolution()?.Url : null;

    public object ConvertBack(
        object? value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
