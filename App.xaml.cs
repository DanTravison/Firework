using System.Diagnostics;

namespace FireworkExperiment;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new MainPage(this));
    }

    /// <summary>
    /// Called by the base class to create a window.
    /// </summary>
    /// <param name="activationState">The <see cref="IActivationState"/> to use to create the window.</param>
    /// <returns>The created <see cref="Window"/>.</returns>
    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);
        window.Resumed += OnWindowResumed;
        window.Backgrounding += OnWindowBackgrounding;
        window.Activated += OnWindowActivated;
        window.Deactivated += OnWindowDeactivated;
        window.Destroying += OnWindowDestroying;
        window.Stopped += OnWindowStopped;
        return window;
    }

    private void OnWindowStopped(object sender, EventArgs e)
    {
        Trace.WriteLine(nameof(Stopped), TraceCategory);
        Deactivated?.Invoke(this, ApplicationStateEventArgs.Stopped);
    }

    const string TraceCategory = "ActivationState";

    private void OnWindowDeactivated(object sender, EventArgs e)
    {
        Trace.WriteLine(nameof(Deactivated), TraceCategory);
        Deactivated?.Invoke(this, ApplicationStateEventArgs.Deactivated);
    }

    private void OnWindowActivated(object sender, EventArgs e)
    {
        Trace.WriteLine(nameof(Activated), TraceCategory);
        Activated?.Invoke(this, ApplicationStateEventArgs.Activated);   
    }

    private void OnWindowBackgrounding(object sender, BackgroundingEventArgs e)
    {
        Trace.WriteLine(nameof(Backgrounding), TraceCategory);
        Backgrounding?.Invoke(this, new BackgroundApplicationStateEventArgs(e));
    }

    private void OnWindowDestroying(object sender, EventArgs e)
    {
        Trace.WriteLine(nameof(Destroying), TraceCategory);
        Destroying?.Invoke(this, ApplicationStateEventArgs.Destroying);
    }

    private void OnWindowResumed(object sender, EventArgs e)
    {
        Trace.WriteLine(nameof(Resumed), TraceCategory);
        Resumed?.Invoke(this, ApplicationStateEventArgs.Resumed);
    }

    /// <summary>
    /// Occurs when <see cref="ApplicationState"/> changes to <see cref="ApplicationState.Activated"/>.
    /// </summary>
    public event ApplicationStateEventHandler Activated;
    /// <summary>
    /// Occurs when <see cref="ApplicationState"/> changes to <see cref="ApplicationState.Deactivated"/>.
    /// </summary>
    public event ApplicationStateEventHandler Deactivated;
    /// <summary>
    /// Occurs when <see cref="ApplicationState"/> changes to <see cref="ApplicationState.Backgrounding"/>.
    /// </summary>
    public event ApplicationStateEventHandler Backgrounding;
    /// <summary>
    /// Occurs when <see cref="ApplicationState"/> changes to <see cref="ApplicationState.Resumed"/>.
    /// </summary>
    public event ApplicationStateEventHandler Resumed;
    /// <summary>
    /// Occurs when <see cref="ApplicationState"/> changes to <see cref="ApplicationState.Destroying"/>.
    /// </summary>
    public event ApplicationStateEventHandler Destroying;
    /// <summary>
    /// Occurs when <see cref="ApplicationState"/> changes to <see cref="ApplicationState.Stopped"/>.
    /// </summary>
    public event ApplicationStateEventHandler Stopped;
}
