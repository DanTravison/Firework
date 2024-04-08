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
        : base(0, height)
    {
        // calculate a margin to prevent the X from outside the width.
        float xMargin = (int)Math.Round(width * 0.2, 0);
        // calculate a random X value.
        X = (float) Math.Round(xMargin + (float)Rand.NextDouble() * (width - 2 * xMargin), 0);

        _rangeX = new(xMargin, width);

        // calculate a margin to ensure the firework doesn't go off screen
        // (top of the canvas).
        float marginY = (float)Math.Round(height * 0.2, 0);
        float apogee = (float)Math.Round(marginY / (float)(Rand.Next(4) + 1), 0);
        // Randomly select a Y limit.
        _rangeY = new Range(height, apogee);

        if (apogee > 500)
        {
            _rangeY = new(height, _rangeY.Distance * .02f);
        }

        AdjustY = _rangeY.Distance / (float)framerate;
        // Add a random small change in X
        AdjustX = Rand.Next(10) - 5;
        
        // Randomly select a color.
        Color = Rand.Next(4) == 0 ? SKColors.DarkRed : FromHue();
        // randomly draw a trail
        _addTrail = Rand.Next(4) < 2;
    }

    /// <summary>
    /// Updates the <see cref="Firework"/> for rendering.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to update.</param>
    public override void Update(ParticleCollection particles)
    {
        float x = X;
        float y = Y;

        Y -= AdjustY;
        X += AdjustX;

        // The ascent is powered for the first 1/3 of the ascent.
        float distance = _rangeY.Start - Y;
        if (distance >= _rangeY.Distance / 3)
        {
            // power is zero - impart gravity.
            AdjustY -= Gravity;
        }

        if (_addTrail && Y - _rangeY.End > AdjustY)
        {
            particles.Add(new Trail(x, y, X, Y));
        }
    }

    /// <summary>
    /// Renders the <see cref="Firework"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    protected override void OnRender(SKCanvas canvas, SKPaint paint)
    {
        SKColor color = SetAlpha(Color, 32);
        Draw(canvas, paint, color, Meter * 0.8f);
    }

    /// <summary>
    /// Explodes the <see cref="Firework"/>.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to update.</param>
    public void Explode(ParticleCollection particles)
    {
        Spark.AddSparks(particles, Color, X, Y);
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
        get => Y < _rangeY.End || X <= _rangeX.Start || X >= _rangeX.End || AdjustY <= 0;
    }

    #endregion IsDone
}
