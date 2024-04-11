namespace FireworkExperiment.Fireworks;

using SkiaSharp;
using System.Diagnostics;

internal class Firework : Particle, IFirework
{
    #region Fields

    [DebuggerDisplay("{Start}->{End} {Distance}")]
    readonly struct Range
    {
        public readonly float Start;
        public readonly float End;
        public readonly float Distance;

        public Range(float start, float end)
        {
            Start = start;
            End = end;
            Distance = Math.Abs(start - end);
        }
    }

    readonly Range _rangeX;
    readonly Range _rangeY;
    readonly bool _addTrail;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="width">The width of the animation.</param>
    /// <param name="height">The height of the animation.</param>
    /// <param name="framerate">The frames per second.</param>
    public Firework(float width, float height, double framerate)
        : base()
    {
        // calculate a margin to ensure the firework doesn't go off screen
        // (top of the canvas).
        float marginY = (float)Math.Round(height * 0.2, 0);
        // Randomly select a Y limit.
        float apogee = (float)Math.Round(marginY / (float)(Rand.Next(4) + 1), 0);

        // TODO: Need a better calculation for degrading the speed. This approach
        // causes Y to reach zero too soon on lower heights.
        _rangeY = new Range(height, apogee);
        float vy = height / (float)framerate;

        // calculate a margin to prevent the X from outside the width.
        float xMargin = (int)Math.Round(width * 0.2, 0);
        // calculate a random X value.
        float x = (float)Math.Round(xMargin + (float)Rand.NextDouble() * (width - 2 * xMargin), 0);
        
        _rangeX = new(xMargin, width);

        // Add a random small change in X for 2/3 of the launches
        int multiplier = Rand.Next(3) - 1;
        float vx = multiplier * (height * 0.15f) / (float)framerate;

        Velocity = new(vx, vy);
        Location = new(x, height);

        // Randomly select a color.
        Color = Rand.Next(4) == 0 ? SKColors.DarkRed : FromHue();
        // randomly draw a trail
        _addTrail = Rand.Next(4) < 2;
    }

    /// <summary>
    /// Updates the <see cref="Firework"/> for rendering.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to update.</param>
    /// <param name="elapsed">The time since the last update; in milliseconds.</param>
    protected override void OnUpdate(ParticleCollection particles, double elapsed)
    {
        Vector previous = Location.Clone();

        Location = Location.Add(Velocity.X, -Velocity.Y);

        // The ascent is powered for the first 1/3 of the ascent.
        float distance = _rangeY.Start - Location.Y;
        if (distance >= _rangeY.Distance / 3)
        {
            // power is zero - impart gravity.
            Velocity = Velocity.Add(0, -Gravity);
        }

        if (_addTrail && Location.Y - _rangeY.End > Velocity.Y)
        {
            particles.Add(new Trail(previous, Location));
        }

        Color = SetAlpha(Color, 32);
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

    /// <summary>
    /// Explodes the <see cref="Firework"/>.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to update.</param>
    public void Explode(ParticleCollection particles)
    {
        Spark.AddSparks(particles, Color, Location);
     }

    #region IsDone

    /// <summary>
    /// Determines if the <see cref="Firework"/> is done animating.
    /// </summary>
    /// <value>
    /// true if the <see cref="Firework"/> is done animating; otherwise, false.
    /// </value>
    public override bool IsDone
    {
        get => Location.Y < _rangeY.End 
            || Location.X <= _rangeX.Start 
            || Location.X >= _rangeX.End 
            || Velocity.Y <= 0;
    }

    #endregion IsDone
}
