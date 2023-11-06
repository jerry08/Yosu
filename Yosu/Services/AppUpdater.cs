using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Octokit;

namespace Yosu.Services;

public class AppUpdater
{
    private readonly IReleasesClient _releaseClient;
    private readonly GitHubClient _github;

    private readonly string _repositoryOwner = "jerry08";
    private readonly string _repostoryName = "Yosu";
    private Release LatestRelease = default!;

    public AppUpdater()
    {
        _github = new GitHubClient(new ProductHeaderValue(_repostoryName + "-UpdateCheck"));
        _releaseClient = _github.Repository.Release;
    }

    public async Task CheckAsync()
    {
        var dontShow = false;
        var dontShowStr = await SecureStorage.GetAsync(
            $"dont_ask_for_update_{AppInfo.Current.VersionString}"
        );
        if (!string.IsNullOrEmpty(dontShowStr))
            dontShow = Convert.ToBoolean(dontShowStr);

        if (dontShow)
            return;

        try
        {
            //var releases = await _releaseClient.GetAll(_repositoryOwner, _repostoryName);
            //var latestRelease = releases.FirstOrDefault();

            var latestRelease = await _releaseClient.GetLatest(_repositoryOwner, _repostoryName);
            if (latestRelease is null)
                return;

            var latestVersion = new Version(latestRelease.Name);
            var currentVersion = AppInfo.Current.Version;

            if (currentVersion < latestVersion)
            {
                var update = await App.AlertSvc.ShowConfirmationAsync(
                    "Update available",
                    "",
                    "DOWNLOAD",
                    "DISMISS"
                );

                if (update)
                {
                    var asset = latestRelease.Assets.FirstOrDefault()!;

                    await DownloadCenter.Current.EnqueueAsync(asset.Name, asset.BrowserDownloadUrl);
                }
            }
        }
        catch
        {
            // Repository not found
        }
    }

    public async Task<string> RenderReleaseNotes()
    {
        if (LatestRelease is null)
            throw new InvalidOperationException();

        return await _github.Markdown.RenderRawMarkdown(LatestRelease.Body);
    }
}
