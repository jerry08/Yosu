using System;
using System.Diagnostics.CodeAnalysis;
using YoutubeExplode.Videos.Streams;

namespace Yosu.Youtube.Converter;

/// <summary>
/// Encapsulates conversion media format.
/// </summary>
/// <remarks>
/// Initializes an instance of <see cref="ConversionFormat" />.
/// </remarks>
[Obsolete("Use YoutubeExplode.Videos.Streams.Container instead"), ExcludeFromCodeCoverage]
public readonly struct ConversionFormat(string name)
{
    /// <summary>
    /// Format name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Whether this format is a known audio-only format.
    /// </summary>
    public bool IsAudioOnly => new Container(Name).IsAudioOnly();

    /// <inheritdoc />
    public override string ToString() => Name;
}
