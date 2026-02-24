using Xunit;

namespace Maui.FreakyUXKit.Tests;

public class ViewExtensionsTests
{
    // In tests, views are not attached to a visual tree so Handler is always null.
    // These tests cover the null-guard path in GetRelativeBoundsTo.

    [Fact]
    public void GetRelativeBoundsTo_ReturnsZeroRect_WhenViewHasNoHandler()
    {
        var view = new Label();
        var relativeTo = new Label();

        var result = view.GetRelativeBoundsTo(relativeTo);

        Assert.Equal(Rect.Zero, result);
    }

    [Fact]
    public void GetRelativeBoundsTo_ReturnsZeroRect_WhenRelativeToHasNoHandler()
    {
        var view = new Label();
        var relativeTo = new Label();

        var result = view.GetRelativeBoundsTo(relativeTo);

        Assert.Equal(Rect.Zero, result);
    }

    [Fact]
    public void GetRelativeBoundsTo_ReturnsZeroRect_WhenViewIsNull()
    {
        Label? view = null;
        var relativeTo = new Label();

        var result = view!.GetRelativeBoundsTo(relativeTo);

        Assert.Equal(Rect.Zero, result);
    }

    [Fact]
    public void GetRelativeBoundsTo_ReturnsZeroRect_WhenRelativeToIsNull()
    {
        var view = new Label();
        Label? relativeTo = null;

        var result = view.GetRelativeBoundsTo(relativeTo!);

        Assert.Equal(Rect.Zero, result);
    }
}