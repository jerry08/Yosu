using System.Collections.Generic;
using Microsoft.Maui.ApplicationModel;
using Yosu.Settings;
using Yosu.ViewModels.Components;

namespace Yosu.Services;

public class PreferenceService : SettingsManager
{
    public AppTheme AppTheme { get; set; }

    public List<DownloadItem> Downloads { get; set; } = new();

    public SourceType SearchSourceType { get; set; } = SourceType.Youtube;

    public PreferenceService()
    {
        Configuration.UseSecureStorage = false;
    }
}