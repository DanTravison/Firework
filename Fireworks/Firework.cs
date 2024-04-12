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

    /// <summary>
    /// Defines the initial velocity for a launch. (pixels per second)
    /// </summary>
    protected const double InitialVelocity = 1000;

    readonly Range _rangeX;
    readonly Range _rangeY;
    readonly bool _addTrail;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="width">The width of the animation.</param>
    /// <param name="height">The height of the animation.</param>
    /// <param name="framerate">The animation frames per second.</param>
    public Firework(float width, float height, double framerate)
        : base(framerate)
    {
        // Determining launch point and 'velocity' and when to explode.

        // The goal is to have 1/3 of the launches be straight up while
        // the remaining to veer left or right by a random value.
        // The apogee is randomly selected for each.
        // The vertical velocity is fixed for a portion of the launch
        // then gravity is applied.
        // The launch Y coordinate is always the bottom of the 
        // view while the X coordinate is randomly selected. 
        
        // Explosion occurs when one of three conditions is met.
        // 1: The apogee is reached (_rangeY.End)
        // 2: Velocity is less than or equal to zero (Delta.Y).
        // 3: The left or right edge is crossed (_range.Start or _range.End)

        // Apogee (_rangeY.End): A base apogee of 30% of the height
        // divided by a random value from 1 through 4.
        float marginY = (float)Math.Round(height * 0.3, 0);
        float apogee = (float)Math.Round(marginY / (float)(Rand.Next(4) + 1), 0);

        // Set the range of Y and the initial velocity (Delta.Y)
        _rangeY = new Range(height, apogee);
        float deltaY = (float)(InitialVelocity / framerate);

        // Calculate the left and right edges using 80% of the width.
        float xMargin = (int)Math.Round(width * 0.2, 0);
        _rangeX = new(xMargin, width - xMargin);

        // Select an X launch point and direction (Delta.X)
        // 1/3 of the launches will be straight up.
        // The remaining will have a small, random change in X
        // and a random sign (+ or -)

        // Shrink the launch width to ensure the firework is not launched
        // near an edge otherwise it will explode too soon.
        float launchWidth = _rangeX.Distance * .90f;
        xMargin = (width - launchWidth) / 2;

         // Randomly select an X within the smaller width
        float x = xMargin + (float)Math.Round((float)Rand.NextDouble() * launchWidth, 0);

        float deltaX;
        // add a random Delta.X to 2/3 of the launches.
        if (Rand.Next(3) > 0)
        {
            // randomly select the direction.
            int sign = Rand.Next(2) == 0 ? 1 : -1;
            // randomly select the delta.
            double delta = (float)Rand.NextDouble() * 0.15;
            deltaX = (float)((height * delta) / framerate) * sign;
        }
        else
        {
            deltaX = 0;
        }

        Delta = new(deltaX, deltaY);
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

        Location = Location.Add(Delta.X, -Delta.Y);

        // The ascent is powered for the first half of the ascent.
        // NOTE: Y decreases over time.
        float distance = _rangeY.End - Location.Y;
        if (distance >= _rangeY.Distance / 2)
        {
            // power is zero - impart gravity.
            float dy = GravityConstant * (distance / (float)Framerate);
            
            // float dy = Delta.Y - Gravity;
            Delta = new(Delta.X, dy);
        }

        if (_addTrail && Location.Y - _rangeY.End > Delta.Y)
        {
            // the trail is rendered by drawing a line from the previous
            // to current location.
            particles.Add(new Trail(previous, Location, Framerate));
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
        // NOTE: The type of spark is random.
        Spark.AddSparks(particles, Color, Location, Framerate);
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
        // explode at the apogee, the left or right edge,
        // or velocity is zero.
        get => Location.Y < _rangeY.End
            || Location.X <= _rangeX.Start
            || Location.X >= _rangeX.End
            || Delta.Y <= 0;
    }

    #endregion IsDone
}
