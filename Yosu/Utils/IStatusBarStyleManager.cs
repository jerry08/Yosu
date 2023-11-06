namespace Yosu.Utils;

public partial interface IStatusBarStyleManager
{
    /// <summary>
    /// Sets default style based on light/dark theme.
    /// </summary>
    void SetDefault();

    void SetColoredStatusBar(string hexColor, bool isLight);

    void SetWhiteStatusBar();
}
