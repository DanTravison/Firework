using SkiaSharp;

namespace FireworkExperiment.Fireworks;

/// <summary>
/// Defines the interface for a <see cref="Particle"/> that 
/// explodes when it's animation completes.
/// </summary>
internal interface IFirework
{
    /// <summary>
    /// Explodes the firework.
    /// </summary>
    /// <param name="particles">The <see cref="ParticleCollection"/> to update.</param>
    void Explode(ParticleCollection particles);

    /// <summary>
    /// Gets the location of the <see cref="IFirework"/>.
    /// </summary>
    public Vector Location
    {
        get;
    }

    /// <summary>
    /// Gets the animation framerate (frames per second).
    /// </summary>
    public double Framerate
    {
        get;
    }

    /// <summary>
    /// Gets the color to use to draw the <see cref="Spark"/>.
    /// </summary>
    public SKColor Color
    {
        get;
    }
}
