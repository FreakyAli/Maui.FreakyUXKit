using Microsoft.Maui.Graphics;
using SkiaSharp;

namespace Maui.FreakyUXKit.Tests;

public class SkiaSharpExtensionsTests
{
    #region ToSKRect

    [Fact]
    public void ToSKRect_ConvertsPositiveRectCorrectly()
    {
        var rect = new Rect(10, 20, 100, 50);

        var skRect = rect.ToSKRect();

        Assert.Equal(10f, skRect.Left);
        Assert.Equal(20f, skRect.Top);
        Assert.Equal(110f, skRect.Right);   // Left + Width
        Assert.Equal(70f, skRect.Bottom);   // Top + Height
    }

    [Fact]
    public void ToSKRect_ZeroRect_ProducesEmptySKRect()
    {
        var rect = Rect.Zero;

        var skRect = rect.ToSKRect();

        Assert.Equal(0f, skRect.Left);
        Assert.Equal(0f, skRect.Top);
        Assert.Equal(0f, skRect.Right);
        Assert.Equal(0f, skRect.Bottom);
    }

    [Fact]
    public void ToSKRect_RightEqualsLeftPlusWidth()
    {
        var rect = new Rect(5, 15, 80, 40);

        var skRect = rect.ToSKRect();

        Assert.Equal((float)(rect.Left + rect.Width), skRect.Right, 3);
        Assert.Equal((float)(rect.Top + rect.Height), skRect.Bottom, 3);
    }

    #endregion

    #region CalculateOptimalPosition

    [Fact]
    public void CalculateOptimalPosition_PrefersBottom_WhenBothTopAndBottomFit()
    {
        // topSpace = 200 - 20 = 180 >= 100 ✓, bottomSpace = 800 - 250 - 20 = 530 >= 100 ✓
        var targetBounds = new SKRect(100, 200, 300, 250);
        var containerBounds = new SKRect(0, 0, 400, 800);
        var overlaySize = new Size(150, 100);

        var result = targetBounds.CalculateOptimalPosition(containerBounds, overlaySize);

        Assert.Equal(CoachmarkPosition.Bottom, result);
    }

    [Fact]
    public void CalculateOptimalPosition_ReturnsTop_WhenOnlyTopHasSpace()
    {
        // bottomSpace = 800 - 700 - 20 = 80 < 100, topSpace = 650 - 20 = 630 >= 100
        var targetBounds = new SKRect(100, 650, 300, 700);
        var containerBounds = new SKRect(0, 0, 400, 800);
        var overlaySize = new Size(150, 100);

        var result = targetBounds.CalculateOptimalPosition(containerBounds, overlaySize);

        Assert.Equal(CoachmarkPosition.Top, result);
    }

    [Fact]
    public void CalculateOptimalPosition_ReturnsLeft_WhenOnlyLeftHasSpace()
    {
        // Target near right edge: rightSpace = 400 - 390 - 20 = -10 ✗
        // bottomSpace = 150 - 100 - 20 = 30 < 80 ✗, topSpace = 50 - 20 = 30 < 80 ✗
        // leftSpace = 300 - 20 = 280 >= 200 ✓
        var targetBounds = new SKRect(300, 50, 390, 100);
        var containerBounds = new SKRect(0, 0, 400, 150);
        var overlaySize = new Size(200, 80);

        var result = targetBounds.CalculateOptimalPosition(containerBounds, overlaySize);

        Assert.Equal(CoachmarkPosition.Left, result);
    }

    [Fact]
    public void CalculateOptimalPosition_ReturnsRight_WhenOnlyRightHasSpace()
    {
        // Target near left edge: leftSpace = 10 - 20 = -10 ✗
        // rightSpace = 400 - 60 - 20 = 320 >= 200 ✓
        // topSpace = 50 - 20 = 30 < 80 ✗, bottomSpace = 150 - 100 - 20 = 30 < 80 ✗
        var targetBounds = new SKRect(10, 50, 60, 100);
        var containerBounds = new SKRect(0, 0, 400, 150);
        var overlaySize = new Size(200, 80);

        var result = targetBounds.CalculateOptimalPosition(containerBounds, overlaySize);

        Assert.Equal(CoachmarkPosition.Right, result);
    }

    [Fact]
    public void CalculateOptimalPosition_FallsBackToBottom_WhenNothingFits()
    {
        var targetBounds = new SKRect(40, 40, 60, 60);
        var containerBounds = new SKRect(0, 0, 100, 100);
        var overlaySize = new Size(500, 500);

        var result = targetBounds.CalculateOptimalPosition(containerBounds, overlaySize);

        Assert.Equal(CoachmarkPosition.Bottom, result);
    }

    #endregion

    #region CalculateOverlayMargin

    [Fact]
    public void CalculateOverlayMargin_Bottom_PositionsBelowTarget()
    {
        var targetBounds = new SKRect(50, 100, 200, 150);
        var overlaySize = new Size(100, 80);
        var containerBounds = new SKRect(0, 0, 400, 800);

        var result = CoachmarkPosition.Bottom.CalculateOverlayMargin(targetBounds, overlaySize, containerBounds, margin: 10f);

        Assert.Equal(50.0, result.Left, 1);
        Assert.Equal(160.0, result.Top, 1);  // Bottom (150) + margin (10)
        Assert.Equal(0.0, result.Right, 1);
        Assert.Equal(0.0, result.Bottom, 1);
    }

    [Fact]
    public void CalculateOverlayMargin_Top_PositionsAboveTarget()
    {
        var targetBounds = new SKRect(50, 200, 200, 250);
        var overlaySize = new Size(100, 80);
        var containerBounds = new SKRect(0, 0, 400, 800);

        var result = CoachmarkPosition.Top.CalculateOverlayMargin(targetBounds, overlaySize, containerBounds, margin: 10f);

        Assert.Equal(50.0, result.Left, 1);
        Assert.Equal(110.0, result.Top, 1);  // Top (200) - height (80) - margin (10)
        Assert.Equal(0.0, result.Right, 1);
        Assert.Equal(0.0, result.Bottom, 1);
    }

    [Fact]
    public void CalculateOverlayMargin_Left_PositionsLeftOfTarget()
    {
        var targetBounds = new SKRect(200, 100, 350, 150);
        var overlaySize = new Size(100, 80);
        var containerBounds = new SKRect(0, 0, 400, 800);

        var result = CoachmarkPosition.Left.CalculateOverlayMargin(targetBounds, overlaySize, containerBounds, margin: 10f);

        Assert.Equal(90.0, result.Left, 1);   // Left (200) - width (100) - margin (10)
        Assert.Equal(100.0, result.Top, 1);
        Assert.Equal(0.0, result.Right, 1);
        Assert.Equal(0.0, result.Bottom, 1);
    }

    [Fact]
    public void CalculateOverlayMargin_Right_PositionsRightOfTarget()
    {
        var targetBounds = new SKRect(50, 100, 200, 150);
        var overlaySize = new Size(100, 80);
        var containerBounds = new SKRect(0, 0, 400, 800);

        var result = CoachmarkPosition.Right.CalculateOverlayMargin(targetBounds, overlaySize, containerBounds, margin: 10f);

        Assert.Equal(210.0, result.Left, 1);  // Right (200) + margin (10)
        Assert.Equal(100.0, result.Top, 1);
        Assert.Equal(0.0, result.Right, 1);
        Assert.Equal(0.0, result.Bottom, 1);
    }

    [Fact]
    public void CalculateOverlayMargin_Auto_FallsBackToBottomBehaviour()
    {
        var targetBounds = new SKRect(50, 100, 200, 150);
        var overlaySize = new Size(100, 80);
        var containerBounds = new SKRect(0, 0, 400, 800);

        var result = CoachmarkPosition.Auto.CalculateOverlayMargin(targetBounds, overlaySize, containerBounds, margin: 10f);

        // Default/Auto falls through to the same calculation as Bottom
        Assert.Equal(50.0, result.Left, 1);
        Assert.Equal(160.0, result.Top, 1);
    }

    [Fact]
    public void CalculateOverlayMargin_Top_ClampsTopToZero_WhenNegative()
    {
        // Target very close to top: Top - overlayHeight - margin would be negative
        var targetBounds = new SKRect(50, 5, 200, 30);
        var overlaySize = new Size(100, 80);
        var containerBounds = new SKRect(0, 0, 400, 800);

        var result = CoachmarkPosition.Top.CalculateOverlayMargin(targetBounds, overlaySize, containerBounds, margin: 10f);

        Assert.Equal(0.0, result.Top, 1); // Math.Max(0, 5 - 80 - 10) = 0
    }

    #endregion

    #region Canvas Drawing

    [Fact]
    public void DrawDarkOverlayWithSpotlight_DoesNotThrow()
    {
        using var bitmap = new SKBitmap(400, 800);
        using var canvas = new SKCanvas(bitmap);

        var exception = Record.Exception(() =>
            canvas.DrawDarkOverlayWithSpotlight(200f, 400f, 100f, 50f));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(ArrowStyle.Default)]
    [InlineData(ArrowStyle.Filled)]
    [InlineData(ArrowStyle.DoubleLine)]
    [InlineData(ArrowStyle.Dashed)]
    [InlineData(ArrowStyle.HandDrawn)]
    [InlineData(ArrowStyle.Sketched)]
    public void DrawArrow_AllStyles_DoNotThrow(ArrowStyle style)
    {
        using var bitmap = new SKBitmap(400, 800);
        using var canvas = new SKCanvas(bitmap);
        var start = new SKPoint(50, 50);
        var end = new SKPoint(200, 200);

        var exception = Record.Exception(() =>
            canvas.DrawArrow(start, end, SKColors.Red, style, 4f));

        Assert.Null(exception);
    }

    [Fact]
    public void DrawDefaultArrow_DoesNotThrow_WithZeroLengthLine()
    {
        using var bitmap = new SKBitmap(200, 200);
        using var canvas = new SKCanvas(bitmap);
        var point = new SKPoint(100, 100);

        var exception = Record.Exception(() =>
            canvas.DrawDefaultArrow(point, point, SKColors.Blue, 2f));

        Assert.Null(exception);
    }

    [Fact]
    public void DrawDoubleLineArrow_DoesNotThrow_WithZeroLengthLine()
    {
        using var bitmap = new SKBitmap(200, 200);
        using var canvas = new SKCanvas(bitmap);
        var point = new SKPoint(100, 100);

        var exception = Record.Exception(() =>
            canvas.DrawDoubleLineArrow(point, point, SKColors.White, 3f));

        Assert.Null(exception);
    }

    #endregion
}
