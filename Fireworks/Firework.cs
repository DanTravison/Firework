namespace CodeCadence.Maui.Fireworks;

using SkiaSharp;

internal class Firework : Particle
{
    #region Fields

    int _animationType;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="x">The zero-base X coordinate.</param>
    /// <param name="y">The zero-base Y coordinate.</param>
    public Firework(float x, float y) 
        : base(x, y)
    {
        AdjustY = (1.6f + (float)Rand.NextDouble() * 0.4f) * Meter;
        _animationType = Rand.Next(4);
        Color = _animationType == 0 ? SKColors.DarkRed : FromHue();
    }

    protected override void OnRender(SKCanvas canvas, SKPaint paint)
    {
        Update();
        SKColor color = SetAlpha(Color, 32);
        Draw(canvas, paint, color, Meter * 0.8f);
    }

    #region IsDone

    public override bool IsDone(int height)
    {
        return Y > height || IsDone();
    }

    public override bool IsDone()
    {
        return AdjustY <= 0;
    }

    #endregion IsDone

    #region Particles

    public void Explode(ParticleCollection particles)
    {
        switch (_animationType)
        {
            case 0:
                AddHearts(particles);
                break;
            case 1:
                AddSparks(particles);
                break;
            default:
                AddBalls(particles);
                break;
        }
    }

    const double DegreeToRad = 0.01745329251994329576923690768489;

    void AddBalls(ParticleCollection particles)
    {
        for (int i = 0; i < 80; i++)
        {
            double vel = Rand.NextDouble() * 1.2;
            double ax = Math.Sin(i * 4.5 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 4.5 * DegreeToRad) * vel;
            particles.Add(new Spark(X, Y, (float)ax, (float)ay, Color));
        }
    }

    void AddHearts(ParticleCollection particles)
    {
        for (int i = 0; i < 60; i++)
        {
            int x2 = i * 6;

            int f = x2 > 180 ? -1 : 1;
            if (x2 > 180) x2 = 360 - x2;
            double vel = Heart(x2) * (0.7 + Rand.NextDouble() * 0.3);
            double ax = Math.Sin(x2 * DegreeToRad) * vel * f;
            double ay = Math.Cos(x2 * DegreeToRad) * vel;
            particles.Add(new Spark(X, Y, (float)ax, (float)ay, SKColors.DarkRed));
        }
    }

    static double Heart(double x)
    {
        double a = 0.00001932;
        double b = -0.00580493;
        double c = 0.48038548;
        double d = 5;
        double result = a * (x * x * x) + b * (x * x) + c * x + d;
        return result / 10;
    }

    void AddSparks(ParticleCollection particles)
    {
        for (int i = 0; i < 40; i++)
        {
            double vel = Rand.NextDouble() * 1.2;
            double ax = Math.Sin(i * 18 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 18 * DegreeToRad) * vel;
            particles.Add(new Spark(X, Y, (float)ax, (float)ay, Color));
        }

        for (int i = 0; i < 20; i++)
        {
            double vel = Rand.NextDouble() * 1.2;
            double ax = Math.Sin(i * 36 * DegreeToRad) * vel;
            double ay = Math.Cos(i * 36 * DegreeToRad) * vel;
            particles.Add(new Spark(X, Y, (float)ax, (float)ay, FromHue(Color.Hue + 180)));
        }
    }

#endregion Particles
}
