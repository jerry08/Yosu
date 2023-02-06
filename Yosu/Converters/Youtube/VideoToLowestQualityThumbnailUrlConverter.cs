using System;
using System.Linq;
using System.Globalization;
using Microsoft.Maui.Controls;

/* Unmerged change from project 'Yosu (net7.0-android)'
Before:
using YoutubeExplode.Videos;
After:
using YoutubeExplode.Videos;
using Yosu;
using Yosu.Converters;
using Yosu.Converters.Youtube;
*/
using YoutubeExplode.Videos;

namespace Yosu.Converters.Youtube;

public class VideoToLowestQualityThumbnailUrlConverter : IValueConverter
{
    public static VideoToLowestQualityThumbnailUrlConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is IVideo video
            ? video.Thumbnails.OrderBy(t => t.Resolution.Area).FirstOrDefault()?.Url
            : null;

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}