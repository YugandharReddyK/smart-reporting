using System.Globalization;

namespace PdfGenerator.Pdf.Shared;

internal static class StatusUiHelper
{
    private const string IconCircle = "\uf111";
    private const string IconArrowUp = "\uf30c";
    private const string IconArrowDown = "\uf309";

    public static string TrendSymbol(string trend)
    {
        return trend.Trim().ToLowerInvariant() switch
        {
            "up" or "normal" => IconArrowUp,
            "down" or "abnormal" => IconArrowDown,
            _ => IconCircle
        };
    }

    public static string StatusIcon(string status)
    {
        return status.Trim().ToLowerInvariant() switch
        {
            "normal" or "up" => IconArrowUp,
            "borderline" or "neutral" => IconCircle,
            _ => IconArrowDown
        };
    }

    public static float StatusIconFontSize(string status, float textSize)
    {
        var normalizedStatus = status.Trim().ToLowerInvariant();
        return normalizedStatus is "borderline" or "neutral" ? textSize - 1f : textSize + 1f;
    }

    public static float HealthTrendIconFontSize(string trend)
    {
        var normalizedTrend = trend.Trim().ToLowerInvariant();
        return normalizedTrend is "up" or "normal" or "down" or "abnormal" ? 15f : 14f;
    }

    public static float ClinicalLevelIconFontSize(string level)
    {
        var normalizedLevel = level.Trim().ToLowerInvariant();
        return normalizedLevel is "up" or "normal" or "down" or "abnormal" ? 20f : 14f;
    }

    public static string DisplayStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return string.Empty;

        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(status.Trim().ToLowerInvariant());
    }
}
