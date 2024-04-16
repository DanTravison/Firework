namespace FireworkExperiment.Fireworks;

using FireworkExperiment.ObjectModel;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.ComponentModel;
using System.Diagnostics;

/// <summary>
/// Provides a firework animation.
/// </summary>
public class FireworkAnimation : ObservableObject, IDisposable
{
    #region Constants

    /// <summary>
    /// Defines the minimum number of launches per second.
    /// </summary>
    public const int MinimumLaunches = 1;

    /// <summary>
    /// Defines the maximum number of launches per second.
    /// </summary>
    public const int MaximumLaunches = 5;

    /// <summary>
    /// Defines the default number of launches per second.
    /// </summary>
    public const int DefaultLaunches = MaximumLaunches;

    /// <summary>
    /// Defines the minimum number of frames per second.
    /// </summary>
    public const double MinimumFramerate = 10;

    /// <summary>
    /// Defines the maximum number of frames per second.
    /// </summary>
    public const double MaximumFramerate = 120;

    /// <summary>
    /// Defines the default number of frames per second.
    /// </summary>
    public const double DefaultFramerate = 60;
    
    #endregion Constants

    #region Fields

    SKCanvasView _canvas;

    /// <summary>
    /// The delay, in milliseconds, between launches
    /// </summary>
    double _launchDelay = 1000 / DefaultLaunches;

    /// <summary>
    /// Stopwatch used to time launches.
    /// </summary>
    readonly Stopwatch _launchClock = new();

    /// <summary>
    /// Stopwatch used to time frameDelay time between OnPaint calls.
    /// </summary>
    readonly Stopwatch _frameClock = new();

    // Frames per second
    double _framerate = DefaultFramerate;
    
    // The animation loop's cycle length
    TimeSpan _delay;

    // The current animation state
    AnimationState _state;
    Task _loopTask;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvasView"/> to draw to.</param>
    public FireworkAnimation(SKCanvasView canvas)
    {
        _canvas = canvas;
        _canvas.PaintSurface += OnPaintSurface;
        _canvas.SizeChanged += OnSizeChanged;
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
    /// Gets the current <see cref="AnimationState"/>.
    /// </summary>
    public AnimationState State
    {
        get => _state;
        private set
        {
            if (SetProperty(ref _state, value, StateChangedEventArgs))
            {
                if (_state == AnimationState.Running)
                {
                    _launchClock.Restart();
                    _frameClock.Restart();
                }
                else
                {
                    _launchClock.Stop();
                    _frameClock.Stop();
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the number of frames per second.
    /// </summary>
    /// <value>
    /// A zero-based value indicating the number of updates per second.
    /// </value>
    /// <remarks>
    /// The value is throttled to the Range of <see cref="MinimumFramerate"/>
    /// and <see cref="MaximumFramerate"/>.
    /// </remarks>
    public double Framerate
    {
        get => _framerate;
        set
        {
            value = Math.Round(value, 0);
            if (value < MinimumFramerate)
            {
                value = MinimumFramerate;
            }
            else if (value > MaximumFramerate)
            {
                value = MaximumFramerate;
            }
            if (SetProperty(ref _framerate, value, FramerateChangedEventArgs))
            {
                UpdateClock();
            }
        }
    }

    /// <summary>
    /// Gets or sets the current number of launches per second.
    /// </summary>
    public int Launches
    {
        get => (int)(1000 / _launchDelay);
        set
        {
            if (value < MinimumLaunches)
            {
                value = MinimumLaunches;
            }
            else if (value > MaximumLaunches)
            {
                value = MaximumLaunches;
            }
            SetProperty(ref _launchDelay, 1000 / value, LaunchesChangedEventArgs);
        }
    }

    #endregion Properties

    #region Animation Control

    void UpdateClock()
    {
        _delay = TimeSpan.FromMilliseconds(1000 / _framerate);
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
            Particles.Clear();
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

    /// <summary>
    /// Defines the parallel animation loop.
    /// </summary>
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

    /// <summary>
    /// Invalidate the canvas on the UI thread.
    /// </summary>
    /// <returns>
    /// true if the invalidate was queued; otherwise, false if it 
    /// the canvas is no longer valid.
    /// </returns>
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

        double frameDelay = _frameClock.ElapsedMilliseconds;
        _frameClock.Restart();

        if (_state == AnimationState.Running)
        {
            double launchDelay = _launchClock.ElapsedMilliseconds;

            if (launchDelay >= _launchDelay)
            {
                Firework firework = new Firework(canvasSize.Width, canvasSize.Height, Framerate);
                Particles.Add(firework);
                _launchClock.Restart();
            }
        }

        if (_state != AnimationState.Stopped && Particles.Count > 0)
        {
            List<int> stale = [];

            if (_state == AnimationState.Paused)
            {
                frameDelay = 0;
            }

            using (SKPaint paint = new())
            {
                paint.StrokeWidth = 1;
                paint.Style = SKPaintStyle.Fill;

                for (int x = 0; x < Particles.Count; x++)
                {
                    Particle particle = Particles[x];
                    if (particle == null)
                    {
                        // TODO: Report and/or log error/warning
                        // Possible race.
                        if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                        return;
                    }
                    if (_state == AnimationState.Running)
                    {
                        if (frameDelay == 0)
                        {
                            // TODO: Report and/or log error/warning
                            if (Debugger.IsAttached)
                            {
                                Debugger.Break();
                            }
                            return;
                        }
                        particle.Update(Particles, frameDelay);
                        if (particle.IsDone)
                        {
                            // insert in reverse order.
                            stale.Insert(0, x);
                            if (particle is IFirework firework)
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
    /// when <see cref="Framerate"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs FramerateChangedEventArgs = new(nameof(Framerate));

    /// <summary>
    /// <see cref="PropertyChangedEventArgs"/> passed to <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// when <see cref="Launches"/> changes.
    /// </summary>
    public static readonly PropertyChangedEventArgs LaunchesChangedEventArgs = new(nameof(Launches));

    #endregion PropertyChangedEventArgs
}
