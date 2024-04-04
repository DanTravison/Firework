namespace FireworkExperiment;

using FireworkExperiment.Fireworks;
using FireworkExperiment.ObjectModel;

public partial class MainPage : ContentPage
{
    readonly MainViewModel _model = new();
    AnimationState _previousState;

    public MainPage()
    {
        BindingContext = _model;
        InitializeComponent();
        _model.Animation = new(Canvas);
    }

    protected override void OnDisappearing()
    {
        _previousState = _model.Animation.State;
        _model.Animation.Stop();
        base.OnDisappearing();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_previousState != AnimationState.Stopped)
        {
            _model.Animation.Start();
            if (_previousState == AnimationState.Paused)
            {
                _model.Animation.Pause();
            }
        }
    }
}
