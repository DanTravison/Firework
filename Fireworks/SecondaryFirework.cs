using SkiaSharp;

namespace FireworkExperiment.Fireworks;

/// <summary>
/// Provides a secondary firework.
/// </summary>
/// <remarks>
/// Concept: Allow a firework to randomly split and launch two or more secondary firework instances. 
/// Not ready for prime time.
/// </remarks>
internal class SecondaryFirework : Particle, IFirework
{
    float _distance;
    float _baseX;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="firework">The <see cref="Firework"/> launching this instance.</param>
    /// <param name="direction">The direction to change the X location,
    /// <para>
    /// true to increase; otherwise, false.
    /// </para>X axis direction.</param>
    public SecondaryFirework(Firework firework, float distance, bool direction) 
        : base(firework.X, firework.Y)
    {
        _baseX = firework.X;
        _distance = distance;
        AdjustX = direction ? -5 : 5;
        AdjustY = 15;
        Color = SetAlpha(firework.Color, firework.Color.Alpha / 2);
    }

    /// <summary>
    /// Determines if the <see cref="SecondaryFirework"/> is done animating.
    /// </summary>
    /// <value>true if the <see cref="SecondaryFirework"/> is done animating; otherwise, false.</value>
    public override bool IsDone
    {
        get => Math.Abs(X - _baseX) >= _distance;
    }

    /// <summary>
    /// Updates the <see cref="SecondaryFirework"/> for rendering.
    /// </summary>
    public override void Update(ParticleCollection particles)
    {
        Y += AdjustY;
        X += AdjustX;
        AdjustY += Gravity;
    }

    /// <summary>
    /// Explodes the <see cref="SecondaryFirework"/>.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to update.</param>
    public void Explode(ParticleCollection particles)
    {
        Spark.AddSparks(particles, Color, X, Y);
    }

    /// <summary>
    /// Renders the <see cref="Firework"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    protected override void OnRender(SKCanvas canvas, SKPaint paint)
    {
        Draw(canvas, paint, Color, Meter * 0.8f);
    }
}
