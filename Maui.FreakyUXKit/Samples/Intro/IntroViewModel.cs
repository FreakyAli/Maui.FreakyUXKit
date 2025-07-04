using Maui.FreakyUXKit;

namespace Samples.Intro;

public partial class IntroViewModel : FreakyBaseViewModel
{
    public List<FreakyIntroStep> Steps { get; set; }
    public IntroViewModel()
    {
        Steps =
        [
            new() {
                TitleText = "Products from worldwide",
                SubTitleText = "Its a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.",
                ImageSource = "people_on_bike.png",
                LeftButtonText = "Skip",
                CenterButtonText = "Next",
                RightButtonText = "Learn More",
                TitleLabelStyle = Application.Current.Resources.GetResource<Style>("PrimaryLabelStyle"),
                SubtitleLabelStyle = Application.Current.Resources.GetResource<Style>("PrimaryLabelStyle"),
                BackgroundAnimationColor = Application.Current.Resources.GetResource<Color>("Yellow"),
                Background = Application.Current.Resources.GetResource<Color>("Orange")
            },
            new() {
                TitleText = "Buy from anywhere",
                SubTitleText = "Its a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.",
                ImageSource = "people_together.png",
                LeftButtonText = "Skip",
                CenterButtonText = "Next",
                RightButtonText = "Learn More",
                TitleLabelStyle = Application.Current.Resources.GetResource<Style>("PrimaryLabelStyle"),
                SubtitleLabelStyle = Application.Current.Resources.GetResource<Style>("PrimaryLabelStyle"),
                BackgroundAnimationColor = Application.Current.Resources.GetResource<Color>("Yellow")
            },
            new() {
                TitleText = "Buy from anywhere",
                SubTitleText = "Its a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.",
                ImageSource = "person_with_pet.png",
                LeftButtonText = "Skip",
                CenterButtonText = "Next",
                RightButtonText = "Learn More",
                TitleLabelStyle = Application.Current.Resources.GetResource<Style>("PrimaryLabelStyle"),
                SubtitleLabelStyle = Application.Current.Resources.GetResource<Style>("PrimaryLabelStyle"),
                BackgroundAnimationColor = Application.Current.Resources.GetResource<Color>("Orange")
            }
        ];
    }
}
