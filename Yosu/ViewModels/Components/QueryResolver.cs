using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gress;
using ScQueryResolver = Yosu.Soundcloud.Core.Resolving.QueryResolver;
using SpQueryResolver = Yosu.Spotify.Core.Resolving.QueryResolver;
using YtQueryResolver = Yosu.Youtube.Core.Resolving.QueryResolver;

namespace Yosu.ViewModels.Components;

internal class QueryResolver
{
    private readonly YtQueryResolver _ytQueryResolver = new();
    private readonly ScQueryResolver _scQueryResolver = new();
    private readonly SpQueryResolver _spQueryResolver = new();

    public async Task<List<object>> ResolveAsync(
        string query,
        SourceType sourceType,
        IProgress<Percentage>? progress = null,
        CancellationToken cancellationToken = default
    )
    {
        var results = new List<object>();

        var queries = query.Split(
            "\n",
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );

        var ytQueries = new List<string>();
        var scQueries = new List<string>();
        var spQueries = new List<string>();

        foreach (var q in queries)
        {
            if (Uri.IsWellFormedUriString(q, UriKind.Absolute))
            {
                var host = new Uri(q).Host;

                if (host.Contains("youtube") || host.Contains("youtu.be"))
                    ytQueries.Add(q);
                else if (host.Contains("soundcloud"))
                    scQueries.Add(q);
                else if (host.Contains("spotify"))
                    spQueries.Add(q);
            }
            else
            {
                switch (sourceType)
                {
                    case SourceType.Youtube:
                        ytQueries.Add(q);
                        break;
                    case SourceType.Soundcloud:
                        scQueries.Add(q);
                        break;
                    case SourceType.Spotify:
                        spQueries.Add(q);
                        break;
                }
            }
        }

        if (ytQueries.Count > 0)
        {
            var ytResults = await _ytQueryResolver.ResolveAsync(
                queries,
                progress,
                cancellationToken
            );
            var downloads = ytResults.Videos.Select(video => new YoutubeDownloadViewModel()
            {
                Video = video
            });
            results.AddRange(downloads);
        }

        if (scQueries.Count > 0)
        {
            var scResults = await _scQueryResolver.ResolveAsync(
                queries,
                progress,
                cancellationToken
            );
            var downloads = scResults.Tracks.Select(track => new SoundcloudDownloadViewModel()
            {
                Track = track
            });
            results.AddRange(downloads);
        }

        if (spQueries.Count > 0)
        {
            var spResults = await _spQueryResolver.ResolveAsync(
                queries,
                progress,
                cancellationToken
            );
            var downloads = spResults.Tracks.Select(track => new SpotifyDownloadViewModel()
            {
                Track = track
            });
            results.AddRange(downloads);
        }

        return results;
    }
}
