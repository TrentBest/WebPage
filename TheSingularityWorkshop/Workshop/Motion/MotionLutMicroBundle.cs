using System;

namespace TheSingularityWorkshop.Workshop.Motion;

/// <summary>
/// Deterministic presentation LUTs used by living Workshop geometry.
/// Keeping the motion functions here lets visual micro-bundles share the same
/// animation vocabulary without coupling presentation code to CSS easing names.
/// </summary>
public static class MotionLutMicroBundle
{
    private const int Samples = 256;
    private static readonly double[] SmoothStepLut = Build(SmoothStepRaw);
    private static readonly double[] BounceOutLut = Build(BounceOutRaw);

    public static double SmoothStep(double progress) => Sample(SmoothStepLut, progress);
    public static double BounceOut(double progress) => Sample(BounceOutLut, progress);

    private static double[] Build(Func<double, double> function)
    {
        var lut = new double[Samples + 1];
        for (var i = 0; i <= Samples; i++)
            lut[i] = function(i / (double)Samples);
        return lut;
    }

    private static double Sample(double[] lut, double progress)
    {
        progress = Math.Clamp(progress, 0, 1);
        var position = progress * Samples;
        var lower = (int)Math.Floor(position);
        var upper = Math.Min(Samples, lower + 1);
        var fraction = position - lower;
        return lut[lower] + (lut[upper] - lut[lower]) * fraction;
    }

    private static double SmoothStepRaw(double t) => t * t * (3 - 2 * t);

    private static double BounceOutRaw(double t)
    {
        const double n1 = 7.5625;
        const double d1 = 2.75;

        if (t < 1 / d1)
            return n1 * t * t;
        if (t < 2 / d1)
        {
            t -= 1.5 / d1;
            return n1 * t * t + .75;
        }
        if (t < 2.5 / d1)
        {
            t -= 2.25 / d1;
            return n1 * t * t + .9375;
        }

        t -= 2.625 / d1;
        return n1 * t * t + .984375;
    }
}