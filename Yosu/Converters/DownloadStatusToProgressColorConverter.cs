using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Yosu.ViewModels.Components;

namespace Yosu.Converters;

public class DownloadStatusToProgressColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DownloadStatus status)
        {
            return status switch
            {
                DownloadStatus.None => Colors.Blue,
                DownloadStatus.Enqueued => Colors.Blue,
                DownloadStatus.Started => Colors.Blue,
                DownloadStatus.Completed => Colors.Green,
                DownloadStatus.Failed => Colors.Red,
                DownloadStatus.Canceled => Colors.Orange,
                _ => Colors.Blue,
            };
        }

        return Colors.Blue;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
