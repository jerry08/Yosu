namespace Yosu.Utils.Extensions;

public static class StringExtensions
{
    public static string ToKiloFormat(this int num, bool applyToThousand = true)
    {
        if (num >= 1000000000)
            return (num / 1000000000D).ToString("0.#") + "B";

        if (num >= 100000000)
            return (num / 1000000D).ToString("#,0M");

        if (num >= 1000000)
            return (num / 1000000D).ToString("0.#") + "M";

        if (num >= 100000)
            return (num / 1000D).ToString("#,0K");

        if (applyToThousand)
        {
            if (num >= 1000)
                return (num / 1000D).ToString("0.#") + "K";
        }
        else
        {
            if (num >= 10000)
                return (num / 1000D).ToString("0.#") + "K";
        }

        return num.ToString("#,0");
    }
}
