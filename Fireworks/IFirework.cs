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
}
