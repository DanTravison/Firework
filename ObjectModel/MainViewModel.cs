using CodeCadence.Maui.Fireworks;
using FireworkExperiment.Fireworks;
using FireworkExperiment.Resources;
using System.ComponentModel;

namespace FireworkExperiment.ObjectModel;

internal class MainViewModel : ObservableObject
{
    #region Fields

    double _framerate = FireworkAnimation.DefaultFramerate;
    FireworkAnimation _animation;

    #endregion Fields

    public MainViewModel()
    {
        RunCommand = new(OnRun, FluentUI.CaretRight, Strings.PlayText, false);
        PauseCommand = new(OnPause, FluentUI.Pause, Strings.PauseText, false);
        StopCommand = new(OnStop, FluentUI.SquareFilled, Strings.StopText, false);
    }

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="FireworkAnimation.Framerate"/>.
    /// </summary>
    public double Framerate
    {
        get => _framerate;
        set
        {
            value = Math.Round(value, 0);
            if (value >= MinFramerate && value <= MaxFramerate)
            {
                if (SetProperty(ref _framerate, value, FramerateChangedEventArgs) && _animation != null)
                {
                    _animation.Framerate = value;
                }
            }
        }
    }

    /// <summary>
    /// Gets the minimum <see cref="FireworkAnimation.Framerate"/>.
    /// </summary>
    public double MinFramerate
    {
        get;
    } = FireworkAnimation.MinimumFramerate;

    /// <summary>
    /// Gets the maximum <see cref="FireworkAnimation.Framerate"/>.
    /// </summary>
    public double MaxFramerate
    {
        get;
    } = FireworkAnimation.MaximumFramerate;

    /// <summary>
    /// Gets or sets the <see cref="FireworkAnimation"/>.
    /// </summary>
    public FireworkAnimation Animation
    {
        get => _animation;
        set
        {
            if (!object.ReferenceEquals(_animation, value))
            {
                if (_animation != null)
                {
                    _animation.PropertyChanged -= OnAnimationPropertyChanged;
                    _animation.Stop();
                }
                _animation = value;
                if (_animation != null)
                {
                    _animation.PropertyChanged += OnAnimationPropertyChanged;
                    _animation.Framerate = Framerate;
                }
                SetState(AnimationState.Stopped);
                OnPropertyChanged(AnimationChangedEventArgs);
            }
        }
    }

    #endregion Properties

    private void OnAnimationPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (ReferenceEquals(e, FireworkAnimation.StateChangedEventArgs))
        {
            SetState(_animation.State);
        }
    }

    #region Commands

    /// <summary>
    /// Gets the command to start or resume the animation.
    /// </summary>
    public Command RunCommand
    {
        get;
    }

    /// <summary>
    /// Gets the command to pause the animation.
    /// </summary>
    public Command PauseCommand
    {
        get;
    }

    /// <summary>
    /// Gets the command to stop the animation.
    /// </summary>
    public Command StopCommand
    {
        get;
    }

    #endregion Commands

    #region Command Actions

    void OnRun(object parameter)
    {
        if (_animation != null)
        {
            _animation.Start();
        }
    }

    void OnPause(object parameter)
    {
        if (_animation != null)
        {
            if (_animation.State == AnimationState.Running)
            {
                _animation.Pause();
            }
            else if (_animation.State == AnimationState.Paused) 
            {
                _animation.Start();
            }
        }
    }

    void OnStop(object parameter)
    {
        if (_animation != null)
        {
            _animation.Stop();
        }
    }

    #endregion Command Actions

    /// <summary>
    /// Update the command IsEnabled states based on the <see cref="AnimationState"/>.
    /// </summary>
    /// <param name="state"></param>
    void SetState(AnimationState state)
    {
        RunCommand.IsEnabled = state == AnimationState.Stopped ||  state == AnimationState.Paused;
        PauseCommand.IsEnabled = state == AnimationState.Running;
        StopCommand.IsEnabled = state != AnimationState.Stopped;
    }

    /// <summary>
    /// <see cref="PropertyChangedEventArgs"/> passed to <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// when <see cref="Framerate"/> changes.
    /// </summary>
    static readonly PropertyChangedEventArgs FramerateChangedEventArgs = new(nameof(Framerate));

    /// <summary>
    /// <see cref="PropertyChangedEventArgs"/> passed to <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// when <see cref="Animation"/> changes.
    /// </summary>
    static readonly PropertyChangedEventArgs AnimationChangedEventArgs = new(nameof(Animation));
}
