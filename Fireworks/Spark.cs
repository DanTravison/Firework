namespace FireworkExperiment.Fireworks;

using SkiaSharp;
using System.Diagnostics;

/// <summary>
/// Provides a spark <see cref="Particle"/>.
/// </summary>
[DebuggerDisplay("{_sparkType} {Location.X, Location.Y}@{Delta.X, Delta.Y}")]
internal class Spark : Particle
{
    /// <summary>
    /// Defines the types of sparks.
    /// </summary>
    /// <remarks>
    /// Currently used in DebuggerDisplay attribute to distinguish
    /// between types when debugging.
    /// </remarks>
    enum SparkType : int
    {
        /// <summary>
        /// Heart-shaped spark.
        /// </summary>
        Heart,
        /// <summary>
        /// Cluster/Ball spark.
        /// </summary>
        Ball,
        /// <summary>
        /// 'Explosive' spark
        /// </summary>
        Spark
    }

    /// <summary>
    /// The <see cref="Particle.Age"/> in seconds to reach before color starts to fade.
    /// </summary>
    const double FadeThreshold = .25;

    /// <summary>
    /// The initial velocity of an expanding heard. (pixels per second)
    /// </summary>
    const double HeartVelocity = 75;

    /// <summary>
    /// The initial velocity of an expanding ball.
    /// </summary>
    const double BallVelocity = 100;

    /// <summary>
    /// The initial velocity of an expanding spark. (pixels per second)
    /// </summary>
    protected const double SparkVelocity = 200;

    /// <summary>
    /// The <see cref="SparkType"/> for this instance.
    /// </summary>
    readonly SparkType _sparkType;

    static double SparkLifetime
    {
        // Randomize the maximum lifetime.
        get => .75 + Rand.NextDouble();
    }

    private Spark(SparkType type, Vector location, Vector velocity, SKColor color, double framerate)
        : base(location, velocity, framerate, SparkLifetime)
    {
        _sparkType = type;
        Color = color;
    }

    /// <summary>
    /// Gets the value indicating if the <see cref="Spark"/> is done.
    /// </summary>
    /// <returns>true if the <see cref="Spark"/> is done; otherwise, false.</returns>
    public override bool IsDone
    {
        get => Age >= Lifetime || Color.Alpha == 0;
    }

    /// <summary>
    /// Updates the <see cref="Spark"/> for rendering.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to optionally update - not used.</param>
    /// <param name="elapsed">The elapsed time, in milliseconds, since the last update.</param>
    protected override void OnUpdate(ParticleCollection particles, double elapsed)
    {
        // TODO: Determine a method for providing a consistent dispersal 
        // independent of framerate.
        // Currently, dispersal is the inverse of frame rate.
        // e.g., reducing framerate causes dispersal to increase
        // and vice-versa. 
        // Either tune Delta.X calculations to address this (preferred)
        // or tune Lifetime. 60fps seems to be a good balance right now. 
        Location = Location.Add(Delta.X, -Delta.Y);
        Delta = Delta.Add(0, -Gravity);
        Color = Fade(FadeThreshold);
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
    /// <param name="framerate">The animation frame rate (Frames per second)</param>
    public static void AddSparks(ParticleCollection particles, SKColor color, Vector location, double framerate)
    {
        int sparkType = Rand.Next(4);

        if (sparkType == (int)SparkType.Heart)
        {
            AddHearts(particles, location, color, framerate);
        }
        else if (sparkType == (int)SparkType.Ball)
        {
            AddBalls(particles, location, color, framerate);
        }
        else
        {
            AddSparks(particles, location, color, framerate);
        }
    }


    static void AddBalls(ParticleCollection particles, Vector location, SKColor color, double framerate)
    {
        double velocity = BallVelocity / framerate;

        for (int i = 0; i < 80; i++)
        {
            double vel = (Rand.NextDouble() + .2) * velocity;
            double ax = Math.Sin(i * 4.5 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 4.5 * DegreeToRad) * vel;
            Vector delta = new((float)ax, (float)ay);
            particles.Add(new Spark(SparkType.Ball, location, delta, color, framerate));
        }
    }

    static void AddSparks(ParticleCollection particles, Vector location, SKColor color, double framerate)
    {
        double velocity = SparkVelocity / framerate;
        int multiplier = 1;

        // Randomly double the number of sparks
        if (Rand.Next(4) == 0)
        {
            multiplier = 2;
        }

        // outer zone - Largest spread.
        for (int i = 0; i < 60 * multiplier; i++)
        {
            double vel = multiplier * (Rand.NextDouble() + .5) * velocity;
            double dx = Math.Sin(i * 18 * DegreeToRad) * vel;
            double dy = Math.Cos(i * 18 * DegreeToRad) * vel;
            Vector delta = new((float)dx, (float)dy);
            particles.Add(new Spark(SparkType.Spark, location, delta, color, framerate));
        }

        // middle zone - medium spread.
        for (int i = 0; i < 40 * multiplier; i++)
        {
            double vel = (Rand.NextDouble() + .25) * velocity;
            double dx = Math.Sin(i * 18 * DegreeToRad) * vel;
            double dy = Math.Cos(i * 18 * DegreeToRad) * vel;
            Vector delta = new((float)dx, (float)dy);

            particles.Add(new Spark(SparkType.Spark, location, delta, color, framerate));
        }

        // inner zone - smallest spread.
        for (int i = 0; i < 20 * multiplier; i++)
        {
            double vel = (Rand.NextDouble() + .1) * velocity;
            double dx = Math.Sin(i * 36 * DegreeToRad) * vel;
            double dy = Math.Cos(i * 36 * DegreeToRad) * vel;
            Vector delta = new((float)dx,  (float)dy);

            particles.Add(new Spark(SparkType.Spark, location, delta, FromHue(color.Hue + 180), framerate));
        }
    }

    static void AddHearts(ParticleCollection particles, Vector location, SKColor color, double framerate)
    {
        double velocity = HeartVelocity / framerate;

        // vary the expansion velocity between instances but maintain shape coherence
        double velocityMultiplier = 0.7 + Rand.NextDouble() * velocity;
        for (int i = 0; i < 60; i++)
        {
            int x2 = i * 6;

            int f = x2 > 180 ? -1 : 1;
            if (x2 > 180) x2 = 360 - x2;
            double vel = Heart(x2) * velocityMultiplier;
            double ax = Math.Sin(x2 * DegreeToRad) * vel * f;
            double ay = Math.Cos(x2 * DegreeToRad) * vel;
            Vector delta = new((float)ax, (float)ay);

            particles.Add(new Spark(SparkType.Heart, location, delta, color, framerate));
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
