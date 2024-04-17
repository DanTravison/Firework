namespace FireworkExperiment;

using FireworkExperiment.Fireworks;
using FireworkExperiment.ObjectModel;

public partial class MainPage : ContentPage
{
    readonly MainViewModel _model = new();
    AnimationState _previousState;
    readonly App _app;

    public MainPage(App app)
    {
        BindingContext = _model;
        InitializeComponent();
        _model.Animation = new(Canvas);
        _app = app;
        Subscribe();
    }

    #region Event Handlers

    private void OnPauseAnimation(object sender, ApplicationStateEventArgs e)
    {
        _previousState = _model.Animation.State;
        if (_previousState == AnimationState.Running)
        {
            _model.Animation.Pause();
        }
    }

    private void OnStopAnimation(object sender, ApplicationStateEventArgs e)
    {
        _previousState = AnimationState.Stopped;
        _model.Animation.Stop();
    }

    private void OnResumeAnimation(object sender, EventArgs e)
    {
        if (_previousState == AnimationState.Running || _previousState == AnimationState.Paused)
        {
            _model.Animation.Start();
        }
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        if (_model.Animation.State != AnimationState.Stopped)
        {
            _model.Animation.Stop();
        }
        Unsubscribe();
    }

    #endregion Event Handlers

    void Subscribe()
    {
        // Use lifecycle events on mobile
        // to pause animations when not activated.
        // NOTE: Might not be desired for split screen modes.
#if (IOS || ANDROID)
        _app.Backgrounding += OnPauseAnimation;
        _app.Deactivated += OnPauseAnimation;
        _app.Stopped -= OnPauseAnimation;

        _app.Activated += OnResumeAnimation;
        _app.Resumed += OnResumeAnimation;

        _app.Destroying += OnStopAnimation;
#endif
        Unloaded += OnUnloaded;
    }

    void Unsubscribe()
    {
#if (IOS || ANDROID)
        _app.Activated -= OnResumeAnimation;
        _app.Resumed -= OnResumeAnimation;

        _app.Deactivated -= OnPauseAnimation;
        _app.Backgrounding -= OnPauseAnimation;
        _app.Stopped -= OnPauseAnimation;

        _app.Destroying -= OnStopAnimation;
#endif
        Unloaded -= OnUnloaded;
    }
}
