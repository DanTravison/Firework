namespace FireworkExperiment.ObjectModel;

using System;
using System.ComponentModel;
using System.Windows.Input;

internal class Command : ObservableObject, ICommand
{
    #region Fields

    bool _isEnabled;
    readonly Action<object> _action;
    string _glyph;
    string _description;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="glyph">The string glyph to display</param>
    /// <param name="description">The text to display as a description.</param>
    /// <param name="isEnabled">true if the command <see cref="IsEnabled"/>; otherwise, false.</param>
    public Command(Action<object> action, string glyph, string description, bool isEnabled = false)
    {
        _action = action;
        _isEnabled = isEnabled;
        _glyph = glyph;
    }

    /// <summary>
    /// Gets or sets the <see cref="IsEnabled"/> value.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (SetProperty(ref _isEnabled, value, IsEnabledChangedEventArgs))
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets the string glyph to display.
    /// </summary>
    public string Glyph
    {
        get => _glyph;
        set => SetProperty(ref _glyph, value, GlyphChangedEventArgs);
    }

    /// <summary>
    /// Gets or sets the string description of the command.
    /// </summary>
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value, DescriptionChangedEventArgs);
    }

    /// <summary>
    /// Gets the value indicating if the command can be executed.
    /// </summary>
    /// <param name="parameter">not used.</param>
    /// <returns>true if the command can be executed; otherwise, false.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool CanExecute(object parameter)
    {
        return _isEnabled;
        throw new NotImplementedException();
    }

    /// <summary>
    /// Executes the <see cref="Action{Object}"/> for the command.
    /// </summary>
    /// <param name="parameter">The parameter to pass to the action.</param>
    public void Execute(object parameter)
    {
        if (_isEnabled)
        {
            _action.Invoke(parameter);
        }
    }

    /// <summary>
    /// Occurs when <see cref="CanExecute(object)"/> changes.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// <see cref="PropertyChangedEventArgs"/> passed to <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// when <see cref="IsEnabled"/> changes.
    /// </summary>
    public static PropertyChangedEventArgs IsEnabledChangedEventArgs = new(nameof(IsEnabled));

    /// <summary>
    /// <see cref="PropertyChangedEventArgs"/> passed to <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// when <see cref="Glyph"/> changes.
    /// </summary>
    public static PropertyChangedEventArgs GlyphChangedEventArgs = new(nameof(Glyph));

    /// <summary>
    /// <see cref="PropertyChangedEventArgs"/> passed to <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// when <see cref="Description"/> changes.
    /// </summary>
    public static PropertyChangedEventArgs DescriptionChangedEventArgs = new(nameof(Description));
}