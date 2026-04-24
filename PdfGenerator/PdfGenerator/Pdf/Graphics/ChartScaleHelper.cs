namespace PdfGenerator.Pdf.Graphics;

internal static class ChartScaleHelper
{
    public static ChartAxisScale Create(decimal minValue, decimal maxValue, int minimumIntervals)
    {
        if (minimumIntervals < 1)
            minimumIntervals = 1;

        if (minValue == maxValue)
        {
            var padding = minValue == 0 ? 1m : Math.Abs(minValue) * 0.1m;
            minValue -= padding;
            maxValue += padding;
        }

        var rawRange = maxValue - minValue;
        var niceRange = NiceNumber(rawRange, round: false);
        var step = NiceNumber(niceRange / minimumIntervals, round: true);
        var axisMin = Math.Floor(minValue / step) * step;
        var axisMax = Math.Ceiling(maxValue / step) * step;
        var intervalCount = Math.Max(minimumIntervals, (int)Math.Ceiling((axisMax - axisMin) / step));
        axisMax = axisMin + (intervalCount * step);

        return new ChartAxisScale(axisMin, axisMax, step, intervalCount, DetermineDecimals(step));
    }

    public static string FormatLabel(decimal value, int decimals)
    {
        var rounded = Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        return rounded.ToString($"F{decimals}").TrimEnd('0').TrimEnd('.');
    }

    private static decimal NiceNumber(decimal value, bool round)
    {
        if (value <= 0)
            return 1m;

        var exponent = Math.Floor(Math.Log10((double)value));
        var power = (decimal)Math.Pow(10, exponent);
        var fraction = value / power;

        decimal niceFraction;
        if (round)
        {
            if (fraction < 1.5m)
                niceFraction = 1m;
            else if (fraction < 3m)
                niceFraction = 2m;
            else if (fraction < 7m)
                niceFraction = 5m;
            else
                niceFraction = 10m;
        }
        else
        {
            if (fraction <= 1m)
                niceFraction = 1m;
            else if (fraction <= 2m)
                niceFraction = 2m;
            else if (fraction <= 5m)
                niceFraction = 5m;
            else
                niceFraction = 10m;
        }

        return niceFraction * power;
    }

    private static int DetermineDecimals(decimal step)
    {
        step = Math.Abs(step);

        if (step >= 1m)
            return 0;

        var decimals = 0;
        while (step < 1m && decimals < 6)
        {
            step *= 10m;
            decimals++;
        }

        return decimals;
    }
}

internal sealed record ChartAxisScale(decimal Min, decimal Max, decimal Step, int IntervalCount, int Decimals)
{
    public decimal ValueAt(int tickIndex)
    {
        return Min + (tickIndex * Step);
    }
}