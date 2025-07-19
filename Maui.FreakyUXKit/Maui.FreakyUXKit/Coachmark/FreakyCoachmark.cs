using SkiaSharp;

namespace Maui.FreakyUXKit;

public static class FreakyCoachmark
{
    private static Dictionary<Page, List<View>> _registeredCoachmarkViews = [];

    #region ArrowStrokeWidth
    public static readonly BindableProperty ArrowStrokeWidthProperty =
        BindableProperty.CreateAttached(
            "ArrowStrokeWidth",
            typeof(float),
            typeof(FreakyCoachmark),
            2.0f);
    public static float GetArrowStrokeWidth(BindableObject view) =>
        (float)view.GetValue(ArrowStrokeWidthProperty);
    public static void SetArrowStrokeWidth(BindableObject view, float value) =>
        view.SetValue(ArrowStrokeWidthProperty, value);
    #endregion

    #region ArrowStyle
    public static readonly BindableProperty ArrowStyleProperty =
        BindableProperty.CreateAttached(
            "ArrowStyle",
            typeof(ArrowStyle),
            typeof(FreakyCoachmark),
            ArrowStyle.Default);
    public static ArrowStyle GetArrowStyle(BindableObject view) =>
        (ArrowStyle)view.GetValue(ArrowStyleProperty);
    public static void SetArrowStyle(BindableObject view, ArrowStyle value) =>
        view.SetValue(ArrowStyleProperty, value);
    #endregion

    #region ArrowColor
    public static readonly BindableProperty ArrowColorProperty =
        BindableProperty.CreateAttached(
            "ArrowColor",
            typeof(Color),
            typeof(FreakyCoachmark),
            Colors.Red);
    public static Color GetArrowColor(BindableObject view) =>
        (Color)view.GetValue(ArrowColorProperty);
    public static void SetArrowColor(BindableObject view, Color value) =>
        view.SetValue(ArrowColorProperty, value);
    #endregion

    #region FocusAnimationColor
    public static readonly BindableProperty FocusAnimationColorProperty =
        BindableProperty.CreateAttached(
            "FocusAnimationColor",
            typeof(Color),
            typeof(FreakyCoachmark),
            Constants.focusAnimationColor);
    public static Color GetFocusAnimationColor(BindableObject view) =>
        (Color)view.GetValue(FocusAnimationColorProperty);
    public static void SetFocusAnimationColor(BindableObject view, Color value) =>
        view.SetValue(FocusAnimationColorProperty, value);
    #endregion

    #region HighlightPadding
    public static readonly BindableProperty HighlightPaddingProperty =
        BindableProperty.CreateAttached(
            "HighlightPadding",
            typeof(float),
            typeof(FreakyCoachmark),
            0.0f);
            
    public static float GetHighlightPadding(BindableObject view) =>
        (float)view.GetValue(HighlightPaddingProperty);
        
    public static void SetHighlightPadding(BindableObject view, float value) =>
        view.SetValue(HighlightPaddingProperty, value);
    #endregion

    #region PreferredPosition
    public static readonly BindableProperty PreferredPositionProperty =
        BindableProperty.CreateAttached(
            "PreferredPosition",
            typeof(CoachmarkPosition),
            typeof(FreakyCoachmark),
            CoachmarkPosition.Auto);
            
    public static CoachmarkPosition GetPreferredPosition(BindableObject view) =>
        (CoachmarkPosition)view.GetValue(PreferredPositionProperty);
        
    public static void SetPreferredPosition(BindableObject view, CoachmarkPosition value) =>
        view.SetValue(PreferredPositionProperty, value);
    #endregion

    #region OverlayMargin
    public static readonly BindableProperty OverlayMarginProperty =
        BindableProperty.CreateAttached(
            "OverlayMargin",
            typeof(float),
            typeof(FreakyCoachmark),
            10.0f);
            
    public static float GetOverlayMargin(BindableObject view) =>
        (float)view.GetValue(OverlayMarginProperty);
        
    public static void SetOverlayMargin(BindableObject view, float value) =>
        view.SetValue(OverlayMarginProperty, value);
    #endregion

    #region HighlightShapeCornerRadius

    public static readonly BindableProperty HighlightShapeCornerRadiusProperty =
        BindableProperty.CreateAttached(
            "HighlightShapeCornerRadius",
            typeof(float),
            typeof(FreakyCoachmark),
            10.0f);
    public static float GetHighlightShapeCornerRadius(BindableObject view) =>  
        (float)view.GetValue(HighlightShapeCornerRadiusProperty);      

    public static void SetHighlightShapeCornerRadius(BindableObject view, float value) =>
        view.SetValue(HighlightShapeCornerRadiusProperty, value);

    #endregion

    #region CoachmarkAnimation
    public static readonly BindableProperty CoachmarkAnimationProperty =
        BindableProperty.CreateAttached(
            "CoachmarkAnimation",
            typeof(CoachmarkAnimationStyle),
            typeof(FreakyCoachmark),
            CoachmarkAnimationStyle.Spotlight);
    public static CoachmarkAnimationStyle GetCoachmarkAnimation(BindableObject view) =>
        (CoachmarkAnimationStyle)view.GetValue(CoachmarkAnimationProperty);
    public static void SetCoachmarkAnimation(BindableObject view, CoachmarkAnimationStyle value) =>
        view.SetValue(CoachmarkAnimationProperty, value);
    #endregion

    #region HighlightShape
    public static readonly BindableProperty HighlightShapeProperty =
    BindableProperty.CreateAttached(
        "HighlightShape",
        typeof(HighlightShape),
        typeof(View),
        HighlightShape.RoundRectangle);

    public static HighlightShape GetHighlightShape(BindableObject view) =>
    (HighlightShape)view.GetValue(HighlightShapeProperty);

    public static void SetHighlightShape(BindableObject view, HighlightShape value) =>
    view.SetValue(HighlightShapeProperty, value);

    #endregion

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
            page.Loaded += OnPageLoaded;
        else
            page.Loaded -= OnPageLoaded;
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

    private static async void OnPageLoaded(object? sender, EventArgs e)
    {
        if (sender is not Page page)
            return;

        if (!_registeredCoachmarkViews.ContainsKey(page))
            return;

        var coachMarkViews = _registeredCoachmarkViews[page]
            .OrderBy(GetDisplayOrder)
            .ToList();

        await CoachmarkManager.PresentCoachmarksAsync(coachMarkViews);
        page.Loaded -= OnPageLoaded;
    }
}