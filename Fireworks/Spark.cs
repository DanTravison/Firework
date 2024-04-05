namespace CodeCadence.Maui.Fireworks;

using SkiaSharp;

/// <summary>
/// Provides a spark <see cref="Particle"/>.
/// </summary>
internal class Spark : Particle
{
    const int SparkLife = 100;
    int _age;

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="x">The zero-base X coordinate.</param>
    /// <param name="y">The zero-base Y coordinate.</param>
    /// <param name="adjustX">The <see cref="Particle.X"/> adjustment amount..</param>
    /// <param name="adjustY">The <see cref="Particle.Y"/> adjustment amount.</param>
    /// <param name="color">The <see cref="SKColor"/> to use to draw the <see cref="Spark"/>.</param>
    public Spark(float x, float y, float adjustX, float adjustY, SKColor color)
        : base(x, y, adjustX, adjustY)
    {
        Color = color;
    }

    /// <summary>
    /// Gets the value indicating if the <see cref="Spark"/> is done.
    /// </summary>
    /// <returns>true if the <see cref="Spark"/> is done; otherwise, false.</returns>
    public override bool IsDone()
    {
        return _age > SparkLife;
    }

    /// <summary>
    /// Updates the particle for rendering.
    /// </summary>
    public override void Update()
    {
        _age++;
        base.Update();
    }

    /// <summary>
    /// Renders the <see cref="Spark"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    protected override void OnRender(SKCanvas canvas, SKPaint paint)
    {
        int alpha = Math.Min(SparkLife - _age + 5, 255);
        SKColor color = SetAlpha(Color, alpha);
        base.Draw(canvas, paint, color, Meter * 0.5f);
    }
}
