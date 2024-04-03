namespace CodeCadence.Maui.Fireworks;

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
    /// Defines the gravity constant.
    /// </summary>
    protected const float Gravity = 9.81f / 50f;

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

    #endregion Properties

    /// <summary>
    /// Determines if the <see cref="Particle"/> is done animating.
    /// </summary>
    /// <param name="height">The current height.</param>
    /// <returns>true if the <see cref="Particle"/> is done animating; otherwise, false.</returns>
    public virtual bool IsDone(int height)
    {
        return IsDone();
    }

    /// <summary>
    /// Determines if the <see cref="Particle"/> is done animating.
    /// </summary>
    /// <returns>true if the <see cref="Particle"/> is done animating; otherwise, false.</returns>
    public virtual bool IsDone()
    {
        return false;
    }

    /// <summary>
    /// Updates the particle for rendering.
    /// </summary>
    protected virtual void Update()
    {
        Y -= AdjustY;
        X += AdjustX;
        AdjustY -= Gravity;
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
    protected void Draw(SKCanvas canvas, SKPaint paint, SKColor color, float size)
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
            hue = Particle.Rand.Next(360);
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
