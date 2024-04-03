using System.ComponentModel;

namespace FireworkExperiment;

/// <summary>
/// Provides an abstract base class for classes supporting INotifyPropertyChanged
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    #region Fields

    readonly WeakEventManager _propertyChanged = new();

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    protected ObservableObject()
    {
    }

    #endregion Constructors

    #region INotifyPropertyChanged

    /// <summary>
    /// Occurs when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged
    {
        add
        {
            _propertyChanged.AddEventHandler(value);
        }
        remove
        {
            _propertyChanged.RemoveEventHandler(value);
        }
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event with a cached <see cref="PropertyChangedEventArgs"/>.
    /// </summary>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> for the event.</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        _propertyChanged.HandleEvent(this, e, nameof(PropertyChanged));
    }

    #endregion INotifyPropertyChanged

    #region SetProperty

    /// <summary>
    /// Provides a <see cref="INotifyPropertyChanged"/> event with a cached <see cref="PropertyChangedEventArgs"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> identifying the event.</param>
    /// <returns>true if the property was set; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is a null reference.</exception>
    protected bool SetProperty<T>(ref T field, T newValue, PropertyChangedEventArgs e)
    {
        return SetProperty(ref field, newValue, null, e);
    }

    /// <summary>
    /// Provides a <see cref="INotifyPropertyChanged"/> event with a cached <see cref="PropertyChangedEventArgs"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> identifying the event.</param>
    /// <returns>true if the property was set; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is a null reference.</exception>
    protected bool SetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));

        do
        {
            if (comparer != null && comparer.Equals(field, newValue))
            {
                break;
            }

            if (field != null)
            {
                if (newValue == null || field.Equals(newValue))
                {
                    break;
                }
            }

            field = newValue;
            OnPropertyChanged(e);
            return true;

        } while (false);

        return false;
    }

    #endregion SetProperty
}
