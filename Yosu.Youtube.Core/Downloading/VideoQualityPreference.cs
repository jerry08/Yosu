﻿using System;

namespace Yosu.Youtube.Core.Downloading;

public enum VideoQualityPreference
{
    Lowest,
    UpTo360p,
    UpTo480p,
    UpTo720p,
    UpTo1080p,
    Highest,
}

public static class VideoQualityPreferenceExtensions
{
    public static string GetDisplayName(this VideoQualityPreference preference) =>
        preference switch
        {
            VideoQualityPreference.Lowest => "Lowest quality",
            VideoQualityPreference.UpTo360p => "≤ 360p",
            VideoQualityPreference.UpTo480p => "≤ 480p",
            VideoQualityPreference.UpTo720p => "≤ 720p",
            VideoQualityPreference.UpTo1080p => "≤ 1080p",
            VideoQualityPreference.Highest => "Highest quality",
            _ => throw new ArgumentOutOfRangeException(nameof(preference)),
        };
}
