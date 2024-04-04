namespace FireworkExperiment;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

internal class CanvasView : SKCanvasView
{
    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
        canvas.Clear();
        base.OnPaintSurface(e);
    }
}
