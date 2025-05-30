namespace Maui.FreakyUXKit;

internal class CoachmarkManager
{
    internal static async Task PresentCoachmarksAsync(IList<View> coachMarkViews)
    {
        var tutorialPage = new FreakyPopupPage(coachMarkViews.OrderBy(FreakyCoachmark.GetDisplayOrder));
        await Constants.MainPage?.Navigation.PushModalAsync(tutorialPage, false);
    }
}