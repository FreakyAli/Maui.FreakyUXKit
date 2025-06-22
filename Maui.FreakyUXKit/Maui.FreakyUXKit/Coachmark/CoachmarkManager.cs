using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;

namespace Maui.FreakyUXKit;

internal class CoachmarkManager
{
    internal static async Task PresentCoachmarksAsync(IList<View> coachMarkViews)
    {
        var oderedCoachMarkViews = coachMarkViews.OrderBy(FreakyCoachmark.GetDisplayOrder).ToList();
        var tutorialPage = new FreakyPopupPage(oderedCoachMarkViews);
        var popupOptions = new PopupOptions
        {
            PageOverlayColor = Colors.Transparent,
            CanBeDismissedByTappingOutsideOfPopup = false,
            Shadow = null,
        };
        await Constants.MainPage.ShowPopupAsync(tutorialPage, popupOptions);
    }
}