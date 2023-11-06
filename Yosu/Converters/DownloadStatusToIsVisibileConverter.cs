using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Yosu.ViewModels.Components;

namespace Yosu.Converters;

public class DownloadStatusToIsVisibileConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DownloadStatus status)
        {
            return status switch
            {
                DownloadStatus.None => false,
                DownloadStatus.Enqueued => true,
                DownloadStatus.Started => true,
                DownloadStatus.Completed => true,
                DownloadStatus.Failed => true,
                DownloadStatus.Canceled => false,
                _ => false,
            };
        }

        return true;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
