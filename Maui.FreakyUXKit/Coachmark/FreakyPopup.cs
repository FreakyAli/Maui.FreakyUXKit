using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Maui.FreakyUXKit.Helpers;

namespace Maui.FreakyUXKit;
internal class FreakyPopup : Popup
{
    private CancellationTokenSource cancelationTokenSource = new();
    private readonly IEnumerable<View> _views;
    private Grid? contentGrid;
    private int currentIndex;

    internal FreakyPopup(IEnumerable<View> coachMarkViews)
    {
        CanBeDismissedByTappingOutsideOfPopup = false;
        Color = Colors.Transparent;
        _views = coachMarkViews;
        Content = SetupContainer();
        Opened += OnPopupOpened;
    }

    protected override Task OnClosed(object result, bool wasDismissedByTappingOutsideOfPopup, CancellationToken token = default)
    {
        cancelationTokenSource?.Cancel();
        cancelationTokenSource?.Dispose();
        Opened -= OnPopupOpened;
        return base.OnClosed(result, wasDismissedByTappingOutsideOfPopup, token);
    }

    private async void OnPopupOpened(object sender, PopupOpenedEventArgs e)
    {
        Size = new Size(Constants.Width, Constants.Height);
        await Task.Delay(10);
        var firstView = _views.FirstOrDefault();
        if (firstView is View currentView)
            DrawCoachMarkForView(currentView);
    }

    private void DrawCoachMarkForView(View view)
    {
        if (contentGrid is null || view.Handler is null)
            return;

        ResetView();

        var bounds = view.GetRelativeBoundsTo(contentGrid);
        var coachmarkView = (View)FreakyCoachmark.GetOverlayView(view);
        var shape = FreakyCoachmark.GetHighlightShape(view);


        if (coachmarkView.HorizontalOptions == LayoutOptions.Start ||
            coachmarkView.HorizontalOptions == LayoutOptions.Center ||
            coachmarkView.HorizontalOptions == LayoutOptions.End)
        {
            coachmarkView.MaximumWidthRequest = bounds.Width;
        }

        coachmarkView.Margin = new Thickness(bounds.X, bounds.Y + bounds.Height + 10, 0, 0);

        const double borderPadding = 4; 

        var highlightBox = new Border
        {
            BackgroundColor = Colors.Transparent,
            Stroke = Colors.White,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            WidthRequest = bounds.Width + (borderPadding * 2),
            HeightRequest = bounds.Height + (borderPadding * 2),
            Padding = borderPadding,
            Margin = new Thickness(bounds.X, bounds.Y, 0, 0),  
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start
        };

        highlightBox.StrokeShape = shape switch
        {
            HighlightShape.Circle => new Ellipse(),
            HighlightShape.Ellipse => new Ellipse(),
            HighlightShape.Rectangle => new Rectangle(),
            _ => new RoundRectangle { CornerRadius = 10 },
        };

        contentGrid.Children.Insert(0, highlightBox);
        contentGrid.Children.Insert(1, coachmarkView);

    }

    private void ResetView()
    {
        cancelationTokenSource?.Cancel();
        cancelationTokenSource?.Dispose();
        cancelationTokenSource = new();
        contentGrid?.Children.Clear();
    }

    private Grid SetupContainer()
    {
        contentGrid = new Grid
        {
            BackgroundColor = Colors.Black.WithAlpha(0.8f),
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill
        };

        contentGrid.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                currentIndex++;
                var view = _views.ElementAtOrDefault(currentIndex);
                if (view is View currentView)
                    DrawCoachMarkForView(currentView);
                else
                    await this.CloseAsync();
            })
        });
        return contentGrid;
    }
}