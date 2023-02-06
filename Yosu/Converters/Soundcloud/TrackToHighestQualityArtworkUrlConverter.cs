using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using SoundCloudExplode.Track;

namespace Yosu.Converters.Soundcloud;

public class TrackToHighestQualityArtworkUrlConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is TrackInformation track
            ? track.ArtworkUrl?.ToString().Replace("large", "t500x500").Replace("small", "t500x500")
            : null;

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}