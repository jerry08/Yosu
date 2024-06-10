using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Cogwheel;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using PropertyChanged;
using Yosu.ViewModels.Components;

namespace Yosu.Services;

[AddINotifyPropertyChangedInterface]
public partial class PreferenceService : SettingsBase, INotifyPropertyChanged
{
    public AppTheme AppTheme { get; set; }

    public List<DownloadItem> Downloads { get; set; } = [];

    public SourceType SearchSourceType { get; set; } = SourceType.Youtube;

    public PreferenceService()
        : base(Path.Combine(FileSystem.AppDataDirectory, "Preferences.json")) { }
}
