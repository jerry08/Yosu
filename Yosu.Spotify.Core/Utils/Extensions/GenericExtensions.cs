using System;

namespace Yosu.Spotify.Core.Utils.Extensions;

internal static class GenericExtensions
{
    public static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> transform) => transform(input);
}