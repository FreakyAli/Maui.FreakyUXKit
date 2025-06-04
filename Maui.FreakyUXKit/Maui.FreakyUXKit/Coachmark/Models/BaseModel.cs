using SkiaSharp;

namespace Maui.FreakyUXKit.Coachmark.Models;

internal class BaseModel
{
    public View Target { get; set; }
    public SKRect Rect { get; set; }
    public float MidX { get; set; }
    public float MidY { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Radius { get; set; }
}