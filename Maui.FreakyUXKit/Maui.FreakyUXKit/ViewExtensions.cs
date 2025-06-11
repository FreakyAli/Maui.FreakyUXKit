using Rect = Microsoft.Maui.Graphics.Rect;
using Point = Microsoft.Maui.Graphics.Point;
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

namespace Maui.FreakyUXKit;

public static class ViewExntensions
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
        
#elif ANDROID
        var nativeView = view.Handler.PlatformView as Android.Views.View;
        var nativeRelativeTo = relativeTo.Handler.PlatformView as Android.Views.View;

        if (nativeView == null || nativeRelativeTo == null)
            return Rect.Zero;

        int[] viewLocation = new int[2];
        int[] relativeLocation = new int[2];

        nativeView.GetLocationOnScreen(viewLocation);
        nativeRelativeTo.GetLocationOnScreen(relativeLocation);

        double x = viewLocation[0] - relativeLocation[0];
        double y = viewLocation[1] - relativeLocation[1];

        double width = nativeView.Width / nativeView.Context.Resources.DisplayMetrics.Density;
        double height = nativeView.Height / nativeView.Context.Resources.DisplayMetrics.Density;

        x /= nativeView.Context.Resources.DisplayMetrics.Density;
        y /= nativeView.Context.Resources.DisplayMetrics.Density;

        return new Rect(x, y, width, height);

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
