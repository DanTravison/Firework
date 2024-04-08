namespace FireworkExperiment.Fireworks;

using SkiaSharp;

/// <summary>
/// Provides a spark <see cref="Particle"/>.
/// </summary>
internal class Spark : Particle
{
    /// <summary>
    /// Randomize the maximum <see cref="Particle.Age"/>.
    /// </summary>
    double _maximumAge = 12.0 * (Rand.NextDouble() + .25);
    
    /// <summary>
    /// The <see cref="Particle.Age"/> to reach before color starts to fade.
    /// </summary>
    const double FadeThreshold = .25;
    
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="x">The <see cref="Particle.X"/> coordinate.</param>
    /// <param name="y">The <see cref="Particle.Y"/> coordinate.</param>
    /// <param name="adjustX">The <see cref="Particle.X"/> adjustment amount..</param>
    /// <param name="adjustY">The <see cref="Particle.Y"/> adjustment amount.</param>
    /// <param name="color">The <see cref="SKColor"/> to use to draw the <see cref="Spark"/>.</param>
    public Spark(float x, float y, float adjustX, float adjustY, SKColor color)
        : base(x, y, adjustX, adjustY)
    {
        // TODO: Consider increasing _maximumAge for 'taller' animations
        Color = color;
    }

    /// <summary>
    /// Gets the value indicating if the <see cref="Spark"/> is done.
    /// </summary>
    /// <returns>true if the <see cref="Spark"/> is done; otherwise, false.</returns>
    public override bool IsDone
    {
        get => Age >= _maximumAge;
    }

    /// <summary>
    /// Updates the <see cref="Spark"/> for rendering.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to optionally update - not used.</param>
    /// <param name="elapsed">The elapsed time, in milliseconds, since the last update.</param>
    protected override void OnUpdate(ParticleCollection particles, double elapsed)
    {
        base.OnUpdate(particles, elapsed);
        Color = Fade(FadeThreshold, _maximumAge);
    }

    /// <summary>
    /// Renders the <see cref="Spark"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    protected override void OnRender(SKCanvas canvas, SKPaint paint)
    {
        base.Draw(canvas, paint, Color, Meter * 0.5f);
    }

    /// <summary>
    /// Randomly adds <see cref="Spark"/> elements.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to update.</param>
    /// <param name="color">The base <see cref="SKColor"/> to use.</param>
    /// <param name="x">The <see cref="Particle.X"/> coordinate.</param>
    /// <param name="y">The <see cref="Particle.Y"/> coordinate.</param>
    public static void AddSparks(ParticleCollection particles, SKColor color, float x, float y)
    {
        int sparkType = Rand.Next(4);
        switch (sparkType)
        {
            case 0:
                AddHearts(particles, x, y, color);
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
            particles.Add(new Spark(x, y, (float)ax, (float)ay, color));
        }

        // middle zone
        for (int i = 0; i < 40 * multiplier; i++)
        {
            double vel = (Rand.NextDouble() + .1) * 2.0;
            double ax = Math.Sin(i * 18 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 18 * DegreeToRad) * vel;
            particles.Add(new Spark(x, y, (float)ax, (float)ay, color));
        }

        // inner zone
        for (int i = 0; i < 20 * multiplier; i++)
        {
            double vel = Rand.NextDouble() * 1.5;
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
