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
    readonly int _launcherDelay;
    bool _running;
    DateTime _start;
    Task _loopTask;

    public FireworkAnimation(SKCanvasView canvas, int launcherDelay = 400)
    {
        _canvas = canvas;
        _launcherDelay = launcherDelay;
    }

    #region Properties

    /// <summary>
    /// Get the particles in the animation.
    /// </summary>
    internal ParticleCollection Particles
    {
        get;
    } = new();

    /// <summary>
    /// Gets the value indicating if the animation is done.
    /// </summary>
    public bool IsDone
    {
        get => !_running;
    }

    #endregion Properties

    /// <summary>
    /// Starts the animation.
    /// </summary>
    /// <param name="framerate">The number of frames per second</param>
    public async void Start(int framerate)
    {
        Particles.Clear();
        if (_loopTask != null)
        {
            _running = false;
            await _loopTask;
        }
        
        _start = DateTime.Now;
        _delay = TimeSpan.FromMilliseconds(1000 / framerate);
        _running = true;
        _loopTask = Loop();
    }

    public void Stop()
    {
        _running = false;
        _loopTask = null;
    }

    bool Post()
    {
        try
        {
            _canvas.InvalidateSurface();
            return true;
        }
        catch
        {
            return false;
        }
    }

    async Task Loop()
    {
        await Task.Delay(_delay);
        while (_running)
        {
            if (!Post())
            {
                _running = false;
                return;
            }
            await Task.Delay(_delay);
        }
        Post();
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

        DateTime now = DateTime.Now;
        if ((now - _start).TotalMilliseconds > _launcherDelay)
        {
            _start = now;
            int xRange = (int)Math.Round(canvasSize.Width * 0.7, 0);
            int yRange = (int)Math.Round(canvasSize.Height * 0.7, 0);
            int xMargin = (int)Math.Round((canvasSize.Width - xRange) / 2, 0);
            int yMargin = (int)Math.Round(canvasSize.Height - yRange / 2, 0);
            int x = Particle.Rand.Next(xRange) + xMargin;
            int y = Particle.Rand.Next(yRange) + yMargin;
            Particles.Add(new Firework(x, yRange));
        }

        if (_running && Particles.Count > 0)
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
                    if (particle == null)
                    {
                        _running = false;
                        return;
                    }
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
                // Remove stale particles
                foreach (int index in stale)
                {
                    Particles.RemoveAt(index);
                }
            }
        }
    }
}
