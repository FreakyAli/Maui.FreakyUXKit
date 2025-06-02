using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Maui.FreakyUXKit;

internal static class SkiaSharpExtensions
{
    internal static SKRect ToSKRect(this Rect rect)
    {
        return new SKRect(
            (float)rect.Left,
            (float)rect.Top,
            (float)rect.Right,
            (float)rect.Bottom);
    }
    
    public static void DrawArrow(this SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, ArrowStyle style, float strokeWidth)
    {
        switch (style)
        {
            case ArrowStyle.Filled:
                DrawFilledArrow(canvas, start, end, color, strokeWidth);
                break;
            case ArrowStyle.DoubleLine:
                DrawDoubleLineArrow(canvas, start, end, color, strokeWidth);
                break;
            case ArrowStyle.Dashed:
                DrawDashedArrow(canvas, start, end, color, strokeWidth);
                break;
            case ArrowStyle.HandDrawn:
                DrawHandDrawnArrow(canvas, start, end, color, strokeWidth);
                break;
            case ArrowStyle.Sketched:
                DrawSketchedArrow(canvas, start, end, color, strokeWidth);
                break;
            case ArrowStyle.Default:
            default:
                DrawDefaultArrow(canvas, start, end, color, strokeWidth);
                break;
        }
    }

    public static void DrawSketchedArrow(this SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float baseStrokeWidth)
    {
        var rand = new Random();
        int strokePasses = 3;

        for (int i = 0; i < strokePasses; i++)
        {
            float variation = (float)(rand.NextDouble() * 0.6 - 0.3); // +/-30%
            float strokeWidth = baseStrokeWidth * (1f + variation);

            // Slight positional offset for realism
            float offsetX = (float)(rand.NextDouble() - 0.5) * 2f;
            float offsetY = (float)(rand.NextDouble() - 0.5) * 2f;

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = color,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round
            };

            DrawWobblyLine(canvas, start, end, paint, offsetX, offsetY);
        }

        // Arrowhead
        DrawSketchedArrowHead(canvas, start, end, color, baseStrokeWidth);
    }

    private static void DrawWobblyLine(SKCanvas canvas, SKPoint start, SKPoint end, SKPaint paint, float offsetX, float offsetY)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var length = (float)Math.Sqrt(dx * dx + dy * dy);
        var direction = new SKPoint(dx / length, dy / length);

        int segments = 6;
        var rand = new Random();
        var points = new List<SKPoint> { new SKPoint(start.X + offsetX, start.Y + offsetY) };

        for (int i = 1; i < segments; i++)
        {
            float t = i / (float)segments;
            var baseX = start.X + dx * t + offsetX;
            var baseY = start.Y + dy * t + offsetY;

            float wave = (float)(Math.Sin(t * Math.PI * 2) * 1.5f); // subtle wave
            float perpX = -direction.Y * wave;
            float perpY = direction.X * wave;

            points.Add(new SKPoint(baseX + perpX, baseY + perpY));
        }

        points.Add(new SKPoint(end.X + offsetX, end.Y + offsetY));

        var path = new SKPath();
        path.MoveTo(points[0]);
        for (int i = 1; i < points.Count - 1; i++)
        {
            var mid = new SKPoint((points[i].X + points[i + 1].X) / 2, (points[i].Y + points[i + 1].Y) / 2);
            path.QuadTo(points[i], mid);
        }
        path.LineTo(points.Last());

        canvas.DrawPath(path, paint);
    }

    private static void DrawSketchedArrowHead(SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float baseStrokeWidth)
    {
        var angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
        float headLength = 18f;
        float headAngle = (float)(Math.PI / 5); // wider than usual

        var rand = new Random();

        for (int i = 0; i < 2; i++)
        {
            float lengthVariation = (float)(rand.NextDouble() * 4f - 2f);
            float strokeWidth = baseStrokeWidth * (1f + (float)(rand.NextDouble() * 0.3 - 0.15f));

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = color,
                StrokeWidth = strokeWidth,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round
            };

            float angleOffset = i == 0 ? -headAngle : headAngle;
            float len = headLength + lengthVariation;

            var point = new SKPoint(
                end.X - len * (float)Math.Cos(angle + angleOffset),
                end.Y - len * (float)Math.Sin(angle + angleOffset)
            );

            canvas.DrawLine(end, point, paint);
        }
    }

    public static void DrawHandDrawnArrow(this SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round
        };

        // Vector and direction
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var length = (float)Math.Sqrt(dx * dx + dy * dy);
        if (length == 0) return;

        var unit = new SKPoint(dx / length, dy / length);

        // Generate 6-8 control points along the path with subtle random offset
        var points = new List<SKPoint> { start };
        int segments = 6;
        var rand = new Random();

        for (int i = 1; i < segments; i++)
        {
            float t = i / (float)segments;
            var baseX = start.X + dx * t;
            var baseY = start.Y + dy * t;

            // Smaller random offset for more subtle effect
            float offset = strokeWidth * 0.5f; // reduced wobble
            float offsetX = ((float)rand.NextDouble() - 0.5f) * offset;
            float offsetY = ((float)rand.NextDouble() - 0.5f) * offset;

            points.Add(new SKPoint(baseX + offsetX, baseY + offsetY));
        }

        points.Add(end);

        // Draw using a path with smoothed segments
        var path = new SKPath();
        path.MoveTo(points[0]);

        for (int i = 1; i < points.Count - 1; i++)
        {
            var mid = new SKPoint(
                (points[i].X + points[i + 1].X) / 2,
                (points[i].Y + points[i + 1].Y) / 2
            );
            path.QuadTo(points[i], mid);
        }

        path.LineTo(end);
        canvas.DrawPath(path, paint);

        // Draw cleaner arrowhead
        float arrowHeadLength = Math.Min(20f, length * 0.2f);
        float arrowHeadAngle = (float)(Math.PI / 6);
        var angle = Math.Atan2(dy, dx);

        var arrowPoint1 = new SKPoint(
            end.X - arrowHeadLength * (float)Math.Cos(angle - arrowHeadAngle),
            end.Y - arrowHeadLength * (float)Math.Sin(angle - arrowHeadAngle)
        );

        var arrowPoint2 = new SKPoint(
            end.X - arrowHeadLength * (float)Math.Cos(angle + arrowHeadAngle),
            end.Y - arrowHeadLength * (float)Math.Sin(angle + arrowHeadAngle)
        );

        canvas.DrawLine(end, arrowPoint1, paint);
        canvas.DrawLine(end, arrowPoint2, paint);
    }

    public static void DrawDefaultArrow(this SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth = 4f)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        // Draw the main line
        canvas.DrawLine(start, end, paint);

        // Calculate the direction
        var angle = Math.Atan2(end.Y - start.Y, end.X - start.X);

        // Length and angle of the arrowhead lines
        float arrowHeadLength = 20f;
        float arrowHeadAngle = (float)(Math.PI / 6); // 30 degrees

        // Calculate points for arrowhead
        var arrowPoint1 = new SKPoint(
            end.X - arrowHeadLength * (float)Math.Cos(angle - arrowHeadAngle),
            end.Y - arrowHeadLength * (float)Math.Sin(angle - arrowHeadAngle)
        );

        var arrowPoint2 = new SKPoint(
            end.X - arrowHeadLength * (float)Math.Cos(angle + arrowHeadAngle),
            end.Y - arrowHeadLength * (float)Math.Sin(angle + arrowHeadAngle)
        );

        // Draw the arrowhead
        canvas.DrawLine(end, arrowPoint1, paint);
        canvas.DrawLine(end, arrowPoint2, paint);
    }

    private static void DrawFilledArrow(SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
        using var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = color,
            IsAntialias = true
        };

        canvas.DrawLine(start, end, paint);

        var angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
        float arrowHeadLength = 25f;
        float arrowHeadAngle = (float)(Math.PI / 7);

        var arrowPoint1 = new SKPoint(
            end.X - arrowHeadLength * (float)Math.Cos(angle - arrowHeadAngle),
            end.Y - arrowHeadLength * (float)Math.Sin(angle - arrowHeadAngle)
        );

        var arrowPoint2 = new SKPoint(
            end.X - arrowHeadLength * (float)Math.Cos(angle + arrowHeadAngle),
            end.Y - arrowHeadLength * (float)Math.Sin(angle + arrowHeadAngle)
        );

        using var path = new SKPath();
        path.MoveTo(end);
        path.LineTo(arrowPoint1);
        path.LineTo(arrowPoint2);
        path.Close();

        canvas.DrawPath(path, fillPaint);
        canvas.DrawPath(path, paint);
    }

    public static void DrawDoubleLineArrow(this SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round
        };

        // Direction vector
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var length = (float)Math.Sqrt(dx * dx + dy * dy);
        if (length == 0) return;

        var unitDir = new SKPoint(dx / length, dy / length);
        var perpendicular = new SKPoint(-unitDir.Y, unitDir.X);

        // Use strokeWidth to determine gap, but clamp it for visibility
        float arrowGap = Math.Max(4f, strokeWidth * 0.75f); // min 4px spacing
        var offset = new SKPoint(perpendicular.X * arrowGap / 2f, perpendicular.Y * arrowGap / 2f);

        // Arrowhead
        float arrowHeadLength = Math.Min(20f, length * 0.25f); // proportional to arrow length
        float arrowHeadAngle = (float)(Math.PI / 6); // 30 degrees

        // Base of arrowhead (end of shaft)
        var shaftEnd = new SKPoint(
            end.X - unitDir.X * arrowHeadLength,
            end.Y - unitDir.Y * arrowHeadLength
        );

        // Two parallel lines leading to shaftEnd
        canvas.DrawLine(
            new SKPoint(start.X + offset.X, start.Y + offset.Y),
            new SKPoint(shaftEnd.X + offset.X, shaftEnd.Y + offset.Y),
            paint
        );

        canvas.DrawLine(
            new SKPoint(start.X - offset.X, start.Y - offset.Y),
            new SKPoint(shaftEnd.X - offset.X, shaftEnd.Y - offset.Y),
            paint
        );

        // Arrowhead
        var angle = Math.Atan2(dy, dx);
        var arrowPoint1 = new SKPoint(
            end.X - arrowHeadLength * (float)Math.Cos(angle - arrowHeadAngle),
            end.Y - arrowHeadLength * (float)Math.Sin(angle - arrowHeadAngle)
        );

        var arrowPoint2 = new SKPoint(
            end.X - arrowHeadLength * (float)Math.Cos(angle + arrowHeadAngle),
            end.Y - arrowHeadLength * (float)Math.Sin(angle + arrowHeadAngle)
        );

        canvas.DrawLine(end, arrowPoint1, paint);
        canvas.DrawLine(end, arrowPoint2, paint);
    }


    private static void DrawDashedArrow(SKCanvas canvas, SKPoint start, SKPoint end, SKColor color, float strokeWidth)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 0)
        };

        canvas.DrawLine(start, end, paint);

        paint.PathEffect = null;

        var angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
        float arrowHeadLength = 20f;
        float arrowHeadAngle = (float)(Math.PI / 6);

        var arrowPoint1 = new SKPoint(
            end.X - arrowHeadLength * (float)Math.Cos(angle - arrowHeadAngle),
            end.Y - arrowHeadLength * (float)Math.Sin(angle - arrowHeadAngle)
        );

        var arrowPoint2 = new SKPoint(
            end.X - arrowHeadLength * (float)Math.Cos(angle + arrowHeadAngle),
            end.Y - arrowHeadLength * (float)Math.Sin(angle + arrowHeadAngle)
        );

        canvas.DrawLine(end, arrowPoint1, paint);
        canvas.DrawLine(end, arrowPoint2, paint);
    }


    /// <summary>
    /// Calculates the best position for overlay view based on available space
    /// </summary>
    internal static CoachmarkPosition CalculateOptimalPosition(this SKRect targetBounds, SKRect containerBounds,
        Size overlaySize, float margin = 20f)
    {
        var positions = new Dictionary<CoachmarkPosition, float>();

        // Calculate available space in each direction
        float topSpace = targetBounds.Top - margin;
        float bottomSpace = containerBounds.Height - targetBounds.Bottom - margin;
        float leftSpace = targetBounds.Left - margin;
        float rightSpace = containerBounds.Width - targetBounds.Right - margin;

        // Check if overlay fits in each position
        if (topSpace >= overlaySize.Height)
            positions[CoachmarkPosition.Top] = topSpace;

        if (bottomSpace >= overlaySize.Height)
            positions[CoachmarkPosition.Bottom] = bottomSpace;

        if (leftSpace >= overlaySize.Width)
            positions[CoachmarkPosition.Left] = leftSpace;

        if (rightSpace >= overlaySize.Width)
            positions[CoachmarkPosition.Right] = rightSpace;

        // Check diagonal positions
        if (topSpace >= overlaySize.Height && leftSpace >= overlaySize.Width)
            positions[CoachmarkPosition.TopLeft] = Math.Min(topSpace, leftSpace);

        if (topSpace >= overlaySize.Height && rightSpace >= overlaySize.Width)
            positions[CoachmarkPosition.TopRight] = Math.Min(topSpace, rightSpace);

        if (bottomSpace >= overlaySize.Height && leftSpace >= overlaySize.Width)
            positions[CoachmarkPosition.BottomLeft] = Math.Min(bottomSpace, leftSpace);

        if (bottomSpace >= overlaySize.Height && rightSpace >= overlaySize.Width)
            positions[CoachmarkPosition.BottomRight] = Math.Min(bottomSpace, rightSpace);

        // Return position with most space, prefer bottom first, then top
        if (positions.ContainsKey(CoachmarkPosition.Bottom))
            return CoachmarkPosition.Bottom;
        if (positions.ContainsKey(CoachmarkPosition.Top))
            return CoachmarkPosition.Top;

        return positions.Any() ? positions.OrderByDescending(p => p.Value).First().Key : CoachmarkPosition.Bottom;
    }

    /// <summary>
    /// Calculates the margin/positioning for overlay view based on position
    /// </summary>
    internal static Thickness CalculateOverlayMargin(this CoachmarkPosition position, SKRect targetBounds,
        Size overlaySize, SKRect containerBounds, float margin = 10f)
    {
        return position switch
        {
            CoachmarkPosition.Top => new Thickness(
                Math.Max(0, targetBounds.Left),
                Math.Max(0, targetBounds.Top - overlaySize.Height - margin),
                0, 0),

            CoachmarkPosition.Bottom => new Thickness(
                Math.Max(0, targetBounds.Left),
                targetBounds.Bottom + margin,
                0, 0),

            CoachmarkPosition.Left => new Thickness(
                Math.Max(0, targetBounds.Left - overlaySize.Width - margin),
                Math.Max(0, targetBounds.Top),
                0, 0),

            CoachmarkPosition.Right => new Thickness(
                targetBounds.Right + margin,
                Math.Max(0, targetBounds.Top),
                0, 0),

            CoachmarkPosition.TopLeft => new Thickness(
                Math.Max(0, targetBounds.Left - overlaySize.Width - margin),
                Math.Max(0, targetBounds.Top - overlaySize.Height - margin),
                0, 0),

            CoachmarkPosition.TopRight => new Thickness(
                targetBounds.Right + margin,
                Math.Max(0, targetBounds.Top - overlaySize.Height - margin),
                0, 0),

            CoachmarkPosition.BottomLeft => new Thickness(
                Math.Max(0, targetBounds.Left - overlaySize.Width - margin),
                targetBounds.Bottom + margin,
                0, 0),

            CoachmarkPosition.BottomRight => new Thickness(
                targetBounds.Right + margin,
                targetBounds.Bottom + margin,
                0, 0),

            _ => new Thickness(Math.Max(0, targetBounds.Left), targetBounds.Bottom + margin, 0, 0)
        };
    }

    internal static void DrawBasicBackgroundOverlay(this SKCanvas canvas, SKRect rect)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = Constants.backgroundSKColor,
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

    public static void DrawHighlightShape(this SKCanvas canvas, SKRect rect, HighlightShape shape)
    {
        using var paint = new SKPaint
        {
            BlendMode = SKBlendMode.Clear,
            IsAntialias = true
        };

        switch (shape)
        {
            case HighlightShape.Circle:
                canvas.DrawCircle(rect.MidX, rect.MidY, Math.Min(rect.Width, rect.Height) / 2, paint);
                break;
            case HighlightShape.RoundRectangle:
                canvas.DrawRoundRect(rect, 20f, 20f, paint);
                break;
            case HighlightShape.Rectangle:
            default:
                canvas.DrawRect(rect, paint);
                break;
        }
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
        float centerX, float centerY, float targetWidth, float targetHeight, float cornerRadius, float progress, Color focusMauiColor)
    {
        var focusColor = focusMauiColor.ToSKColor();   // Red color for focus effect

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
                Color = Constants.backgroundSKColor,
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