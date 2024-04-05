using CodeCadence.Maui.Fireworks;
using FireworkExperiment.Fireworks;
using FireworkExperiment.Resources;
using System.ComponentModel;

namespace FireworkExperiment.ObjectModel;

internal class MainViewModel : ObservableObject
{
    #region Fields

    FireworkAnimation _animation;

    #endregion Fields

    public MainViewModel()
    {
        RunCommand = new(OnRun, FluentUI.CaretRight, Strings.PlayText, false);
        PauseCommand = new(OnPause, FluentUI.Pause, Strings.PauseText, false);
        StopCommand = new(OnStop, FluentUI.CircleFilled, Strings.StopText, false);
    }

    #region Properties

    /// <summary>
    /// Gets the minimum <see cref="FireworkAnimation.Framerate"/>.
    /// </summary>
    public double MinimumSpeed
    {
        get;
    } = FireworkAnimation.MinimumFramerate;

    /// <summary>
    /// Gets the maximum <see cref="FireworkAnimation.Framerate"/>.
    /// </summary>
    public double MaximumSpeed
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
        RunCommand.IsEnabled = state == AnimationState.Stopped;
        PauseCommand.IsEnabled = state == AnimationState.Running || state == AnimationState.Paused;
        StopCommand.IsEnabled = state != AnimationState.Stopped;
    }

    /// <summary>
    /// <see cref="PropertyChangedEventArgs"/> passed to <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// when <see cref="Animation"/> changes.
    /// </summary>
    static readonly PropertyChangedEventArgs AnimationChangedEventArgs = new(nameof(Animation));
}
