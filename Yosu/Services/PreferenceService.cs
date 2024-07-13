using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Yosu.ViewModels.Components;
using YoutubeExplode.Videos;

namespace Yosu.Services;

// Can't use [ObservableProperty] here because System.Text.Json's source generator doesn't see
// the generated properties.
[INotifyPropertyChanged]
public partial class PreferenceService()
    : SettingsBase(
        Path.Combine(FileSystem.AppDataDirectory, "Preferences.json"),
        SerializerContext.Default
    )
{
    private AppTheme _appTheme;
    public AppTheme AppTheme
    {
        get => _appTheme;
        set => SetProperty(ref _appTheme, value);
    }

    private List<DownloadItem> _downloads = [];
    public List<DownloadItem> Downloads
    {
        get => _downloads;
        set => SetProperty(ref _downloads, value);
    }

    private SourceType _searchSourceType = SourceType.Youtube;
    public SourceType SearchSourceType
    {
        get => _searchSourceType;
        set => SetProperty(ref _searchSourceType, value);
    }
}

public partial class PreferenceService
{
    [JsonSerializable(typeof(PreferenceService))]
    [JsonSerializable(typeof(Video))]
    private partial class SerializerContext : JsonSerializerContext;
}
