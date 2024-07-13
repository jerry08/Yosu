using System;
using TagLib;
using TagFile = TagLib.File;

namespace Yosu.Soundcloud.Core.Tagging;

internal partial class MediaFile : IDisposable
{
    private readonly TagFile _file;

    public MediaFile(TagFile file) => _file = file;

    public void SetThumbnail(byte[] thumbnailData) =>
        _file.Tag.Pictures = [new Picture(thumbnailData)];

    public void SetArtist(string artist) => _file.Tag.Performers = [artist];

    public void SetArtistSort(string artistSort) => _file.Tag.PerformersSort = [artistSort];

    public void SetTitle(string title) => _file.Tag.Title = title;

    public void SetPerformers(string[] performers) => _file.Tag.Performers = performers;

    public void SetAlbum(string album) => _file.Tag.Album = album;

    public void SetDescription(string description) => _file.Tag.Description = description;

    public void SetComment(string comment) => _file.Tag.Comment = comment;

    public void Dispose()
    {
        _file.Tag.DateTagged = DateTime.Now;
        _file.Save();
        _file.Dispose();
    }
}

internal partial class MediaFile
{
    public static MediaFile Create(string filePath) => new(TagFile.Create(filePath));
}
