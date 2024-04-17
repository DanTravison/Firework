using System.Diagnostics;

namespace FireworkExperiment.Fireworks;

/// <summary>
/// Provides a floating point 2-dimensional vector.
/// </summary>
[DebuggerDisplay("{X}x{Y}")]
internal readonly struct Vector
{
    /// <summary>
    /// Defines a Vector instance with zero X and Y values.
    /// </summary>
    public static readonly Vector Zero = new();

    /// <summary>
    /// Gets the X value.
    /// </summary>
    public readonly float X;

    /// <summary>
    /// Gets the Y value.
    /// </summary>
    public readonly float Y;

    /// <summary>
    /// Initializes a new instance of this struct.
    /// </summary>
    public Vector()
    { 
    }

    /// <summary>
    /// Initializes a new instance of this struct.
    /// </summary>
    /// <param name="x">The <see cref="X"/> value.</param>
    /// <param name="y">The <see cref="Y"/> value.</param>
    public Vector(float x, float y) 
    {
        X = x; 
        Y = y;
    }

    /// <summary>
    /// Creates a copy of this instance.
    /// </summary>
    /// <returns>A copy of this instance.</returns>
    public readonly Vector Clone()
    {
        return new(X, Y);
    }

    /// <summary>
    /// Creates a new <see cref="Vector"/> with updated <see cref="X"/> and <see cref="Y"/> values.
    /// </summary>
    /// <param name="x">The value to add to <see cref="X"/>.</param>
    /// <param name="y">The value to add to <see cref="Y"/>.</param>
    /// <returns>A new instance of a <see cref="Vector"/>.</returns>
    public readonly Vector Add(float x, float y)
    {
        return new(X + x, Y + y);
    }
}
