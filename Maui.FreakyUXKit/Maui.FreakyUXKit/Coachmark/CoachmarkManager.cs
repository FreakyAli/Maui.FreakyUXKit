using CommunityToolkit.Maui.Views;

namespace Maui.FreakyUXKit;

internal class CoachmarkManager
{
    internal static async Task PresentCoachmarksAsync(IList<View> coachMarkViews)
    {
        var views = coachMarkViews.OrderBy(FreakyCoachmark.GetDisplayOrder).ToList();
        var tutorialPage = new FreakyPopupPage(views);
        var mainPage = Constants.MainPage;
        if (mainPage is Shell)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(tutorialPage);
        }
        else
        {
            await mainPage.ShowPopupAsync(tutorialPage);
        }
    }
}