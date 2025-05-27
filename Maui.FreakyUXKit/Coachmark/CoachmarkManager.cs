using CommunityToolkit.Maui.Views;

namespace Maui.FreakyUXKit;

internal class CoachmarkManager
{
    internal static async Task PresentCoachmarksAsync(IList<View> coachMarkViews)
    {
        var tutorialPage = new FreakyPopup(coachMarkViews.OrderBy(FreakyCoachmark.GetDisplayOrder));
        await Constants.MainPage?.ShowPopupAsync(tutorialPage);
    }
}