﻿namespace FireworkExperiment.Fireworks;

using SkiaSharp;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides an abstract particle base class.
/// </summary>
internal abstract class Particle
{
    #region Fields

    /// <summary>
    /// Defines a singleton Random.
    /// </summary>
    public static readonly Random Rand = new Random();

    /// <summary>
    /// Defines the age of the particle, in seconds.
    /// </summary>
    double _age;

    /// <summary>
    /// Defines the base gravity constant.
    /// </summary>
    protected const float GravityConstant = 9.81f;

    /// <summary>
    /// Defines the default gravity constant to use to decelerate particles.
    /// </summary>
    protected const float Gravity = GravityConstant / 50f;

    /// <summary>
    /// Constant to convert degrees to radians
    /// </summary>
    protected const double DegreeToRad = 0.01745329251994329576923690768489;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="x">The zero-base X coordinate.</param>
    /// <param name="y">The zero-base Y coordinate.</param>
    protected Particle(float x, float y)
    {
        X = x;
        Y = y;
        Meter = y / 100;
    }

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="x">The zero-base X coordinate.</param>
    /// <param name="y">The zero-base Y coordinate.</param>
    /// <param name="adjustX">The X adjustment amount.</param>
    /// <param name="adjustY">The Y adjustment amount.</param>
    protected Particle(float x, float y, float adjustX, float adjustY)
        : this(x, y)
    {
        AdjustX = adjustX;
        AdjustY = adjustY;
    }

    #region Properties

    /// <summary>
    /// Gets the color to use to draw the <see cref="Spark"/>.
    /// </summary>
    public SKColor Color
    {
        get;
        protected set;
    }

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    public float X
    {
        get;
        protected set;
    }

    /// <summary>
    /// Gets the Y coordinate.
    /// </summary>
    public float Y
    {
        get;
        protected set;
    }

    /// <summary>
    /// Gets the X adjustment.
    /// </summary>
    protected float AdjustX
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the Y adjustment.
    /// </summary>
    protected float AdjustY
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the meter to use to draw.
    /// </summary>
    /// <remarks>
    /// This property is only valid within <see cref="OnRender"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">This property is not valid
    /// outside <see cref="OnRender"/>.</exception>
    protected float Meter
    {
        get;
        private set;
    }

    /// <summary>
    /// Determines if the <see cref="Particle"/> is done animating.
    /// </summary>
    /// <value>true if the <see cref="Particle"/> is done animating; otherwise, false.</value>
    public virtual bool IsDone
    {
        get => false;
    }

    /// <summary>
    /// Gets the age of the <see cref="Particle"/>, in seconds.
    /// </summary>
    public double Age
    {
        get => _age;
    }

    #endregion Properties

    /// <summary>
    /// Updates the particle for rendering.
    /// </summary>
    /// <param name="elapsed">The time since the last update; in milliseconds.</param>
    public void Update(ParticleCollection particles, double elapsed)
    {
        _age += elapsed / 1000;
        OnUpdate(particles, elapsed);
    }

    /// <summary>
    /// Overridden in the derive class to update the <see cref="Particle"/> for rendering.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to optionally update.</param>
    /// <param name="elapsed">The elapsed time, in milliseconds, since the last update.</param>
    /// <remarks>
    /// By default, <see cref="X"/> and <see cref="Y"/> are updated
    /// <see cref="AdjustX"/> and <see cref="AdjustY"/> respectively
    /// -and-
    /// <see cref="AdjustX"/> is decreased by <see cref="Gravity"/>.
    /// </remarks>
    protected virtual void OnUpdate(ParticleCollection particles, double elapsed)
    {
        Y -= AdjustY;
        X += AdjustX;
        AdjustY -= Gravity;
    }

    /// <summary>
    /// Fades a color based on it's maximumAge.
    /// </summary>
    /// <param name="fadeThreshold">The age threshold to start to fade.</param>
    /// <param name="maximumAge">The maximum maximumAge of the <see cref="Particle"/>.</param>
    /// <returns>The <see cref="SKColor"/> to use to render the <see cref="Parallel"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="maximumAge"/> is less than or equal to zero.
    /// </exception>
    protected SKColor Fade(double fadeThreshold, double maximumAge)
    {
        if (maximumAge <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumAge));
        }
        // delay fading the color
        if (_age <= fadeThreshold)
        {
            return Color;
        }
        else
        {
            double age = Math.Min(_age, maximumAge);
            // Fade the color based on age.
            int alpha = (int)(255 * (maximumAge - age) / maximumAge);
            return SetAlpha(Color, alpha);
        }
    }

    /// <summary>
    /// Renders the <see cref="Particle"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="canvasSize">The size of the <paramref name="canvas"/>.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    public void Render(SKCanvas canvas, SKSize canvasSize, SKPaint paint)
    {
        Meter = canvasSize.Height / 100;
        OnRender(canvas, paint);
    }

    /// <summary>
    /// Implemented in the derived class to render the <see cref="Particle"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    protected abstract void OnRender(SKCanvas canvas, SKPaint paint);

    /// <summary>
    /// Draws the <see cref="Particle"/>.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw to.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    /// <param name="color">The <see cref="SKColor"/> to use to draw.</param>
    /// <param name="size">The number of items to draw.</param>
    protected virtual void Draw(SKCanvas canvas, SKPaint paint, SKColor color, float size)
    {
        size = (float)Math.Round(size, 0);
        paint.Color = color;

        for (int i = 0; i < size; i++)
        {
            float x = X - (size - i);
            float y = Y - 1 - i;
            float w = size * 2 - (2 * i);
            float h = 2 + (2 * i);

            SKRect rect = new(x, y, x + w, y + h);
            canvas.DrawRect(rect, paint);
        }
    }

    #region Color Utilities

    /// <summary>
    /// Updates the alpha channel of an <see cref="SKColor"/>.
    /// </summary>
    /// <param name="color">The source <see cref="SKColor"/>.</param>
    /// <param name="alpha">The alpha value.</param>
    /// <returns>A new instance of an <see cref="SKColor"/>.</returns>
    public static SKColor SetAlpha(SKColor color, int alpha)
    {
        if (alpha < 0)
        {
            alpha = 0;
        }
        else if (alpha > 255)
        {
            alpha = 255;
        }
        return new SKColor(color.Red, color.Green, color.Blue, (byte)alpha);
    }

    /// <summary>
    /// Gets an <see cref="SKColor"/> from a hue.
    /// </summary>
    /// <param name="hue">The hue value; otherwise, zero to use a random hue value.</param>
    /// <returns>A new instance of a <see cref="SKColor"/>.</returns>
    public static SKColor FromHue(float hue = 0)
    {
        if (hue == 0)
        {
            hue = Rand.Next(360);
        }
        else
        {
            hue = hue % 360;
        }

        int hueTransform = (int)(255 * ((hue % 60) / 60.0));

        if (hue >= 0 && hue < 60)
        {
            return FromARGB(255, 255, hueTransform, 0);
        }
        if (hue >= 60 && hue < 120)
        {
            return FromARGB(255, 255 - hueTransform, 255, 0);
        }
        if (hue >= 120 && hue < 180)
        {
            return FromARGB(255, 0, 255, hueTransform);
        }
        if (hue >= 180 && hue < 240)
        {
            return FromARGB(255, 0, 255 - hueTransform, 255);
        }
        if (hue >= 240 && hue < 300)
        {
            return FromARGB(255, hueTransform, 0, 255);
        }
        if (hue >= 300 && hue < 360)
        {
            return FromARGB(255, 255, 0, 255 - hueTransform);
        }

        return new SKColor(255, 255, 255, 255);
    }

    /// <summary>
    /// Creates a color from alpha, red, green, and blue components.
    /// </summary>
    /// <param name="alpha">The <see cref="SKColor.Alpha"/> value.</param>
    /// <param name="red">The <see cref="SKColor.Red"/> value.</param>
    /// <param name="green">The <see cref="SKColor.Green"/> value.</param>
    /// <param name="blue">The <see cref="SKColor.Blue"/> value.</param>
    /// <returns>A new instance of a <see cref="SKColor"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static SKColor FromARGB(float alpha, float red, float green, float blue)
    {
        return new SKColor
        (
            (byte)((uint)Math.Round(red, 0) & 0xFF),
            (byte)((uint)Math.Round(green, 0) & 0xFF),
            (byte)((uint)Math.Round(blue, 0) & 0xFF),
            (byte)((uint)Math.Round(alpha, 0) & 0xFF)
        );
    }

    #endregion Color Utilities
}
