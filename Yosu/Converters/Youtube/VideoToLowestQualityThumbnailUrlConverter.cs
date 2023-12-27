using System;
using System.Linq;
using System.Globalization;
using Microsoft.Maui.Controls;
using YoutubeExplode.Videos;

namespace Yosu.Converters.Youtube;

public class VideoToLowestQualityThumbnailUrlConverter : IValueConverter
{
    public static VideoToLowestQualityThumbnailUrlConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is IVideo video
            ? video.Thumbnails.OrderBy(t => t.Resolution.Area).FirstOrDefault()?.Url
            : null;

    public object ConvertBack(
        object? value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
