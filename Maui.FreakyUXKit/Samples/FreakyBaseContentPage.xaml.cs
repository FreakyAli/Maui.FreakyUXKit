using System.Windows.Input;

namespace Samples;

public partial class FreakyBaseContentPage : ContentPage
{
	public FreakyBaseContentPage()
	{
		InitializeComponent();
		Shell.SetNavBarIsVisible(this, false);
	}

	public ICommand BackButtonCommand
	{
		get => (ICommand)GetValue(BackButtonCommandProperty);
		set => SetValue(BackButtonCommandProperty, value);
	}

	public Color NavBarBackgroundColor
	{
		get => (Color)GetValue(NavBarBackgroundColorProperty);
		set => SetValue(NavBarBackgroundColorProperty, value);
	}

	public Color HeaderTextColor
	{
		get => (Color)GetValue(HeaderTextColorProperty);
		set => SetValue(HeaderTextColorProperty, value);
	}

	public string HeaderText
	{
		get => (string)GetValue(HeaderTextProperty);
		set => SetValue(HeaderTextProperty, value);
	}

	public bool IsContextVisible
	{
		get => (bool)GetValue(IsContextVisibleProperty);
		set => SetValue(IsContextVisibleProperty, value);
	}

	public bool IsBackButtonVisible
	{
		get => (bool)GetValue(IsBackButtonVisibleProperty);
		set => SetValue(IsBackButtonVisibleProperty, value);
	}

	public static readonly BindableProperty BackButtonCommandProperty = BindableProperty.Create
		(
		propertyName: nameof(BackButtonCommand),
		returnType: typeof(ICommand),
		declaringType: typeof(FreakyBaseContentPage),
		defaultValue: null
		);

	public static readonly BindableProperty NavBarBackgroundColorProperty = BindableProperty.Create
		 (
		 propertyName: nameof(NavBarBackgroundColor),
		 returnType: typeof(Color),
		 declaringType: typeof(FreakyBaseContentPage),
		 defaultValue: Colors.Black
		 );

	public static readonly BindableProperty HeaderTextColorProperty = BindableProperty.Create
		  (
		  propertyName: nameof(HeaderTextColor),
		  returnType: typeof(Color),
		  declaringType: typeof(FreakyBaseContentPage),
		  defaultValue: Colors.White
		  );

	public static readonly BindableProperty HeaderTextProperty = BindableProperty.Create
		(
		propertyName: nameof(HeaderText),
		returnType: typeof(string),
		declaringType: typeof(FreakyBaseContentPage),
		defaultValue: string.Empty
		);

	public static readonly BindableProperty IsContextVisibleProperty = BindableProperty.Create
		(
		propertyName: nameof(IsContextVisible),
		returnType: typeof(bool),
		declaringType: typeof(FreakyBaseContentPage),
		defaultValue: true
		);

	public static readonly BindableProperty IsBackButtonVisibleProperty = BindableProperty.Create
		(
		propertyName: nameof(IsBackButtonVisible),
		returnType: typeof(bool),
		declaringType: typeof(FreakyBaseContentPage),
		defaultValue: true
		);
}