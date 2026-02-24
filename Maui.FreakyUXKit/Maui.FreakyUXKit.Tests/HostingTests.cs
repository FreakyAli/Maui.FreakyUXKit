using Xunit;

namespace Maui.FreakyUXKit.Tests;

public class HostingTests
{
    [Fact]
    public void UseFreakyUXKit_ReturnsSameBuilderInstance()
    {
        var builder = MauiApp.CreateBuilder();

        var result = builder.UseFreakyUXKit();

        Assert.Same(builder, result);
    }

    [Fact]
    public void UseFreakyUXKit_DoesNotThrow()
    {
        var builder = MauiApp.CreateBuilder();

        var exception = Record.Exception(() => builder.UseFreakyUXKit());

        Assert.Null(exception);
    }

    [Fact]
    public void UseFreakyUXKit_CanBeCalledMultipleTimes()
    {
        var builder = MauiApp.CreateBuilder();

        var exception = Record.Exception(() =>
        {
            builder.UseFreakyUXKit();
            builder.UseFreakyUXKit();
        });

        Assert.Null(exception);
    }

    [Fact]
    public void UseFreakyUXKit_ReturnsNonNullBuilder()
    {
        var builder = MauiApp.CreateBuilder();

        var result = builder.UseFreakyUXKit();

        Assert.NotNull(result);
    }
}