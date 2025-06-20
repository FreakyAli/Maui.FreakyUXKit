using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;

namespace Maui.FreakyUXKit;

internal class CoachmarkManager
{
    internal static async Task PresentCoachmarksAsync(IList<View> coachMarkViews)
    {
        var tutorialPage = new FreakyPopupPage(coachMarkViews.OrderBy(FreakyCoachmark.GetDisplayOrder));
        var popupOptions = new PopupOptions
        {
            PageOverlayColor = Colors.Transparent,
            CanBeDismissedByTappingOutsideOfPopup = false,
            Shadow = null,
        };
        await Constants.MainPage.ShowPopupAsync(tutorialPage, popupOptions);
    }
}