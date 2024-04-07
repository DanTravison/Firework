﻿namespace FireworkExperiment.Fireworks;

using SkiaSharp;

internal class Firework : Particle, IFirework
{
    #region Fields

    readonly struct Range
    {
        public readonly float Start;
        public readonly float End;

        public Range(float start, float end)
        {
            Start = start;
            End = end;
        }

        public readonly bool InBounds(float value)
        {
            return (value >= Start) && (value <= End);
        }
    }

    readonly Range _rangeX;
    readonly Range _rangeY;
    readonly bool _addTrail;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="x">The zero-base X coordinate.</param>
    /// <param name="y">The height.</param>
    /// <param name="framerate">The frames per second of the animation.</param>
    public Firework(float x, float y, float width, float height, double framerate)
        : base(x, y)
    {
        // calculate a margin to ensure the firework doesn't go off screen
        // (top of the canvas).
        float marginY = (float)Math.Round(height * 0.2, 0);
        // set a horizontal margin to ensure firework leaves room on 
        // either side for sparks
        float marginX = (float)Math.Round(width * 0.1, 0);

        // Randomly select a floor.
        _rangeY = new Range(0, marginY / (float)(Rand.Next(4) + 1));

        // Define the valid range of X
        _rangeX = new Range(marginX, width - marginX);

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

        float distance = Y - _rangeY.End;

        // slow down quicker when nearing the floor.
        if (distance < _rangeY.End * 2)
        {
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
        /*
        int count = Rand.Next(3);
        for (int x = 0; x < count; x++)
        {
            int half = (int) Math.Round(_floorY) / 2;
            float distance = _floorY + Rand.Next(half);
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
        get => Y < _rangeY.End || X <= _rangeX.Start || X >= _rangeX.End || AdjustY <= 0;
    }

    #endregion IsDone
}
