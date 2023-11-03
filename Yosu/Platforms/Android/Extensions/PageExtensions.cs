using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using Microsoft.Maui.ApplicationModel;
using Google.Android.Material.BottomSheet;
using Yosu.Services;
using Android.Content.Res;
using Android;
using Android.Widget;
using Android.Views;
using Orientation = Android.Content.Res.Orientation;

namespace Yosu.Extensions;

public static class PageExtensions
{
    class BottomSheetController : IBottomSheetController
    {
        private readonly BottomSheetDialog _dialog;

        public BottomSheetController(BottomSheetDialog dialog)
        {
            _dialog = dialog;
        }

        void IBottomSheetController.Dismiss()
        {
            _dialog.Dismiss();
        }
    }

    public static IBottomSheetController ShowBottomSheet(this Page page, IView bottomSheetContent, bool dimDismiss)
    {
        var bottomSheetDialog = new BottomSheetDialog(
            Platform.CurrentActivity?.Window?.DecorView.FindViewById(Android.Resource.Id.Content)?.Context ?? throw new InvalidOperationException("Context is null")
        //Resource.Style.Test1
        //Resource.Style.CustomBottomSheetDialogTheme
        );

        var view = bottomSheetContent.ToPlatform(page.Handler?.MauiContext ?? throw new Exception("MauiContext is null"));
        var c = new FrameLayout(page.Handler!.MauiContext!.Context);
        c.AddView(view, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

        bottomSheetDialog.SetContentView(c);
        bottomSheetDialog.Behavior.Hideable = dimDismiss;
        bottomSheetDialog.Behavior.FitToContents = true;

        if (bottomSheetDialog.Context.Resources?.Configuration?.Orientation != Orientation.Portrait)
        {
            bottomSheetDialog.Behavior.State = BottomSheetBehavior.StateExpanded;
        }

        bottomSheetDialog.Show();

        return new BottomSheetController(bottomSheetDialog);
    }
}