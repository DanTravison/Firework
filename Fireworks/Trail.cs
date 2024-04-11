using SkiaSharp;

namespace FireworkExperiment.Fireworks;

internal class Trail : Particle
{
    #region Fields

    /// <summary>
    /// The maximum <see cref="Particle.Age"/>.
    /// </summary>
    const double MaximumAge = 3.0;

    /// <summary>
    /// The <see cref="Particle.Age"/> threshold to reach before the color starts to fade.
    /// </summary>
    const double FadeThreshold = .75;

    /// <summary>
    /// The previous <see cref="Particle.Location"/>.
    /// </summary>
    readonly Vector _previous;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="location">The <see cref="Vector"/> for the current <see cref="Particle.Location"/>.</param>
    /// <param name="previous">The <see cref="Vector"/> for the previous location.</param>
    public Trail(Vector location, Vector previous)
        : base (location, Vector.Zero, MaximumAge)
    {
        _previous = previous;
        Color = SKColors.Gainsboro;
    }

    /// <summary>
    /// Determines if the <see cref="Trail"/> is done animating.
    /// </summary>
    /// <value>
    /// true if the <see cref="Trail"/> is done animating; otherwise, false.
    /// </value>
    public override bool IsDone
    {
        get => Age >= Lifetime;
    }

    /// <summary>
    /// Updates the <see cref="Trail"/> for rendering.
    /// </summary>
    /// <param name="elapsed">The time since the last update; in milliseconds.</param>
    protected override void OnUpdate(ParticleCollection particles, double elapsed)
    {
        // NOTE: not adjusting for location.
        Color = Fade(FadeThreshold, MaximumAge);
    }

    /// <summary>
    /// Renders the <see cref="Spark"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    protected override void OnRender(SKCanvas canvas, SKPaint paint)
    {
        Draw(canvas, paint, Color, 1);
    }

    /// <summary>
    /// Draws the <see cref="Trail"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    /// <param name="color">The <see cref="SKColor"/> to use to draw.</param>
    /// <param name="size">The number of items to draw, not used.</param>
    protected override void Draw(SKCanvas canvas, SKPaint paint, SKColor color, float size)
    {
        paint.Color = color;
        paint.StrokeWidth = 1;
        canvas.DrawLine(Location.X, Location.Y, _previous.X, _previous.Y, paint);
    }
}
