using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using System;

#if ANDROID
using Android.Views;
#elif IOS
using UIKit;
using CoreGraphics;
#elif MACCATALYST
using UIKit;
using CoreGraphics;
#elif WINDOWS
using Microsoft.UI.Xaml;
using Windows.Foundation;
#endif

namespace Maui.FreakyUXKit.Helpers;

public static class ViewPositionExtensions
{
    public static Rect GetRelativeBoundsTo(this VisualElement view, VisualElement relativeTo)
    {
        if (view?.Handler == null || relativeTo?.Handler == null)
            return Rect.Zero;

#if IOS || MACCATALYST
        var nativeView = view.Handler.PlatformView as UIView;
        var nativeRelativeTo = relativeTo.Handler.PlatformView as UIView;

        if (nativeView == null || nativeRelativeTo == null)
            return Rect.Zero;

        CGRect convertedFrame = nativeView.ConvertRectToView(nativeView.Bounds, nativeRelativeTo);

        return new Rect(convertedFrame.X, convertedFrame.Y, convertedFrame.Width, convertedFrame.Height);

#elif WINDOWS
        var nativeView = view.Handler.PlatformView as FrameworkElement;
        var nativeRelativeTo = relativeTo.Handler.PlatformView as FrameworkElement;

        if (nativeView == null || nativeRelativeTo == null)
            return Rect.Zero;

        var transform = nativeView.TransformToVisual(nativeRelativeTo);
        var point = transform.TransformPoint(new Windows.Foundation.Point(0, 0));

        return new Rect(point.X, point.Y, nativeView.ActualWidth, nativeView.ActualHeight);

#else
        // Fallback: basic relative positioning by accumulating parent offsets
        var viewPos = GetAbsolutePosition(view);
        var relativePos = GetAbsolutePosition(relativeTo);

        double x = viewPos.X - relativePos.X;
        double y = viewPos.Y - relativePos.Y;

        return new Rect(x, y, view.Width, view.Height);
#endif
    }

    private static Point GetAbsolutePosition(VisualElement element)
    {
        double x = element.X;
        double y = element.Y;
        var parent = element.Parent as VisualElement;

        while (parent != null)
        {
            x += parent.X;
            y += parent.Y;
            parent = parent.Parent as VisualElement;
        }

        return new Point(x, y);
    }
}
