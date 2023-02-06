using System;
using System.IO;
using Microsoft.Maui.Storage;
using SpotifyExplode.Tracks;

namespace Yosu.ViewModels.Components;

public class SpotifyDownloadViewModel : DownloadViewModelBase
{
    public TrackId? Key => Track?.Id;

    public Track? Track { get; set; }

    public SpotifyDownloadViewModel()
    {
        TempFilePath = Path.Combine(FileSystem.CacheDirectory, $"{DateTime.Now.Ticks}.mp3");
    }
}