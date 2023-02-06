using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using Microsoft.Maui.ApplicationModel;
using Google.Android.Material.BottomSheet;
using Yosu.Services;
using Android.Content.Res;
using Android;

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

        bottomSheetDialog.SetContentView(bottomSheetContent.ToPlatform(page.Handler?.MauiContext ?? throw new Exception("MauiContext is null")));
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