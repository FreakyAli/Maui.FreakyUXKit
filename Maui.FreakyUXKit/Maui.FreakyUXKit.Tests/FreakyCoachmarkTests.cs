using Microsoft.Maui;
using Microsoft.Maui.Graphics;

namespace Maui.FreakyUXKit.Tests;

public class FreakyCoachmarkTests
{
    // Minimal BindableObject to host attached properties without needing a real view
    private class TestBindable : BindableObject { }

    #region Default Values

    [Fact]
    public void ArrowStrokeWidth_DefaultValue_IsTwo()
    {
        var bindable = new TestBindable();

        Assert.Equal(2.0f, FreakyCoachmark.GetArrowStrokeWidth(bindable));
    }

    [Fact]
    public void ArrowStyle_DefaultValue_IsDefault()
    {
        var bindable = new TestBindable();

        Assert.Equal(ArrowStyle.Default, FreakyCoachmark.GetArrowStyle(bindable));
    }

    [Fact]
    public void ArrowColor_DefaultValue_IsRed()
    {
        var bindable = new TestBindable();

        Assert.Equal(Colors.Red, FreakyCoachmark.GetArrowColor(bindable));
    }

    [Fact]
    public void HighlightPadding_DefaultValue_IsZero()
    {
        var bindable = new TestBindable();

        Assert.Equal(0.0f, FreakyCoachmark.GetHighlightPadding(bindable));
    }

    [Fact]
    public void PreferredPosition_DefaultValue_IsAuto()
    {
        var bindable = new TestBindable();

        Assert.Equal(CoachmarkPosition.Auto, FreakyCoachmark.GetPreferredPosition(bindable));
    }

    [Fact]
    public void OverlayMargin_DefaultValue_IsTen()
    {
        var bindable = new TestBindable();

        Assert.Equal(10.0f, FreakyCoachmark.GetOverlayMargin(bindable));
    }

    [Fact]
    public void HighlightShapeCornerRadius_DefaultValue_IsTen()
    {
        var bindable = new TestBindable();

        Assert.Equal(10.0f, FreakyCoachmark.GetHighlightShapeCornerRadius(bindable));
    }

    [Fact]
    public void CoachmarkAnimation_DefaultValue_IsSpotlight()
    {
        var bindable = new TestBindable();

        Assert.Equal(CoachmarkAnimationStyle.Spotlight, FreakyCoachmark.GetCoachmarkAnimation(bindable));
    }

    [Fact]
    public void HighlightShape_DefaultValue_IsRoundRectangle()
    {
        var bindable = new TestBindable();

        Assert.Equal(HighlightShape.RoundRectangle, FreakyCoachmark.GetHighlightShape(bindable));
    }

    [Fact]
    public void OverlayView_DefaultValue_IsNull()
    {
        var bindable = new TestBindable();

        Assert.Null(FreakyCoachmark.GetOverlayView(bindable));
    }

    [Fact]
    public void AreCoachmarksEnabled_DefaultValue_IsFalse()
    {
        var bindable = new TestBindable();

        Assert.False(FreakyCoachmark.GetAreCoachmarksEnabled(bindable));
    }

    [Fact]
    public void DisplayOrder_DefaultValue_IsIntMaxValue()
    {
        var bindable = new TestBindable();

        Assert.Equal(int.MaxValue, FreakyCoachmark.GetDisplayOrder(bindable));
    }

    [Fact]
    public void CompletedCommand_DefaultValue_IsNull()
    {
        var bindable = new TestBindable();

        Assert.Null(FreakyCoachmark.GetCompletedCommand(bindable));
    }

    #endregion

    #region Set / Get Round Trips

    [Fact]
    public void SetArrowStrokeWidth_GetArrowStrokeWidth_RoundTrips()
    {
        var bindable = new TestBindable();

        FreakyCoachmark.SetArrowStrokeWidth(bindable, 8.5f);

        Assert.Equal(8.5f, FreakyCoachmark.GetArrowStrokeWidth(bindable));
    }

    [Fact]
    public void SetArrowStyle_GetArrowStyle_RoundTrips()
    {
        var bindable = new TestBindable();

        FreakyCoachmark.SetArrowStyle(bindable, ArrowStyle.Dashed);

        Assert.Equal(ArrowStyle.Dashed, FreakyCoachmark.GetArrowStyle(bindable));
    }

    [Fact]
    public void SetArrowColor_GetArrowColor_RoundTrips()
    {
        var bindable = new TestBindable();

        FreakyCoachmark.SetArrowColor(bindable, Colors.Blue);

        Assert.Equal(Colors.Blue, FreakyCoachmark.GetArrowColor(bindable));
    }

    [Fact]
    public void SetPreferredPosition_GetPreferredPosition_RoundTrips()
    {
        var bindable = new TestBindable();

        FreakyCoachmark.SetPreferredPosition(bindable, CoachmarkPosition.Top);

        Assert.Equal(CoachmarkPosition.Top, FreakyCoachmark.GetPreferredPosition(bindable));
    }

    [Fact]
    public void SetHighlightShape_GetHighlightShape_RoundTrips()
    {
        var bindable = new TestBindable();

        FreakyCoachmark.SetHighlightShape(bindable, HighlightShape.Circle);

        Assert.Equal(HighlightShape.Circle, FreakyCoachmark.GetHighlightShape(bindable));
    }

    [Fact]
    public void SetCoachmarkAnimation_GetCoachmarkAnimation_RoundTrips()
    {
        var bindable = new TestBindable();

        FreakyCoachmark.SetCoachmarkAnimation(bindable, CoachmarkAnimationStyle.Focus);

        Assert.Equal(CoachmarkAnimationStyle.Focus, FreakyCoachmark.GetCoachmarkAnimation(bindable));
    }

    [Fact]
    public void SetHighlightPadding_GetHighlightPadding_RoundTrips()
    {
        var bindable = new TestBindable();

        FreakyCoachmark.SetHighlightPadding(bindable, 15.0f);

        Assert.Equal(15.0f, FreakyCoachmark.GetHighlightPadding(bindable));
    }

    [Fact]
    public void SetOverlayMargin_GetOverlayMargin_RoundTrips()
    {
        var bindable = new TestBindable();

        FreakyCoachmark.SetOverlayMargin(bindable, 25.0f);

        Assert.Equal(25.0f, FreakyCoachmark.GetOverlayMargin(bindable));
    }

    [Fact]
    public void SetHighlightShapeCornerRadius_GetHighlightShapeCornerRadius_RoundTrips()
    {
        var bindable = new TestBindable();

        FreakyCoachmark.SetHighlightShapeCornerRadius(bindable, 20.0f);

        Assert.Equal(20.0f, FreakyCoachmark.GetHighlightShapeCornerRadius(bindable));
    }

    #endregion

    #region Independent Property Isolation

    [Fact]
    public void Properties_AreIndependent_BetweenInstances()
    {
        var bindable1 = new TestBindable();
        var bindable2 = new TestBindable();

        FreakyCoachmark.SetArrowStrokeWidth(bindable1, 5.0f);

        Assert.Equal(5.0f, FreakyCoachmark.GetArrowStrokeWidth(bindable1));
        Assert.Equal(2.0f, FreakyCoachmark.GetArrowStrokeWidth(bindable2)); // unchanged
    }

    #endregion

    #region ClearRegisteredCoachmarks

    [Fact]
    public void ClearRegisteredCoachmarks_DoesNotThrow()
    {
        var exception = Record.Exception(() => FreakyCoachmark.ClearRegisteredCoachmarks());

        Assert.Null(exception);
    }

    [Fact]
    public void ClearRegisteredCoachmarks_CanBeCalledMultipleTimes()
    {
        var exception = Record.Exception(() =>
        {
            FreakyCoachmark.ClearRegisteredCoachmarks();
            FreakyCoachmark.ClearRegisteredCoachmarks();
        });

        Assert.Null(exception);
    }

    #endregion
}
