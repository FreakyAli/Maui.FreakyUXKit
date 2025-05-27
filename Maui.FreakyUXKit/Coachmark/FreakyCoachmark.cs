namespace Maui.FreakyUXKit;

public static class FreakyCoachmark
{
    private static Dictionary<Page, List<View>> _registeredCoachmarkViews = [];

    public static readonly BindableProperty HighlightShapeProperty =
    BindableProperty.CreateAttached(
        "HighlightShape",
        typeof(HighlightShape),
        typeof(View),
        HighlightShape.RoundedRectangle);

    public static HighlightShape GetHighlightShape(BindableObject view) =>
    (HighlightShape)view.GetValue(HighlightShapeProperty);

    public static void SetHighlightShape(BindableObject view, HighlightShape value) =>
    view.SetValue(HighlightShapeProperty, value);

    #region CoachmarkOverlayView

    public static readonly BindableProperty OverlayViewProperty =
        BindableProperty.CreateAttached(
            "OverlayView",
            typeof(IView),
            typeof(FreakyCoachmark),
            null);

    public static IView GetOverlayView(BindableObject view) =>
        (IView)view.GetValue(OverlayViewProperty);

    public static void SetOverlayView(BindableObject view, IView value) =>
        view.SetValue(OverlayViewProperty, value);

    #endregion

    #region AreCoachmarksEnabled

    public static BindableProperty AreCoachmarksEnabledProperty =
        BindableProperty.CreateAttached(
            "AreCoachmarksEnabled",
            typeof(bool),
            typeof(Page),
            false,
            propertyChanged: OnAreCoachmarksEnabledChanged);

    public static bool GetAreCoachmarksEnabled(BindableObject view) =>
        (bool)view.GetValue(AreCoachmarksEnabledProperty);

    public static void SetAreCoachmarksEnabled(BindableObject view, bool value) =>
        view.SetValue(AreCoachmarksEnabledProperty, value);

    private static void OnAreCoachmarksEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Page page)
            return;

        if (newValue is bool enabled && enabled)
            page.Loaded += OnPageAppearing;
        else
            page.Loaded -= OnPageAppearing;
    }

    #endregion


    #region DisplayOrder

    public static readonly BindableProperty DisplayOrderProperty =
        BindableProperty.CreateAttached(
            "DisplayOrder",
            typeof(int),
            typeof(FreakyCoachmark),
            int.MaxValue,
            propertyChanged: OnDisplayOrderChanged);

    public static int GetDisplayOrder(BindableObject view) =>
        (int)view.GetValue(DisplayOrderProperty);

    public static void SetDisplayOrder(BindableObject view, int value) =>
        view.SetValue(DisplayOrderProperty, value);

    private static void OnDisplayOrderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not View targetView)
            return;

        var page = GetOwningPage(targetView);
        if (page is null)
            return;

        if (!_registeredCoachmarkViews.ContainsKey(page))
            _registeredCoachmarkViews.Add(page, []);

        if (!_registeredCoachmarkViews[page].Contains(targetView))
            _registeredCoachmarkViews[page].Add(targetView);
    }

    #endregion

    #region OwningPage

    public static readonly BindableProperty OwningPageProperty =
        BindableProperty.CreateAttached(
            "OwningPage",
            typeof(Page),
            typeof(FreakyCoachmark),
            null,
            propertyChanged: OnDisplayOrderChanged);

    public static Page GetOwningPage(BindableObject view) =>
        (Page)view.GetValue(OwningPageProperty);

    public static void SetOwningPage(BindableObject view, Page value) =>
        view.SetValue(OwningPageProperty, value);

    #endregion

    internal static List<View> GetRegisteredCoachmarksForPage(Page page) =>
        _registeredCoachmarkViews.TryGetValue(page, out var list) ? list : [];

    private static async void OnPageAppearing(object? sender, EventArgs e)
    {
        if (sender is not Page page)
            return;

        if (!_registeredCoachmarkViews.ContainsKey(page))
            return;

        var coachMarkViews = _registeredCoachmarkViews[page]
            .OrderBy(GetDisplayOrder)
            .ToList();

        await CoachmarkManager.PresentCoachmarksAsync(coachMarkViews);
        page.Loaded -= OnPageAppearing;
    }
}