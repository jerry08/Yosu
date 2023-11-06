using Android.App;
using Android.Content;

namespace Yosu.Platforms.Android;

public class PickDirectoryResult
{
    public Result ResultCode { get; set; }

    public Intent? Data { get; set; }

    public bool IsSuccess { get; set; }

    public PickDirectoryResult() { }

    public PickDirectoryResult(Result resultCode, Intent? data)
    {
        ResultCode = resultCode;
        Data = data;
        IsSuccess = resultCode == Result.Ok;
    }
}
