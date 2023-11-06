using System;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;
using SpotifyExplode.Tracks;

namespace Yosu.Converters.Spotify;

public class TrackToHighestQualityArtworkUrlConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is Track track
            ? track.Album.Images.OrderByDescending(x => x.Height).FirstOrDefault()?.Url
            : null;

    public object ConvertBack(
        object? value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
