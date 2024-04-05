namespace FireworkExperiment.Fireworks;

/// <summary>
/// Provides a limited <see cref="Particle"/> collection.
/// </summary>
internal sealed class ParticleCollection
{
    #region Fields

    readonly List<Particle> _items = [];
    readonly object _lock = new();

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the number of particles in the collection.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _items.Count;
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="Particle"/> at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="Particle"/> to get.</param>
    /// <returns>The <see cref="Particle"/> at the specified <paramref name="index"/>.</returns>
    public Particle this[int index]
    {
        get
        {
            lock (_lock)
            {
                if (index < _items.Count)
                {
                    return _items[index];
                }
            }
            return null;
        }
    }

    #endregion Properties

    #region Add/RemoveAt

    /// <summary>
    /// Adds a <see cref="Particle"/> to the collection.
    /// </summary>
    /// <param name="particle"></param>
    public void Add(Particle particle)
    {
        lock (_lock)
        {
            _items.Add(particle);
        }
    }

    /// <summary>
    /// Removes the <see cref="Particle"/> at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="Particle"/> to remove.</param>
    public void RemoveAt(int index)
    {
        lock (_lock)
        {
            if (index < _items.Count)
            {
                _items.RemoveAt(index);
            }
        }
    }

    /// <summary>
    /// Removes all <see cref="Particle"/> items from the collection.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _items.Clear();
        }
    }

    #endregion Add/RemoveAt
}
