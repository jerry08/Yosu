using System;

namespace Yosu.Models;

public class BackPressedEventArgs : EventArgs
{
    public bool Cancelled { get; private set; }

    public void Cancel()
    {
        Cancelled = true;
    }
}
