using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using TagLib;
using TagLib.Id3v2;
using Yosu.Core.Utils;
using Yosu.Youtube.Core.Utils;
using File = System.IO.File;
using TagFile = TagLib.File;

namespace Yosu.Youtube.Core.Tagging;

internal partial class MediaFile(TagFile file) : IDisposable
{
    private readonly TagFile _file = file;

    /*public void SetThumbnail(byte[] thumbnailData)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"{DateTime.Now.Ticks}.jpeg");

        try
        {
            var ms = new MemoryStream();

            //using var image = await Image.LoadAsync(path...);
            using var image = Image.Load(thumbnailData);
            //image.SaveAsPng(tempFilePath, new PngEncoder()
            image.SaveAsPng(ms, new PngEncoder()
            {
                //Method = WebpEncodingMethod.BestQuality
            });

            thumbnailData = ms.ToArray();
        }
        catch
        {
            File.Delete(tempFilePath);
        }

        _file.Tag.Pictures = new IPicture[] { new Picture(thumbnailData) };
    }*/

    public void SetThumbnail(byte[] thumbnailData)
    {
        var ms = new MemoryStream();

        //using var image = await Image.LoadAsync(path...);
        using var image = Image.Load(thumbnailData);
        //image.SaveAsPng(tempFilePath, new PngEncoder()
        image.SaveAsJpeg(
            ms,
            new JpegEncoder()
            {
                //Method = WebpEncodingMethod.BestQuality
            }
        );

        thumbnailData = ms.ToArray();

        var picture = new AttachmentFrame
        {
            TextEncoding = FileEx.IsValidISO(thumbnailData) ? StringType.Latin1 : StringType.UTF8,
            Data = thumbnailData
        };

        _file.Tag.Pictures = [picture];
    }

    public void SetArtist(string artist) => _file.Tag.Performers = [artist];

    public void SetArtistSort(string artistSort) => _file.Tag.PerformersSort = [artistSort];

    public void SetTitle(string title) => _file.Tag.Title = title;

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
