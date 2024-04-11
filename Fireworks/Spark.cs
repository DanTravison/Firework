namespace FireworkExperiment.Fireworks;

using SkiaSharp;
using System.Runtime.Intrinsics.X86;

/// <summary>
/// Provides a spark <see cref="Particle"/>.
/// </summary>
internal class Spark : Particle
{
    /// <summary>
    /// The <see cref="Particle.Age"/> to reach before color starts to fade.
    /// </summary>
    const double FadeThreshold = .25;

    static double SparkLifetime
    {
        // Randomize the maximum lifetime.
        // TODO: Consider increasing Lifetime for 'taller' animations
        get => 12.0 * (Rand.NextDouble() + .25);
    }

    public Spark(Vector location, Vector velocity, SKColor color)
        : base(location, velocity, SparkLifetime)
    {
        Color = color;
    }

    /// <summary>
    /// Gets the value indicating if the <see cref="Spark"/> is done.
    /// </summary>
    /// <returns>true if the <see cref="Spark"/> is done; otherwise, false.</returns>
    public override bool IsDone
    {
        get => Age >= Lifetime;
    }

    /// <summary>
    /// Updates the <see cref="Spark"/> for rendering.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to optionally update - not used.</param>
    /// <param name="elapsed">The elapsed time, in milliseconds, since the last update.</param>
    protected override void OnUpdate(ParticleCollection particles, double elapsed)
    {
        base.OnUpdate(particles, elapsed);
        Color = Fade(FadeThreshold, Lifetime);
    }

    /// <summary>
    /// Renders the <see cref="Spark"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    protected override void OnRender(SKCanvas canvas, SKPaint paint)
    {
        base.Draw(canvas, paint, Color, SizeMetric * 0.5f);
    }

    /// <summary>
    /// Randomly adds <see cref="Spark"/> elements.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to update.</param>
    /// <param name="color">The base <see cref="SKColor"/> to use.</param>
    /// <param name="location">The <see cref="Vector"/> for the starting location.</param>
    public static void AddSparks(ParticleCollection particles, SKColor color, Vector location)
    {
        int sparkType = Rand.Next(4);
        switch (sparkType)
        {
            case 0:
                AddHearts(particles, location, color);
                break;
            case 1:
                AddBalls(particles, location, color);
                break;
            default:
                AddSparks(particles, location, color);
                break;
        }
    }

    static void AddBalls(ParticleCollection particles, Vector location, SKColor color)
    {
        for (int i = 0; i < 80; i++)
        {
            double vel = Rand.NextDouble() * 1.2;
            double ax = Math.Sin(i * 4.5 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 4.5 * DegreeToRad) * vel;
            Vector velocity = new((float)ax, (float)ay);
            particles.Add(new Spark(location, velocity, color));
        }
    }

    static void AddSparks(ParticleCollection particles, Vector location, SKColor color)
    {
        int multiplier = 1;

        // Randomly double the number of sparks
        if (Rand.Next(4) == 0)
        {
            multiplier = 2;
        }

        // outer zone
        for (int i = 0; i < 60 * multiplier; i++)
        {
            // NOTE: When doubled, increase the velocity by the multiplier.
            // for a more dynamic explosion.
            double vel = (Rand.NextDouble() + .5) * 2.5 * multiplier;
            double ax = Math.Sin(i * 18 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 18 * DegreeToRad) * vel;
            Vector velocity = new((float)ax, (float)ay);
            particles.Add(new Spark(location,  velocity, color));
        }

        // middle zone
        for (int i = 0; i < 40 * multiplier; i++)
        {
            double vel = (Rand.NextDouble() + .1) * 2.0;
            double ax = Math.Sin(i * 18 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 18 * DegreeToRad) * vel;
            Vector velocity = new((float)ax, (float)ay);

            particles.Add(new Spark(location, velocity, color));
        }

        // inner zone
        for (int i = 0; i < 20 * multiplier; i++)
        {
            double vel = Rand.NextDouble() * 1.5;
            double ax = Math.Sin(i * 36 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 36 * DegreeToRad) * vel;
            Vector velocity = new((float)ax, (float)ay);

            particles.Add(new Spark(location, velocity, FromHue(color.Hue + 180)));
        }
    }

    static void AddHearts(ParticleCollection particles, Vector location, SKColor color)
    {
        for (int i = 0; i < 60; i++)
        {
            int x2 = i * 6;

            int f = x2 > 180 ? -1 : 1;
            if (x2 > 180) x2 = 360 - x2;
            double vel = Heart(x2) * (0.7 + Rand.NextDouble() * 0.3);
            double ax = Math.Sin(x2 * DegreeToRad) * vel * f;
            double ay = Math.Cos(x2 * DegreeToRad) * vel;
            Vector velocity = new((float)ax, (float)ay);

            particles.Add(new Spark(location, velocity, color));
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
