namespace FireworkExperiment;

using CodeCadence.Maui.Fireworks;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

internal class AnimationView : SKCanvasView
{
    FireworkAnimation _animation;

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
        canvas.Clear();

        Firework?.Draw(canvas, CanvasSize);
    }

    public FireworkAnimation Firework 
    { 
        get => _animation;
        set
        {
            _animation = value;
        }
    }
}
