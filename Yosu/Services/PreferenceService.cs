using System.Collections.Generic;
using System.IO;
using Cogwheel;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Yosu.ViewModels.Components;

namespace Yosu.Services;

public class PreferenceService : SettingsBase
{
    public AppTheme AppTheme { get; set; }

    public List<DownloadItem> Downloads { get; set; } = new();

    public SourceType SearchSourceType { get; set; } = SourceType.Youtube;

    public PreferenceService()
        : base(Path.Combine(FileSystem.AppDataDirectory, "Preferences.json"))
    {
    }
}