using System.Net.Http;

namespace Yosu.Soundcloud.Core.Utils;

internal static class Http
{
    public static HttpClient Client { get; } = new();
}