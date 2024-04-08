using SkiaSharp;

namespace FireworkExperiment.Fireworks;

internal class Trail : Particle
{
    const int TrailLifetime = 30;
    readonly float _dx;
    readonly float _dy;
    int _age;

    public Trail(float x, float y, float dx, float dy)
        : base (x, y)
    {
        _dx = dx;
        _dy = dy;
        Color = SKColors.Gainsboro;
    }

    /// <summary>
    /// Updates the <see cref="Firework"/> for rendering.
    /// </summary>
    /// <param name="particles">Not used.</param>
    public override void Update(ParticleCollection particles)
    {
        _age++;
        if (_age < TrailLifetime)
        {
            int delta = TrailLifetime - _age;
            int alpha = 255 - 255 / delta;
            Color = SetAlpha(Color, alpha);
        }
    }

    /// <summary>
    /// Determines if the <see cref="Trail"/> is done animating.
    /// </summary>
    /// <value>
    /// true if the <see cref="Trail"/> is done animating; otherwise, false.
    /// </value>
    public override bool IsDone
    {
        get => _age >= TrailLifetime;
    }

    protected override void OnRender(SKCanvas canvas, SKPaint paint)
    {
        Draw(canvas, paint, Color, 1);
    }

    protected override void Draw(SKCanvas canvas, SKPaint paint, SKColor color, float size)
    {
        paint.Color = color;
        paint.IsAntialias = true;
        canvas.DrawLine(X, Y, _dx, _dy, paint);
    }
}
