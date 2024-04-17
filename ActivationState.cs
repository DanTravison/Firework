namespace FireworkExperiment;

/// <summary>
/// Defines the various application states.
/// </summary>
/// <remarks>
/// See Cross-platform lifecycle events for more details.
/// (https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/app-lifecycle?view=net-maui-8.0)
/// </remarks>
public enum ApplicationState
{
    /// <summary>
    /// The native window is being destroyed and deallocated. 
    /// The same cross-platform window might be used against a new native 
    /// window when the app is reopened.
    /// </summary>
    Destroying,

    /// <summary>
    /// The window has been activated, and is, or will become, the focused window.
    /// </summary>
    Activated,

    /// <summary>
    /// The window is no longer the focused window. However, the window might still be visible.
    /// </summary>
    Deactivated,

    /// <summary>
    /// The  window is no longer visible. There's no guarantee that an app will 
    /// resume from this state, because it may be terminated by the operating system.
    /// </summary>
    Stopped,

    /// <summary>
    /// The app resumes after being stopped. 
    /// </summary>
    Resumed,

    /// <summary>
    /// The app is being background.
    /// </summary>
    Backgrounding
}

/// <summary>
/// Defines a <see cref="Delegate"/> for handling <see cref="ApplicationState"/> changes.
/// </summary>
/// <param name="sender">The source <see cref="Application"/>.</param>
/// <param name="e">The <see cref="ApplicationStateEventArgs"/> containing the event details.</param>
public delegate void ApplicationStateEventHandler(object sender, ApplicationStateEventArgs e);

/// <summary>
/// Provides <see cref="EventArgs"/> when <see cref="ApplicationState"/> changes.
/// </summary>
public class ApplicationStateEventArgs : EventArgs
{
    internal static readonly ApplicationStateEventArgs Destroying = new(ApplicationState.Destroying);
    internal static readonly ApplicationStateEventArgs Activated = new(ApplicationState.Activated);
    internal static readonly ApplicationStateEventArgs Deactivated = new(ApplicationState.Deactivated);
    internal static readonly ApplicationStateEventArgs Resumed = new(ApplicationState.Resumed);
    internal static readonly ApplicationStateEventArgs Stopped = new(ApplicationState.Stopped);

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="state">The <see cref="ApplicationState"/>.</param>
    internal ApplicationStateEventArgs(ApplicationState state)
    {
        State = state;
    }

    /// <summary>
    /// Gets the <see cref="ApplicationState"/>.
    /// </summary>
    public ApplicationState State
    {
        get;
    }
}

/// <summary>
/// Provides <see cref="EventArgs"/> when <see cref="ApplicationState"/> changes to <see cref="ApplicationState.Backgrounding"/>.
/// </summary>
public class BackgroundApplicationStateEventArgs : ApplicationStateEventArgs
{
    internal BackgroundApplicationStateEventArgs(BackgroundingEventArgs e)
        : base(ApplicationState.Backgrounding)
    {
        PersistedState = e.State;
    }

    /// <summary>
    /// Gets the <see cref="IPersistedState"/>.
    /// </summary>
    public IPersistedState PersistedState
    {
        get;
    }
}
