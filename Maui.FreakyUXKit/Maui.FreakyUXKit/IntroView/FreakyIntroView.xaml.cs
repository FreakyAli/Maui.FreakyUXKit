using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Maui.FreakyUXKit;

public partial class FreakyIntroView
{
	private float animationProgress = 0f;
	private float animationRadius = 0f;
	private float maxRadius = 0f;
	private SKColor animationColor = SKColors.Transparent;
	private bool isAnimating = false;
	private readonly List<Particle> particles = new();
	private float wavePhase = 0f;
	private DateTime animationStartTime;
	private float rippleRadius = 0f;
	private float rippleMaxRadius = 0f;
	private SKColor rippleColor = SKColors.Transparent;
	private bool isAnimatingRipple = false;

	private class Particle
	{
		public SKPoint Position { get; set; }
		public SKPoint Velocity { get; set; }
		public float Life { get; set; } = 1f;
		public float Size { get; set; }
		public SKColor Color { get; set; }
	}

	public FreakyIntroView()
	{
		InitializeComponent();
	}

	public static readonly BindableProperty AnimationTypeProperty =
		BindableProperty.Create(
			nameof(AnimationType),
			typeof(IntroAnimationType),
			typeof(FreakyIntroView),
			IntroAnimationType.Ripple);
	public IntroAnimationType AnimationType
	{
		get => (IntroAnimationType)GetValue(AnimationTypeProperty);
		set => SetValue(AnimationTypeProperty, value);
	}

	// Animation Duration Property
	public static readonly BindableProperty AnimationDurationProperty =
		BindableProperty.Create(
			nameof(AnimationDuration),
			typeof(int),
			typeof(FreakyIntroView),
			2000);
	public int AnimationDuration
	{
		get => (int)GetValue(AnimationDurationProperty);
		set => SetValue(AnimationDurationProperty, value);
	}

	// Animation Speed Property
	public static readonly BindableProperty AnimationSpeedProperty =
		BindableProperty.Create(
			nameof(AnimationSpeed),
			typeof(double),
			typeof(FreakyIntroView),
			1.0);
	public double AnimationSpeed
	{
		get => (double)GetValue(AnimationSpeedProperty);
		set => SetValue(AnimationSpeedProperty, value);
	}

	public static readonly BindableProperty ItemsSourceProperty =
		BindableProperty.Create(
			nameof(ItemsSource),
			typeof(IEnumerable<FreakyIntroStep>),
			typeof(FreakyIntroView),
			default(IEnumerable<FreakyIntroStep>));
	public IEnumerable<FreakyIntroStep> ItemsSource
	{
		get => (IEnumerable<FreakyIntroStep>)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public static readonly BindableProperty PositionProperty =
		BindableProperty.Create(
			nameof(Position),
			typeof(int),
			typeof(FreakyIntroView),
			default(int));
	public int Position
	{
		get => (int)GetValue(PositionProperty);
		set => SetValue(PositionProperty, value);
	}

	public static readonly BindableProperty IsSwipeEnabledProperty =
		BindableProperty.Create(
			nameof(IsSwipeEnabled),
			typeof(bool),
			typeof(FreakyIntroView),
			true);
	public bool IsSwipeEnabled
	{
		get => (bool)GetValue(IsSwipeEnabledProperty);
		set => SetValue(IsSwipeEnabledProperty, value);
	}

	public static readonly BindableProperty IsScrollAnimatedProperty =
		BindableProperty.Create(
			nameof(IsScrollAnimated),
			typeof(bool),
			typeof(FreakyIntroView),
			false);
	public bool IsScrollAnimated
	{
		get => (bool)GetValue(IsScrollAnimatedProperty);
		set => SetValue(IsScrollAnimatedProperty, value);
	}

	public static readonly BindableProperty ImageSourceProperty =
		BindableProperty.Create(
			nameof(ImageSource),
			typeof(ImageSource),
			typeof(FreakyIntroView),
			default);
	public ImageSource ImageSource
	{
		get => (ImageSource)GetValue(ImageSourceProperty);
		set => SetValue(ImageSourceProperty, value);
	}

	public static readonly BindableProperty ImageStyleProperty =
		BindableProperty.Create(
			nameof(ImageStyle),
			typeof(Style),
			typeof(FreakyIntroView),
			default(Style));
	public Style ImageStyle
	{
		get => (Style)GetValue(ImageStyleProperty);
		set => SetValue(ImageStyleProperty, value);
	}

	public static readonly BindableProperty TitleTextProperty =
		BindableProperty.Create(
			nameof(TitleText),
			typeof(string),
			typeof(FreakyIntroView),
			default(string));
	public string TitleText
	{
		get => (string)GetValue(TitleTextProperty);
		set => SetValue(TitleTextProperty, value);
	}

	public static readonly BindableProperty TitleFormattedTextProperty =
		BindableProperty.Create(
			nameof(TitleFormattedText),
			typeof(FormattedString),
			typeof(FreakyIntroView),
			default(FormattedString));

	public FormattedString TitleFormattedText
	{
		get => (FormattedString)GetValue(TitleFormattedTextProperty);
		set => SetValue(TitleFormattedTextProperty, value);
	}

	public static readonly BindableProperty TitleLabelStyleProperty =
		BindableProperty.Create(
			nameof(TitleLabelStyle),
			typeof(Style),
			typeof(FreakyIntroView),
			default(Style));
	public Style TitleLabelStyle
	{
		get => (Style)GetValue(TitleLabelStyleProperty);
		set => SetValue(TitleLabelStyleProperty, value);
	}

	public static readonly BindableProperty SubTitleTextProperty =
		BindableProperty.Create(
			nameof(SubTitleText),
			typeof(string),
			typeof(FreakyIntroView),
			default(string));
	public string SubTitleText
	{
		get => (string)GetValue(SubTitleTextProperty);
		set => SetValue(SubTitleTextProperty, value);
	}

	public static readonly BindableProperty SubtitleFormattedTextProperty =
		BindableProperty.Create(
			nameof(SubtitleFormattedText),
			typeof(FormattedString),
			typeof(FreakyIntroView),
			default(FormattedString));

	public FormattedString SubtitleFormattedText
	{
		get => (FormattedString)GetValue(SubtitleFormattedTextProperty);
		set => SetValue(SubtitleFormattedTextProperty, value);
	}

	public static readonly BindableProperty SubtitleLabelStyleProperty =
		BindableProperty.Create(
			nameof(SubtitleLabelStyle),
			typeof(Style),
			typeof(FreakyIntroView),
			default(Style));
	public Style SubtitleLabelStyle
	{
		get => (Style)GetValue(SubtitleLabelStyleProperty);
		set => SetValue(SubtitleLabelStyleProperty, value);
	}

	public static readonly BindableProperty LeftButtonTextProperty =
		BindableProperty.Create(
			nameof(LeftButtonText),
			typeof(string),
			typeof(FreakyIntroView),
			default(string));
	public string LeftButtonText
	{
		get => (string)GetValue(LeftButtonTextProperty);
		set => SetValue(LeftButtonTextProperty, value);
	}

	public static readonly BindableProperty LeftButtonCommandProperty =
		BindableProperty.Create(
			nameof(LeftButtonCommand),
			typeof(ICommand),
			typeof(FreakyIntroView),
			default(ICommand));
	public ICommand LeftButtonCommand
	{
		get => (ICommand)GetValue(LeftButtonCommandProperty);
		set => SetValue(LeftButtonCommandProperty, value);
	}

	public static readonly BindableProperty CenterButtonTextProperty =
		BindableProperty.Create(
			nameof(CenterButtonText),
			typeof(string),
			typeof(FreakyIntroView),
			default(string));
	public string CenterButtonText
	{
		get => (string)GetValue(CenterButtonTextProperty);
		set => SetValue(CenterButtonTextProperty, value);
	}

	public static readonly BindableProperty CenterButtonCommandProperty =
		BindableProperty.Create(
			nameof(CenterButtonCommand),
			typeof(ICommand),
			typeof(FreakyIntroView),
			default(ICommand));
	public ICommand CenterButtonCommand
	{
		get => (ICommand)GetValue(CenterButtonCommandProperty);
		set => SetValue(CenterButtonCommandProperty, value);
	}

	public static readonly BindableProperty RightButtonTextProperty =
		BindableProperty.Create(
			nameof(RightButtonText),
			typeof(string),
			typeof(FreakyIntroView),
			default(string));
	public string RightButtonText
	{
		get => (string)GetValue(RightButtonTextProperty);
		set => SetValue(RightButtonTextProperty, value);
	}

	public static readonly BindableProperty RightButtonCommandProperty =
		BindableProperty.Create(
			nameof(RightButtonCommand),
			typeof(ICommand),
			typeof(FreakyIntroView),
			default(ICommand));
	public ICommand RightButtonCommand
	{
		get => (ICommand)GetValue(RightButtonCommandProperty);
		set => SetValue(RightButtonCommandProperty, value);
	}

	public static readonly BindableProperty ButtonsDirectionProperty =
		BindableProperty.Create(
			nameof(ButtonsDirection),
			typeof(FlexDirection),
			typeof(FreakyIntroView),
			FlexDirection.Row);
	public FlexDirection ButtonsDirection
	{
		get => (FlexDirection)GetValue(ButtonsDirectionProperty);
		set => SetValue(ButtonsDirectionProperty, value);
	}

	public static readonly BindableProperty IndicatorColorProperty =
		BindableProperty.Create(
			nameof(IndicatorColor),
			typeof(Color),
			typeof(FreakyIntroView),
			Colors.White);
	public Color IndicatorColor
	{
		get => (Color)GetValue(IndicatorColorProperty);
		set => SetValue(IndicatorColorProperty, value);
	}

	public static readonly BindableProperty IndicatorsShapeProperty =
		BindableProperty.Create(
			nameof(IndicatorsShape),
			typeof(IndicatorShape),
			typeof(FreakyIntroView),
			IndicatorShape.Circle);
	public IndicatorShape IndicatorsShape
	{
		get => (IndicatorShape)GetValue(IndicatorsShapeProperty);
		set => SetValue(IndicatorsShapeProperty, value);
	}

	public static readonly BindableProperty SelectedIndicatorColorProperty =
		BindableProperty.Create(
			nameof(SelectedIndicatorColor),
			typeof(Color),
			typeof(FreakyIntroView),
			Colors.Transparent);
	public Color SelectedIndicatorColor
	{
		get => (Color)GetValue(SelectedIndicatorColorProperty);
		set => SetValue(SelectedIndicatorColorProperty, value);
	}

	public static readonly BindableProperty IndicatorMaximumVisibleProperty =
		BindableProperty.Create(
			nameof(IndicatorMaximumVisible),
			typeof(int),
			typeof(FreakyIntroView),
			3);
	public int IndicatorMaximumVisible
	{
		get => (int)GetValue(IndicatorMaximumVisibleProperty);
		set => SetValue(IndicatorMaximumVisibleProperty, value);
	}

	public static readonly BindableProperty IndicatorSizeProperty =
		BindableProperty.Create(
			nameof(IndicatorSize),
			typeof(double),
			typeof(FreakyIntroView),
			10.0);
	public double IndicatorSize
	{
		get => (double)GetValue(IndicatorSizeProperty);
		set => SetValue(IndicatorSizeProperty, value);
	}

	// Event to notify when the position changes
	// Changed from int to PositionChangedEventArgs for more detailed information
	public event EventHandler<PositionChangedEventArgs> PositionChanged;

	private void CarouselView_PositionChanged(object sender, PositionChangedEventArgs e)
	{
		PositionChanged?.Invoke(this, e);
		var steps = this.ItemsSource.ElementAtOrDefault(e.CurrentPosition);
		rippleColor = steps.BackgroundAnimationColor.ToSKColor();
		// Use original ripple for Ripple type, new system for others
		if (AnimationType == IntroAnimationType.Ripple)
		{
			StartRipple(steps.BackgroundAnimationColor);
		}
		else
		{
			StartAnimation(steps.BackgroundAnimationColor);
		}
	}

	public void StartRipple(Color color)
	{
		var canvasSize = canvasView.CanvasSize;
		rippleColor = color.ToSKColor();
		rippleRadius = 0;
		rippleMaxRadius = (float)Math.Sqrt(canvasSize.Width * canvasSize.Width + canvasSize.Height * canvasSize.Height);
		isAnimatingRipple = true;

		Dispatcher.StartTimer(TimeSpan.FromMilliseconds(16), () =>
		{
			rippleRadius += 30 * (float)AnimationSpeed; // Apply speed multiplier
			canvasView.InvalidateSurface();

			if (rippleRadius >= rippleMaxRadius)
			{
				isAnimatingRipple = false;
				BackgroundColor = color; // set final BG
				canvasView.InvalidateSurface(); // clear ripple
				return false; // stop timer
			}

			return true;
		});
	}

	public void StartAnimation(Color color)
	{
		if (AnimationType == IntroAnimationType.None) return;

		var canvasSize = canvasView.CanvasSize;
		animationColor = color.ToSKColor();
		animationProgress = 0f;
		animationRadius = 0f;
		maxRadius = (float)Math.Sqrt(canvasSize.Width * canvasSize.Width + canvasSize.Height * canvasSize.Height);
		isAnimating = true;
		animationStartTime = DateTime.Now;

		// Initialize particles for particle animation
		if (AnimationType == IntroAnimationType.Particle)
		{
			InitializeParticles(canvasSize);
		}

		var updateInterval = TimeSpan.FromMilliseconds(16); // 60 FPS
		Dispatcher.StartTimer(updateInterval, () =>
		{
			var elapsed = (DateTime.Now - animationStartTime).TotalMilliseconds;
			animationProgress = Math.Min(1f, (float)(elapsed / (AnimationDuration / AnimationSpeed)));

			UpdateAnimation();
			canvasView.InvalidateSurface();

			if (animationProgress >= 1f)
			{
				CompleteAnimation(color);
				return false;
			}

			return true;
		});
	}

	private void InitializeParticles(SKSize canvasSize)
	{
		particles.Clear();
		var random = new Random();
		var particleCount = 50;

		for (int i = 0; i < particleCount; i++)
		{
			particles.Add(new Particle
			{
				Position = new SKPoint(canvasSize.Width / 2, canvasSize.Height),
				Velocity = new SKPoint(
					(float)(random.NextDouble() - 0.5) * 200,
					(float)(random.NextDouble() * -300 - 100)),
				Size = (float)(random.NextDouble() * 8 + 2),
				Color = animationColor,
				Life = 1f
			});
		}
	}

	private void UpdateAnimation()
	{
		switch (AnimationType)
		{
			case IntroAnimationType.Ripple:
				animationRadius = maxRadius * EaseOutCubic(animationProgress);
				break;

			case IntroAnimationType.Particle:
				UpdateParticles();
				break;

			case IntroAnimationType.Wave:
				wavePhase += 0.1f * (float)AnimationSpeed;
				break;
		}
	}

	private void UpdateParticles()
	{
		for (int i = particles.Count - 1; i >= 0; i--)
		{
			var particle = particles[i];
			particle.Position = new SKPoint(
				particle.Position.X + particle.Velocity.X * 0.016f,
				particle.Position.Y + particle.Velocity.Y * 0.016f);
			particle.Life -= 0.02f * (float)AnimationSpeed;

			if (particle.Life <= 0)
				particles.RemoveAt(i);
		}
	}

	private void CompleteAnimation(Color color)
	{
		isAnimating = false;
		BackgroundColor = color;
		particles.Clear();
		canvasView.InvalidateSurface();
	}

	private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
	{
		var canvas = e.Surface.Canvas;
		var info = e.Info;
		canvas.Clear();

		// Handle original ripple animation
		if (isAnimatingRipple)
		{
			var center = new SKPoint(info.Width / 2, info.Height); // bottom center

			using var paint = new SKPaint
			{
				Color = rippleColor,
				Style = SKPaintStyle.Fill,
				IsAntialias = true
			};

			canvas.DrawCircle(center, rippleRadius, paint);
			return;
		}

		// Handle new animation types
		if (!isAnimating) return;

		switch (AnimationType)
		{
			case IntroAnimationType.Fade:
				DrawFade(canvas, info);
				break;
			case IntroAnimationType.Slide:
				DrawSlide(canvas, info);
				break;
			case IntroAnimationType.Scale:
				DrawScale(canvas, info);
				break;
			case IntroAnimationType.Rotate:
				DrawRotate(canvas, info);
				break;
			case IntroAnimationType.Bounce:
				DrawBounce(canvas, info);
				break;
			case IntroAnimationType.Particle:
				DrawParticles(canvas, info);
				break;
			case IntroAnimationType.Wave:
				DrawWave(canvas, info);
				break;
		}
	}

	private void DrawRipple(SKCanvas canvas, SKImageInfo info)
	{
		var center = new SKPoint(info.Width / 2, info.Height);
		using var paint = new SKPaint
		{
			Color = animationColor.WithAlpha((byte)(255 * (1 - animationProgress))),
			Style = SKPaintStyle.Fill,
			IsAntialias = true
		};
		canvas.DrawCircle(center, animationRadius, paint);
	}

	private void DrawFade(SKCanvas canvas, SKImageInfo info)
	{
		using var paint = new SKPaint
		{
			Color = animationColor.WithAlpha((byte)(255 * animationProgress)),
			Style = SKPaintStyle.Fill
		};
		canvas.DrawRect(0, 0, info.Width, info.Height, paint);
	}

	private void DrawSlide(SKCanvas canvas, SKImageInfo info)
	{
		var slideOffset = info.Width * (1 - EaseOutCubic(animationProgress));
		using var paint = new SKPaint { Color = animationColor, Style = SKPaintStyle.Fill };
		canvas.DrawRect(slideOffset, 0, info.Width, info.Height, paint);
	}

	private void DrawScale(SKCanvas canvas, SKImageInfo info)
	{
		var scale = EaseOutBack(animationProgress);
		var center = new SKPoint(info.Width / 2, info.Height / 2);
		var size = Math.Max(info.Width, info.Height) * scale;

		canvas.Save();
		canvas.Translate(center.X, center.Y);
		canvas.Scale(scale, scale);

		using var paint = new SKPaint { Color = animationColor, Style = SKPaintStyle.Fill };
		canvas.DrawCircle(0, 0, size / 2, paint);
		canvas.Restore();
	}

	private void DrawRotate(SKCanvas canvas, SKImageInfo info)
	{
		var rotation = 360 * animationProgress;
		var center = new SKPoint(info.Width / 2, info.Height / 2);

		canvas.Save();
		canvas.RotateDegrees(rotation, center.X, center.Y);

		using var paint = new SKPaint { Color = animationColor, Style = SKPaintStyle.Fill };
		canvas.DrawRect(0, 0, info.Width, info.Height, paint);
		canvas.Restore();
	}

	private void DrawBounce(SKCanvas canvas, SKImageInfo info)
	{
		var bounceHeight = info.Height * EaseOutBounce(animationProgress);
		using var paint = new SKPaint { Color = animationColor, Style = SKPaintStyle.Fill };
		canvas.DrawRect(0, info.Height - bounceHeight, info.Width, bounceHeight, paint);
	}

	private void DrawParticles(SKCanvas canvas, SKImageInfo info)
	{
		using var paint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true };

		foreach (var particle in particles)
		{
			paint.Color = particle.Color.WithAlpha((byte)(255 * particle.Life));
			canvas.DrawCircle(particle.Position, particle.Size, paint);
		}
	}

	private void DrawWave(SKCanvas canvas, SKImageInfo info)
	{
		using var paint = new SKPaint { Color = animationColor, Style = SKPaintStyle.Fill };
		using var path = new SKPath();

		var waveHeight = info.Height * animationProgress;
		var amplitude = 50f;
		var frequency = 0.01f;

		path.MoveTo(0, info.Height);

		for (int x = 0; x <= info.Width; x += 5)
		{
			var y = info.Height - waveHeight + amplitude * (float)Math.Sin(frequency * x + wavePhase);
			path.LineTo(x, y);
		}

		path.LineTo(info.Width, info.Height);
		path.Close();
		canvas.DrawPath(path, paint);
	}

	// Easing functions
	private static float EaseOutCubic(float t) => 1f - (float)Math.Pow(1f - t, 3);
	private static float EaseOutBack(float t)
	{
		const float c1 = 1.70158f;
		const float c3 = c1 + 1f;
		return 1f + c3 * (float)Math.Pow(t - 1f, 3) + c1 * (float)Math.Pow(t - 1f, 2);
	}
	private static float EaseOutBounce(float t)
	{
		const float n1 = 7.5625f;
		const float d1 = 2.75f;

		if (t < 1f / d1) return n1 * t * t;
		if (t < 2f / d1) return n1 * (t -= 1.5f / d1) * t + 0.75f;
		if (t < 2.5f / d1) return n1 * (t -= 2.25f / d1) * t + 0.9375f;
		return n1 * (t -= 2.625f / d1) * t + 0.984375f;
	}
}

public enum IntroAnimationType
{
	None,
	Ripple,
	Fade,
	Slide,
	Scale,
	Rotate,
	Bounce,
	Particle,
	Wave
}