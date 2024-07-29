﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Yosu.Youtube.Converter.Utils.Extensions;
using YoutubeExplode.Videos.Streams;

namespace Yosu.Youtube.Converter;

/// <summary>
/// Builder for <see cref="ConversionRequest" />.
/// </summary>
public class ConversionRequestBuilder(string outputFilePath)
{
    private string? _ffmpegCliFilePath;
    private Container? _container;
    private ConversionPreset _preset;

    private Container GetDefaultContainer() =>
        new(Path.GetExtension(outputFilePath).TrimStart('.').NullIfWhiteSpace() ?? "mp4");

    /// <summary>
    /// Sets the path to the FFmpeg CLI.
    /// </summary>
    public ConversionRequestBuilder SetFFmpegPath(string path)
    {
        _ffmpegCliFilePath = path;
        return this;
    }

    /// <summary>
    /// Sets the output container.
    /// </summary>
    public ConversionRequestBuilder SetContainer(Container container)
    {
        _container = container;
        return this;
    }

    /// <summary>
    /// Sets the output container.
    /// </summary>
    public ConversionRequestBuilder SetContainer(string container) =>
        SetContainer(new Container(container));

    /// <summary>
    /// Sets the conversion format.
    /// </summary>
    [Obsolete("Use SetContainer instead."), ExcludeFromCodeCoverage]
    public ConversionRequestBuilder SetFormat(ConversionFormat format) =>
        SetContainer(new Container(format.Name));

    /// <summary>
    /// Sets the conversion format.
    /// </summary>
    [Obsolete("Use SetContainer instead."), ExcludeFromCodeCoverage]
    public ConversionRequestBuilder SetFormat(string format) => SetContainer(format);

    /// <summary>
    /// Sets the conversion preset.
    /// </summary>
    public ConversionRequestBuilder SetPreset(ConversionPreset preset)
    {
        _preset = preset;
        return this;
    }

    /// <summary>
    /// Builds the resulting request.
    /// </summary>
    public ConversionRequest Build() =>
        new(
            _ffmpegCliFilePath ?? FFmpeg.GetFilePath(),
            outputFilePath,
            _container ?? GetDefaultContainer(),
            _preset
        );
}
