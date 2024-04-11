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
    /// <param name="direction">The direction to change the Left location,
    /// <para>
    /// true to increase; otherwise, false.
    /// </para>
    /// </param>
    public SecondaryFirework(Firework firework, float distance, bool direction) 
        : base(firework.Location, Vector.Zero)
    {
        _baseX = firework.Location.X;
        _distance = distance;
        Delta = new(direction ? -5 : 5, 15);
        Color = SetAlpha(firework.Color, firework.Color.Alpha / 2);
    }

    /// <summary>
    /// Determines if the <see cref="SecondaryFirework"/> is done animating.
    /// </summary>
    /// <value>true if the <see cref="SecondaryFirework"/> is done animating; otherwise, false.</value>
    public override bool IsDone
    {
        get => Math.Abs(Location.X - _baseX) >= _distance;
    }

    /// <summary>
    /// Updates the <see cref="SecondaryFirework"/> for rendering.
    /// </summary>
    /// <param name="elapsed">The time since the last update; in milliseconds.</param>
    protected override void OnUpdate(ParticleCollection particles, double elapsed)
    {
        Location = new Vector(Location.X + Delta.X, Location.Y + Delta.Y);
        Delta = new(Delta.X, Delta.Y + Gravity);
    }

    /// <summary>
    /// Explodes the <see cref="SecondaryFirework"/>.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to update.</param>
    public void Explode(ParticleCollection particles)
    {
        Spark.AddSparks(particles, Color, Location);
    }

    /// <summary>
    /// Renders the <see cref="Firework"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    protected override void OnRender(SKCanvas canvas, SKPaint paint)
    {
        Draw(canvas, paint, Color, SizeMetric * 0.8f);
    }
}
