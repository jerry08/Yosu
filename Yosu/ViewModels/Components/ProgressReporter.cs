using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Gress;

namespace Yosu.ViewModels.Components;

public partial class ProgressReporter : ObservableObject, IProgress<Percentage>
{
    public EventHandler<Percentage>? OnReport;

    [ObservableProperty]
    public double _progressFraction;

    public void Report(Percentage value)
    {
        ProgressFraction = value.Fraction;
        OnReport?.Invoke(this, value);
    }
}