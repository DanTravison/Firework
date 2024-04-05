namespace FireworkExperiment.Fireworks;

using SkiaSharp;

internal class Firework : Particle
{
    #region Fields

    readonly float _floor;

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

        // Randomly select a color.
        Color = Rand.Next(4) == 0 ? SKColors.DarkRed : FromHue();
    }


    /// <summary>
    /// Updates the <see cref="Firework"/> for rendering.
    /// </summary>
    public override void Update()
    {
        DateTime now = DateTime.Now;
        Y -= AdjustY;
        AdjustY -= Gravity;

        float distance = Y - _floor;
        // slow down quicker when nearing the floor.
        if (distance < _floor * 2)
        {
            AdjustY -= Gravity;
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
    /// Explodes the firework.
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
    /// <param name="height">The current height.</param>
    /// <returns>true if the <see cref="Firework"/> is done animating; otherwise, false.</returns>
    public override bool IsDone(int height)
    {
        return Y < _floor || IsDone();
    }

    /// <summary>
    /// Determines if the <see cref="Firework"/> is done animating.
    /// </summary>
    /// <returns>true if the <see cref="Firework"/> is done animating; otherwise, false.</returns>
    public override bool IsDone()
    {
        return AdjustY <= 0;
    }

    #endregion IsDone

}
