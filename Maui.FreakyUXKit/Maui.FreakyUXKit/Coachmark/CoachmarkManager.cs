namespace Maui.FreakyUXKit;

internal class CoachmarkManager
{
    internal static async Task PresentCoachmarksAsync(IList<View> coachMarkViews)
    {
        var views = coachMarkViews.OrderBy(FreakyCoachmark.GetDisplayOrder).ToList();
        var mainPage = Constants.MainPage;
        if (mainPage is Shell)
        {
            var navigationParameter = new Dictionary<string, object> { { Constants.Coachmarks, views } };  
            await Shell.Current.GoToAsync(Constants.freakyPopup, false, navigationParameter);
        }
        else
        {
            var tutorialPage = new FreakyPopupPage(views);
            await mainPage.Navigation.PushModalAsync(tutorialPage, false);
        }
    }
}