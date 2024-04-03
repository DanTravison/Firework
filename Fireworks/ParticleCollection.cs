namespace CodeCadence.Maui.Fireworks;

using System.Collections;

/// <summary>
/// Provides a limited <see cref="Particle"/> collection.
/// </summary>
internal sealed class ParticleCollection : IEnumerable<Particle>
{
    readonly List<Particle> _items = [];

    #region Properties

    /// <summary>
    /// Gets the number of particles in the collection.
    /// </summary>
    public int Count
    {
        get => _items.Count;
    }

    /// <summary>
    /// Gets the <see cref="Particle"/> at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="Particle"/> to get.</param>
    /// <returns>The <see cref="Particle"/> at the specified <paramref name="index"/>.</returns>
    public Particle this[int index]
    {
        get => _items[index];
    }

    #endregion Properties

    #region Add/RemoveAt

    /// <summary>
    /// Adds a <see cref="Particle"/> to the collection.
    /// </summary>
    /// <param name="particle"></param>
    public void Add(Particle particle)
    {
        _items.Add(particle);
    }

    /// <summary>
    /// Removes the <see cref="Particle"/> at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="Particle"/> to remove.</param>
    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
    }

    #endregion Add/RemoveAt

    #region IEnumerable

    /// <summary>
    /// Gets an <see cref="IEnumerator{Particle}"/> for enumerating item in then collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{Particle}"/> for enumerating item in then collection.</returns>
    public IEnumerator<Particle> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    /// <summary>
    /// Gets an <see cref="IEnumerator"/> for enumerating item in then collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> for enumerating item in then collection..</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    #endregion IEnumerable
}
