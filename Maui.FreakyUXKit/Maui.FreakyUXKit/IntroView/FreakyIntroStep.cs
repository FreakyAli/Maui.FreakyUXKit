using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;
namespace Maui.FreakyUXKit;

public class FreakyIntroStep
{
    public string TitleText { get; set; }
    public FormattedString TitleFormattedText { get; set; }
    public string SubTitleText { get; set; }
    public FormattedString SubtitleFormattedText { get; set; }
    public ImageSource ImageSource { get; set; }
    public Style ImageStyle { get; set; }
    public Style TitleLabelStyle { get; set; }
    public Style SubtitleLabelStyle { get; set; }
    public string LeftButtonText { get; set; }
    public ICommand LeftButtonCommand { get; set; }
    public bool IsLeftButtonVisible { get; set; }
    public string CenterButtonText { get; set; }
    public ICommand CenterButtonCommand { get; set; }
    public bool IsCenterButtonVisible { get; set; }
    public string RightButtonText { get; set; }
    public ICommand RightButtonCommand { get; set; }
    public bool IsRightButtonVisible { get; set; }
    public Brush BottomBackground { get; set; } = Colors.Transparent;
    public Brush ImageBackground { get; set; } = Colors.Transparent;
    public Brush Background { get; set; } = Brush.Transparent;
    public Color BackgroundAnimationColor { get; set; } = Colors.Transparent;
    public IShape BottomStrokeShape { get; set; } = new RoundRectangle() { CornerRadius = new CornerRadius(30, 30, 0, 0) };
}