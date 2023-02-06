using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Threading.Tasks;
using Plugin.MauiTouchEffect.Extensions;
using Yosu.ViewModels;
using Yosu.ViewModels.Components;

namespace Yosu.Views;

public partial class YoutubeVideoItemView
{
    public YoutubeVideoItemView()
    {
        InitializeComponent();

        //Loaded += YoutubeVideoItemView_Loaded;
    }

    private void YoutubeVideoItemView_Loaded(object? sender, System.EventArgs e)
    {
        OnBufferProgressBarVisibilityChanged();

        //if (BindingContext is YoutubeDownloadViewModel download)
        //{
        //    var test = YoutubeViewModel.Downloads
        //        .FirstOrDefault(x => x.Video?.Id == download.Video?.Id);
        //    if (test is not null)
        //    {
        //        if (test == download)
        //        {
        //
        //        }
        //        else
        //        {
        //            BindingContext = test;
        //        }
        //    }
        //}
    }

    private async void OnBufferProgressBarVisibilityChanged()
    {
        //while (bufferProgressBar.IsVisible)
        //{
        //    bufferProgressBar.Rotation = 0;
        //    await bufferProgressBar.ProgressTo(1, 500, Easing.Linear);
        //
        //    bufferProgressBar.Rotation = 180;
        //    await bufferProgressBar.ProgressTo(0, 500, Easing.Linear);
        //
        //    await Task.Yield();
        //}
        //
        //bufferProgressBar.CancelAnimations();

        //var lowerAnimation = new Animation(v => AnimatedProgressBar.LowerRangeValue = (float)v, -0.4, 1.0);
        //var upperAnimation = new Animation(v => AnimatedProgressBar.UpperRangeValue = (float)v, 0.0, 1.4);
        //
        //lowerAnimation.Commit(AnimatedProgressBar, "lower", length: 1500, easing: Easing.CubicInOut, repeat: () => true);
        //upperAnimation.Commit(AnimatedProgressBar, "upper", length: 1500, easing: Easing.CubicInOut, repeat: () => true);
        //
        //while (true)
        //{
        //    await Task.Delay(3000);
        //    break;
        //}
        //
        //AnimatedProgressBar.CancelAnimations();
        //AnimatedProgressBar.AbortAnimation("lower");
        //AnimatedProgressBar.AbortAnimation("upper");
        ////AnimatedProgressBar.AbortAnimations();
        //
        //AnimatedProgressBar.UseRange = false;
        //AnimatedProgressBar.Progress = 0.1f;
    }
}