namespace FireworkExperiment.Fireworks;

using SkiaSharp;

internal class Firework : Particle, IFirework
{
    #region Fields

    readonly float _floor;
    readonly bool _addTrail;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="x">The zero-base X coordinate.</param>
    /// <param name="y">The height.</param>
    /// <param name="framerate">The frames per second of the animation.</param>
    public Firework(float x, float y, double framerate)
        : base(x, y)
    {
        // calculate a margin to ensure the firework doesn't go off screen
        // (top of the canvas).
        float margin = (float)Math.Round(y * 0.2, 0);

        // Randomly select a floor.
        _floor = margin / (float)(Rand.Next(4) + 1);

        // Select an adjustment between 25 and 30 pixels / frame.
        AdjustY = 25 + (Rand.Next(5) + 1);

        // Add a random small change in X
        AdjustX = Rand.Next(10) - 5;
        
        // Randomly select a color.
        Color = Rand.Next(4) == 0 ? SKColors.DarkRed : FromHue();
        // randomly draw a trail
        _addTrail = Rand.Next(4) < 2;

#if (false)
        if (Rand.Next(4) < 2)
        {
            // explode immediately at the apogee.
            Y = _floor;
        }
#endif
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
        AdjustY -= Gravity;

        float distance = Y - _floor;
        // slow down quicker when nearing the floor.
        if (distance < _floor * 2)
        {
            AdjustY -= Gravity;
        }
        if (_addTrail)
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
        /*
        int count = Rand.Next(3);
        for (int x = 0; x < count; x++)
        {
            int half = (int) Math.Round(_floor) / 2;
            float distance = _floor + Rand.Next(half);
            SecondaryFirework secondary = new(this, distance, (count & 1) == 1);
            particles.Add(secondary);
        }
        */
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
        get => Y < _floor || AdjustY <= 0;
    }

    #endregion IsDone
}
