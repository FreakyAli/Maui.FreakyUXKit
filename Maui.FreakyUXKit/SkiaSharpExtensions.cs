using System;
using SkiaSharp;

namespace Maui.FreakyUXKit;

internal static class SkiaSharpExtensions
{
    internal static void DrawBasicBackgroundOverlay(this SKCanvas canvas, SKRect rect)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(0, 0, 0, 180),
            IsAntialias = true
        };
        canvas.DrawRect(rect, paint);
    }

    internal static void DrawHighlightCutOut(this SKCanvas canvas, HighlightShape shape, float highX, float highY, float width,float height, float cornerRadius)
    {
        using var clearPaint = new SKPaint
        {
            BlendMode = SKBlendMode.Clear,
            IsAntialias = true
        };

        switch (shape)
        {
            case HighlightShape.Circle:
                float radius = Math.Max(width, height) / 2;
                canvas.DrawCircle(highX, highY, radius, clearPaint);
                break;
            case HighlightShape.Ellipse:
                canvas.DrawOval(new SKRect(highX - width / 2, highY - height / 2, highX + width / 2, highY + height / 2), clearPaint);
                break;
            case HighlightShape.Rectangle:
                canvas.DrawRect(new SKRect(highX - width / 2, highY - height / 2, highX + width / 2, highY + height / 2), clearPaint);
                break;
            default:
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(highX - width / 2, highY - height / 2, highX + width / 2, highY + height / 2), cornerRadius), clearPaint);
                break;
        }
    }
    
    internal static void DrawHighlightStroke(this SKCanvas canvas, HighlightShape shape, float highX, float highY, float width, float height)
    {
        using var strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 4,
            IsAntialias = true
        };

        switch (shape)
        {
            case HighlightShape.Circle:
                float radius = Math.Max(width, height) / 2;
                canvas.DrawCircle(highX, highY, radius, strokePaint);
                break;
            case HighlightShape.Ellipse:
                canvas.DrawOval(new SKRect(highX - width / 2, highY - height / 2, highX + width / 2, highY + height / 2), strokePaint);
                break;
            case HighlightShape.Rectangle:
                canvas.DrawRect(new SKRect(highX - width / 2, highY - height / 2, highX + width / 2, highY + height / 2), strokePaint);
                break;
            default:
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(highX - width / 2, highY - height / 2, highX + width / 2, highY + height / 2), 20), strokePaint);
                break;
        }
    }
}