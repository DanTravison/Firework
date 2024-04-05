namespace CodeCadence.Maui.Fireworks;

using FireworkExperiment.Fireworks;
using SkiaSharp;

/// <summary>
/// Provides a spark <see cref="Particle"/>.
/// </summary>
internal class Spark : Particle
{
    const int SparkLife = 100;
    int _age;
    const double DegreeToRad = 0.01745329251994329576923690768489;

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

    /// <summary>
    /// Randomly adds <see cref="Spark"/> elements.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to update.</param>
    /// <param name="color">The base <see cref="SKColor"/> to use.</param>
    /// <param name="x">The zero-based X coordinate.</param>
    /// <param name="y">The zero-based starting Y coordinate.</param>
    public static void AddSparks(ParticleCollection particles, SKColor color, float x, float y)
    {
        int sparkType = Rand.Next(4);
        switch (sparkType)
        {
            case 0:
                AddHearts(particles, x, y, SKColors.Red);
                break;
            case 1:
                AddBalls(particles, x, y, color);
                break;
            default:
                AddSparks(particles, x, y, color);
                break;
        }
    }

    static void AddBalls(ParticleCollection particles, float x, float y, SKColor color)
    {
        for (int i = 0; i < 80; i++)
        {
            double vel = Rand.NextDouble() * 1.2;
            double ax = Math.Sin(i * 4.5 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 4.5 * DegreeToRad) * vel;
            particles.Add(new Spark(x, y, (float)ax, (float)ay, color));
        }
    }

    static void AddSparks(ParticleCollection particles, float x, float y, SKColor color)
    {
        for (int i = 0; i < 40; i++)
        {
            double vel = Rand.NextDouble() * 1.2;
            double ax = Math.Sin(i * 18 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 18 * DegreeToRad) * vel;
            particles.Add(new Spark(x, y, (float)ax, (float)ay, color));
        }

        for (int i = 0; i < 20; i++)
        {
            double vel = Rand.NextDouble() * 1.2;
            double ax = Math.Sin(i * 36 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 36 * DegreeToRad) * vel;
            particles.Add(new Spark(x, y, (float)ax, (float)ay, FromHue(color.Hue + 180)));
        }
    }

    static void AddHearts(ParticleCollection particles, float x, float y, SKColor color)
    {
        for (int i = 0; i < 60; i++)
        {
            int x2 = i * 6;

            int f = x2 > 180 ? -1 : 1;
            if (x2 > 180) x2 = 360 - x2;
            double vel = Heart(x2) * (0.7 + Rand.NextDouble() * 0.3);
            double ax = Math.Sin(x2 * DegreeToRad) * vel * f;
            double ay = Math.Cos(x2 * DegreeToRad) * vel;
            particles.Add(new Spark(x, y, (float)ax, (float)ay, color));
        }
    }

    static double Heart(double x)
    {
        double a = 0.00001932;
        double b = -0.00580493;
        double c = 0.48038548;
        double d = 5;
        double result = a * (x * x * x) + b * (x * x) + c * x + d;
        return result / 10;
    }
}
