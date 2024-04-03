This is a port of the logic in https://github.com/rehwinkel/FireworkScreenSaver
to render a firework animation in .net Maui using SkiaSharp.

High-level changes:

The static FireworkRenderer was rewritten as FireworkAnimation and placing
the Particle list as an instance variable.

The Particle-specific static variables in FireworkRenderer are now
defined in Particle.cs

Instead of calling FireworkRenderer.Render, an instance of a FireworkAnimation
is created and it's Run method is invoked.