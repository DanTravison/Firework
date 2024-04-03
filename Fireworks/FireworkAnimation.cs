namespace CodeCadence.Maui.Fireworks;

using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

/// <summary>
/// Provides a firework animation.
/// </summary>
public class FireworkAnimation
{
    readonly SKCanvasView _canvas;
    TimeSpan _delay;
    bool _running;
    bool _initialized;

    public FireworkAnimation(SKCanvasView canvas)
    {
        _canvas = canvas;
        Particles = [];
    }

    #region Properties

    /// <summary>
    /// Get the particles in the animation.
    /// </summary>
    internal ParticleCollection Particles
    {
        get;
    }

    /// <summary>
    /// Gets the value indicating if the animation is done.
    /// </summary>
    public bool IsDone
    {
        get => !_running;
    }

    #endregion Properties

    public void Start(TimeSpan delay)
    {
        if (!_running)
        {
            _delay = delay;
            _running = true;
            _initialized = false;
            _ = Loop();
        }
    }

    async Task Loop()
    {
        await Task.Delay(_delay);
        while (_running)
        {
            _canvas.InvalidateSurface();
            await Task.Delay(_delay);
        }
        _canvas.InvalidateSurface();
    }

    /// <summary>
    /// Renders the animation.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw on.</param>
    /// <param name="canvasSize">The size of the <paramref name="canvas"/>.</param>
    /// <returns>true if the animation has pending updates; otherwise, false
    /// if the animation is complete.</returns>
    public void Draw(SKCanvas canvas, SKSize canvasSize)
    {
        if (!_running)
        {
            return;
        }
        if (!_initialized)
        {
            int width = (int) Math.Round(canvasSize.Width, 0);
            Firework firework = new(Particle.Rand.Next(width), canvasSize.Height);
            Particles.Add(firework);
            _initialized = true;
        }

        if (Particles.Count > 0)
        {
            List<int> stale = [];

            using (SKPaint paint = new())
            {
                paint.StrokeWidth = 1;
                paint.Style = SKPaintStyle.Fill;
                int height = (int)Math.Round(canvasSize.Height, 0);
                for (int x = 0; x < Particles.Count; x++)
                {
                    Particle particle = Particles[x];
                    if (particle.IsDone(height))
                    {
                        // insert in reverse order.
                        stale.Insert(0, x);
                        if (particle is Firework firework)
                        {
                            firework.Explode(Particles);
                        }
                    }
                    else
                    {
                        particle.Render(canvas, canvasSize, paint);
                    }
                }
            }
            if (stale.Count > 0)
            {
                foreach (int index in stale)
                {
                    Particles.RemoveAt(index);
                }
            }
        }

        _running = Particles.Count > 0;
        if (!_running)
        {
            _initialized = false;
        }
    }
}
