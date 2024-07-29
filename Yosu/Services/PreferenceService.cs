using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Yosu.ViewModels.Components;
using YoutubeExplode.Videos;
using SoundCloudTrack = SoundCloudExplode.Tracks.Track;
using SoundCloudTrackSearchResult = SoundCloudExplode.Search.TrackSearchResult;
using SpotifyTrack = SpotifyExplode.Tracks.Track;
using SpotifyTrackSearchResult = SpotifyExplode.Search.TrackSearchResult;

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

    private SourceType _searchSourceType = SourceType.Youtube;
    public SourceType SearchSourceType
    {
        get => _searchSourceType;
        set => SetProperty(ref _searchSourceType, value);
    }
}

// Need to specify `TypeInfoPropertyName` when two classes share
// the same name but different namespaces
// https://github.com/dotnet/runtime/issues/58198
public partial class PreferenceService
{
    [JsonSerializable(typeof(PreferenceService))]
    [JsonSerializable(typeof(Video))]
    [JsonSerializable(typeof(SpotifyTrack), TypeInfoPropertyName = nameof(SpotifyTrack))]
    [JsonSerializable(
        typeof(SpotifyTrackSearchResult),
        TypeInfoPropertyName = nameof(SpotifyTrackSearchResult)
    )]
    [JsonSerializable(typeof(SoundCloudTrack), TypeInfoPropertyName = nameof(SoundCloudTrack))]
    [JsonSerializable(
        typeof(SoundCloudTrackSearchResult),
        TypeInfoPropertyName = nameof(SoundCloudTrackSearchResult)
    )]
    private partial class SerializerContext : JsonSerializerContext;
}
