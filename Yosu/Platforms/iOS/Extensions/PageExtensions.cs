using System;
using UIKit;
using Microsoft.Maui;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls;
using Yosu.Services;

namespace Yosu.Extensions;

public static class PageExtensions
{
    class BottomSheetController : IBottomSheetController
    {
        private readonly UIViewController _controller;

        public BottomSheetController(UIViewController controller)
        {
            _controller = controller;
        }

        void IBottomSheetController.Dismiss()
        {
            //_controller.DismissViewController(true, null);
            _controller.View?.RemoveFromSuperview();
            _controller.RemoveFromParentViewController();
        }
    }

    public static IBottomSheetController ShowBottomSheet(
        this Page page,
        IView bottomSheetContent,
        bool dimDismiss
    )
    {
        var mauiContext = page.Handler?.MauiContext ?? throw new Exception("MauiContext is null");
        var viewController = page.ToUIViewController(mauiContext);
        var viewControllerToPresent = bottomSheetContent.ToUIViewController(mauiContext);

        var sheet = viewControllerToPresent.SheetPresentationController;
        if (sheet is not null)
        {
            sheet.Detents = new[]
            {
                UISheetPresentationControllerDetent.CreateMediumDetent(),
                UISheetPresentationControllerDetent.CreateLargeDetent(),
            };
            sheet.LargestUndimmedDetentIdentifier = dimDismiss
                ? UISheetPresentationControllerDetentIdentifier.Unknown
                : UISheetPresentationControllerDetentIdentifier.Medium;
            sheet.PrefersScrollingExpandsWhenScrolledToEdge = false;
            sheet.PrefersEdgeAttachedInCompactHeight = true;
            sheet.WidthFollowsPreferredContentSizeWhenEdgeAttached = true;
        }

        viewController.PresentViewController(viewControllerToPresent, animated: true, null);

        return new BottomSheetController(viewController);
    }
}
