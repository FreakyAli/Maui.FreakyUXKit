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

    internal static void DrawHighlightCutOut(this SKCanvas canvas, HighlightShape shape, float centerX, float centerY, 
        float width, float height, float cornerRadius)
    {
        using var clearPaint = new SKPaint
        {
            BlendMode = SKBlendMode.Clear,
            IsAntialias = true
        };

        DrawShape(canvas, shape, centerX, centerY, width, height, cornerRadius, clearPaint);
    }

    internal static void DrawHighlightStroke(this SKCanvas canvas, HighlightShape shape, float centerX, float centerY, 
        float width, float height, float cornerRadius = 20f)
    {
        using var strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 4,
            IsAntialias = true
        };

        DrawShape(canvas, shape, centerX, centerY, width, height, cornerRadius, strokePaint);
    }

    private static void DrawShape(SKCanvas canvas, HighlightShape shape, float centerX, float centerY, 
        float width, float height, float cornerRadius, SKPaint paint)
    {
        switch (shape)
        {
            case HighlightShape.Circle:
                float radius = Math.Max(width, height) / 2;
                canvas.DrawCircle(centerX, centerY, radius, paint);
                break;
            case HighlightShape.Ellipse:
                canvas.DrawOval(new SKRect(centerX - width / 2, centerY - height / 2, 
                    centerX + width / 2, centerY + height / 2), paint);
                break;
            case HighlightShape.Rectangle:
                canvas.DrawRect(new SKRect(centerX - width / 2, centerY - height / 2, 
                    centerX + width / 2, centerY + height / 2), paint);
                break;
            default: // RoundRectangle
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(centerX - width / 2, centerY - height / 2, 
                    centerX + width / 2, centerY + height / 2), cornerRadius), paint);
                break;
        }
    }

    /// <summary>
    /// Draws the Focus animation effect with ripple convergence and pulsing highlight
    /// </summary>
    internal static void DrawFocusRippleEffect(this SKCanvas canvas, SKRect canvasRect, HighlightShape shape, 
        float centerX, float centerY, float targetWidth, float targetHeight, float cornerRadius, float progress)
    {
        // Define colors - keep original dark overlay and distinct ripple color
        var originalColor = new SKColor(0, 0, 0, 180); // Original dark overlay
        var focusColor = new SKColor(255, 0, 0);   // Red color for focus effect
        
        // Calculate the maximum distance from center to any corner of the screen
        float maxDistance = Math.Max(
            Math.Max(
                (float)Math.Sqrt(Math.Pow(centerX, 2) + Math.Pow(centerY, 2)),
                (float)Math.Sqrt(Math.Pow(canvasRect.Width - centerX, 2) + Math.Pow(centerY, 2))
            ),
            Math.Max(
                (float)Math.Sqrt(Math.Pow(centerX, 2) + Math.Pow(canvasRect.Height - centerY, 2)),
                (float)Math.Sqrt(Math.Pow(canvasRect.Width - centerX, 2) + Math.Pow(canvasRect.Height - centerY, 2))
            )
        );
        
        // Phase 1: Convergence phase (0.0 to 0.5) - ripple converges to highlight (happens only once)
        // Phase 2: Static focus background with subtle bounce on highlight (0.5 to 1.0 and beyond)
        const float convergencePhaseEnd = 0.5f;
        
        // Check if we've completed the first ripple cycle
        bool hasCompletedFirstRipple = progress > 1.0f;
        
        if (!hasCompletedFirstRipple && progress <= convergencePhaseEnd)
        {
            // Convergence phase - show the ripple effect (only during first cycle)
            float convergenceProgress = progress / convergencePhaseEnd;
            float minRadius = Math.Max(targetWidth, targetHeight) / 2;
            float currentRadius = maxDistance - (maxDistance - minRadius) * convergenceProgress;
            
            // Step 1: Draw the original dark overlay everywhere first
            using var originalPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = originalColor,
                IsAntialias = true
            };
            canvas.DrawRect(canvasRect, originalPaint);
            
            // Step 2: Draw the focus area ONLY in the ripple zone (outside the current radius)
            canvas.Save();
            
            // Create inverse clipping path - everything OUTSIDE the current ripple boundary
            var focusClipPath = CreateFocusClipPath(shape, centerX, centerY, currentRadius, targetWidth, targetHeight, cornerRadius, convergenceProgress, true);
            
            // Invert the clipping - we want to draw focus color OUTSIDE the convergence area
            var fullScreenPath = new SKPath();
            fullScreenPath.AddRect(canvasRect);
            
            // Use instance Op method to create the difference path
            var inversePath = fullScreenPath.Op(focusClipPath, SKPathOp.Difference);
            
            if (inversePath != null)
            {
                canvas.ClipPath(inversePath);
            }
            else
            {
                // Fallback: use simple rect clipping if path operation fails
                canvas.ClipRect(canvasRect);
            }
            
            // Draw focus color only in the clipped area (outside convergence boundary)
            using var focusPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = focusColor,
                IsAntialias = true
            };
            canvas.DrawRect(canvasRect, focusPaint);
            
            canvas.Restore();
            
            // Clean up paths
            fullScreenPath.Dispose();
            inversePath?.Dispose();
            
            // Step 3: Draw focus ring at the boundary
            DrawFocusRing(canvas, shape, centerX, centerY, currentRadius, cornerRadius, targetWidth, targetHeight, convergenceProgress, true);
        }
        else
        {
            // After ripple completion OR during bounce phase - use focus color as the new background
            // This ensures the background stays red after the ripple completes
            using var backgroundPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = focusColor, // Use focus color as the permanent background now
                IsAntialias = true
            };
            canvas.DrawRect(canvasRect, backgroundPaint);
        }
    }

    private static SKPath CreateFocusClipPath(HighlightShape shape, float centerX, float centerY, float radius,
        float targetWidth, float targetHeight, float cornerRadius, float convergenceProgress, bool isConverging)
    {
        var clipPath = new SKPath();

        switch (shape)
        {
            case HighlightShape.Circle:
                clipPath.AddCircle(centerX, centerY, radius);
                break;

            case HighlightShape.Ellipse:
                if (isConverging)
                {
                    float aspectRatio = targetHeight / targetWidth;
                    float ellipseWidth = radius * 2;
                    float ellipseHeight = radius * 2 * aspectRatio;

                    float convergeFactor = 1f - convergenceProgress;
                    ellipseWidth = targetWidth + (ellipseWidth - targetWidth) * convergeFactor;
                    ellipseHeight = targetHeight + (ellipseHeight - targetHeight) * convergeFactor;

                    clipPath.AddOval(new SKRect(
                        centerX - ellipseWidth / 2,
                        centerY - ellipseHeight / 2,
                        centerX + ellipseWidth / 2,
                        centerY + ellipseHeight / 2));
                }
                break;

            case HighlightShape.Rectangle:
                if (isConverging)
                {
                    float convergeFactor = 1f - convergenceProgress;
                    float rectWidth = targetWidth + (radius * 2 - targetWidth) * convergeFactor;
                    float rectHeight = targetHeight + (radius * 2 - targetHeight) * convergeFactor;

                    clipPath.AddRect(new SKRect(
                        centerX - rectWidth / 2,
                        centerY - rectHeight / 2,
                        centerX + rectWidth / 2,
                        centerY + rectHeight / 2));
                }
                break;

            default: // RoundRectangle
                if (isConverging)
                {
                    float convergeFactor = 1f - convergenceProgress;
                    float roundRectWidth = targetWidth + (radius * 2 - targetWidth) * convergeFactor;
                    float roundRectHeight = targetHeight + (radius * 2 - targetHeight) * convergeFactor;
                    float currentCornerRadius = cornerRadius + (cornerRadius * 3 - cornerRadius) * convergeFactor;

                    clipPath.AddRoundRect(new SKRect(
                        centerX - roundRectWidth / 2,
                        centerY - roundRectHeight / 2,
                        centerX + roundRectWidth / 2,
                        centerY + roundRectHeight / 2),
                        currentCornerRadius, currentCornerRadius);
                }
                break;
        }

        return clipPath;
    }

    private static void DrawFocusRing(SKCanvas canvas, HighlightShape shape, float centerX, float centerY,
        float radius, float cornerRadius, float targetWidth, float targetHeight, float convergenceProgress, bool isConverging)
    {
        using var ringPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(255, 255, 255, 120),
            StrokeWidth = 3f,
            IsAntialias = true
        };

        switch (shape)
        {
            case HighlightShape.Circle:
                canvas.DrawCircle(centerX, centerY, radius, ringPaint);
                break;

            case HighlightShape.Ellipse:
                if (isConverging)
                {
                    float aspectRatio = targetHeight / targetWidth;
                    float convergeFactor = 1f - convergenceProgress;
                    float ellipseWidth = targetWidth + (radius * 2 - targetWidth) * convergeFactor;
                    float ellipseHeight = targetHeight + (radius * 2 * aspectRatio - targetHeight) * convergeFactor;

                    canvas.DrawOval(new SKRect(
                        centerX - ellipseWidth / 2,
                        centerY - ellipseHeight / 2,
                        centerX + ellipseWidth / 2,
                        centerY + ellipseHeight / 2), ringPaint);
                }
                break;

            case HighlightShape.Rectangle:
                if (isConverging)
                {
                    float convergeFactor = 1f - convergenceProgress;
                    float rectWidth = targetWidth + (radius * 2 - targetWidth) * convergeFactor;
                    float rectHeight = targetHeight + (radius * 2 - targetHeight) * convergeFactor;

                    canvas.DrawRect(new SKRect(
                        centerX - rectWidth / 2,
                        centerY - rectHeight / 2,
                        centerX + rectWidth / 2,
                        centerY + rectHeight / 2), ringPaint);
                }
                break;

            default: // RoundRectangle
                if (isConverging)
                {
                    float convergeFactor = 1f - convergenceProgress;
                    float roundRectWidth = targetWidth + (radius * 2 - targetWidth) * convergeFactor;
                    float roundRectHeight = targetHeight + (radius * 2 - targetHeight) * convergeFactor;
                    float currentCornerRadius = cornerRadius + (cornerRadius * 3 - cornerRadius) * convergeFactor;

                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(
                        centerX - roundRectWidth / 2,
                        centerY - roundRectHeight / 2,
                        centerX + roundRectWidth / 2,
                        centerY + roundRectHeight / 2),
                        currentCornerRadius), ringPaint);
                }
                break;
        }
    }
}