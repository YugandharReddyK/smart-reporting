namespace PdfGenerator.Pdf.Shared;

internal static class LayoutHelper
{
    public static float HeadingRuleWidth(string heading, float fontSize)
    {
        if (string.IsNullOrWhiteSpace(heading))
            return 120f;

        var condensedHeading = heading.Replace("\n", " ").Trim();
        var estimatedWidth = condensedHeading.Length * fontSize * 0.55f;
        return Math.Clamp(estimatedWidth, 120f, 430f);
    }
}
