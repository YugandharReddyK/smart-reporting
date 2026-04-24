namespace PdfGenerator.Pdf.Shared;

internal static class ReportTheme
{
    public static StatusPalette GetPalette(string status)
    {
        return status.Trim().ToLowerInvariant() switch
        {
            "normal" or "up" => new StatusPalette("#78F2A0", "#0A9F21", "#0A9F21"),
            "borderline" or "neutral" => new StatusPalette("#FFF3A3", "#A78600", "#A78600"),
            _ => new StatusPalette("#F8A8AE", "#B10000", "#B10000")
        };
    }
}

internal readonly record struct StatusPalette(string Background, string Pill, string Strong);
