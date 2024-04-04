namespace CodeCadence.Maui.Fireworks;

using FireworkExperiment.Fireworks;
using FireworkExperiment.ObjectModel;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.ComponentModel;

/// <summary>
/// Provides a firework animation.
/// </summary>
public class FireworkAnimation : ObservableObject, IDisposable
{
    #region Constants

    /// <summary>
    /// Defines the minimum updates per second.
    /// </summary>
    public const double MinimumSpeed = 10;
    
    /// <summary>
    /// Defines the maximum updates per second.
    /// </summary>
    public const double MaximumSpeed = 120;

    /// <summary>
    /// Defines the default updates per second.
    /// </summary>
    public const double DefaultSpeed = 60;

    #endregion Constants

    #region Fields

    SKCanvasView _canvas;
    TimeSpan _delay;
    readonly int _launcherDelay;
    double _speed = DefaultSpeed;
    bool _running;
    DateTime _clock;
    AnimationState _state;
    Task _loopTask;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvasView"/> to draw to.</param>
    /// <param name="launcherDelay">The delay between launching a new firework.</param>
    public FireworkAnimation(SKCanvasView canvas, int launcherDelay = 400)
    {
        _canvas = canvas;
        _canvas.PaintSurface += OnPaintSurface;
        _canvas.SizeChanged += OnSizeChanged;
        _launcherDelay = launcherDelay;
    }

    #region Event Handlers

    private void OnSizeChanged(object sender, EventArgs e)
    {
        if (State == AnimationState.Running)
        {
            // Reset the animation to the new canvas size.
            Particles.Clear();
        }
    }

    #endregion Event Handlers

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

    /// <summary>
    /// Gets the current <see cref="AnimationState"/>.
    /// </summary>
    public AnimationState State
    {
        get => _state;
        private set
        {
            if (SetProperty(ref _state, value, StateChangedEventArgs))
            {
                _clock = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Gets or sets the speed of the animation.
    /// </summary>
    /// <value>
    /// A zero-based value indicating the number of updates per second.
    /// </value>
    /// <remarks>
    /// The value is throttled to the range of 10 through 12.
    /// The framework determines the frequency of the animation updates
    /// (i.e., a larger framerate results in a faster animation.)
    /// </remarks>
    public double Speed
    {
        get => _speed;
        set
        {
            value = Math.Round(value, 0);
            if (value < MinimumSpeed)
            {
                value = MinimumSpeed;
            }
            else if (value > MaximumSpeed) 
            {
                value = MaximumSpeed;
            }
            if (SetProperty(ref _speed, value, FramerateChangedEventArgs))
            {
                UpdateClock();
            }
        }
    }

    #endregion Properties

    #region Animation Control

    void UpdateClock()
    {
        _clock = DateTime.Now;
        _delay = TimeSpan.FromMilliseconds(1000 / _speed);
    }

    /// <summary>
    /// Starts or resumes the animation.
    /// </summary>
    public void Start()
    {
        if (_state == AnimationState.Stopped)
        {
            UpdateClock();
            State = AnimationState.Running;
            _loopTask = Task.Run(() => { Loop(); });
        }
        else if (_state == AnimationState.Paused)
        {
            UpdateClock();
            State = AnimationState.Running;
        }
    }

    /// <summary>
    /// Stops the animation
    /// </summary>
    /// <returns>
    /// true if the animation was stopped; otherwise, false if the animation 
    /// was not running or was paused.
    /// </returns>
    public void Stop()
    {
        if (_state != AnimationState.Stopped)
        {
            State = AnimationState.Stopped;
            _loopTask = null;
        }
    }

    /// <summary>
    /// Pauses the animation.
    /// </summary>
    public void Pause()
    {
        if (_state == AnimationState.Running)
        {
            State = AnimationState.Paused;
        }
    }

    async void Loop()
    {
        await Task.Delay(_delay);
        while (_state != AnimationState.Stopped)
        {
            if (_state == AnimationState.Running)
            {
                if (!Invalidate())
                {
                    State = AnimationState.Stopped;
                    return;
                }
            }
            await Task.Delay(_delay);
        }
        Invalidate();
    }

    #endregion Animation Control

    #region Draw

    bool Invalidate()
    {
        try
        {
            _canvas.Dispatcher.Dispatch(() => _canvas.InvalidateSurface());
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Renders the animation.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The <see cref="SKPaintSurfaceEventArgs"/> containing the event details.</param>
    void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (_state == AnimationState.Stopped)
        {
            return;
        }
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
        SKSize canvasSize = _canvas.CanvasSize;

        int yRange = (int)Math.Round(canvasSize.Height * 0.7, 0);

        if (_state == AnimationState.Running)
        {
            DateTime now = DateTime.Now;
            if ((now - _clock).TotalMilliseconds > _launcherDelay)
            {
                _clock = now;
                int xRange = (int)Math.Round(canvasSize.Width * 0.7, 0);
                int xMargin = (int)Math.Round((canvasSize.Width - xRange) / 2, 0);
                int x = Particle.Rand.Next(xRange) + xMargin;
                Particles.Add(new Firework(x, yRange));
            }
        }

        if (_state != AnimationState.Stopped && Particles.Count > 0)
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
                    if (_state == AnimationState.Running)
                    {
                        particle.Update();
                        if (particle.IsDone(yRange))
                        {
                            // insert in reverse order.
                            stale.Insert(0, x);
                            if (particle is Firework firework)
                            {
                                firework.Explode(Particles);
                                continue;
                            }
                        }
                    }
                    if (_state == AnimationState.Stopped)
                    {
                        break;
                    }
                    particle.Render(canvas, canvasSize, paint);
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

    #endregion Draw

    #region IDisposable

    /// <summary>
    /// Releases all resources and references.
    /// </summary>
    public void Dispose()
    {
        if (_canvas != null)
        {
            if (_state != AnimationState.Stopped)
            {
                _canvas.PaintSurface -= OnPaintSurface;
                _canvas.SizeChanged -= OnSizeChanged;
                Particles.Clear();
            }
            _canvas = null;
            GC.SuppressFinalize(this);
        }
    }

    #endregion IDisposable

    #region PropertyChangedEventArgs

    /// <summary>
    /// <see cref="PropertyChangedEventArgs"/> passed to <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// when <see cref="State"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs StateChangedEventArgs = new(nameof(State));

    /// <summary>
    /// <see cref="PropertyChangedEventArgs"/> passed to <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// when <see cref="Speed"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs FramerateChangedEventArgs = new(nameof(Speed));

    #endregion PropertyChangedEventArgs

}
