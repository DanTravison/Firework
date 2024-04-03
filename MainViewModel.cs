using System.ComponentModel;

namespace FireworkExperiment;

internal class MainViewModel : ObservableObject
{
    double _framerate = 100;

    public double Framerate
    {
        get => _framerate;
        set
        {
            if (value >= MinFramerate && value <= MaxFramerate)
            {
                value = Math.Round(value, 0);
                SetProperty(ref _framerate, value, FramerateChangedEventArgs);
            }
        }
    }

    public double MinFramerate
    {
        get => 10;
    }

    public double MaxFramerate
    {
        get => 200;
    }

    static readonly PropertyChangedEventArgs FramerateChangedEventArgs = new(nameof(Framerate));
}
