using QuestPDF.Infrastructure;
using System.Globalization;

namespace PdfGenerator.Pdf.Graphics;

internal static class SvgIconFactory
{
    public static string BuildScoreGradientRingSvg()
    {
        return """
            <svg width="146" height="146" viewBox="0 0 146 146" xmlns="http://www.w3.org/2000/svg">
              <defs>
                <linearGradient id="scoreGrad" x1="1" y1="0" x2="0" y2="1">
                  <stop offset="0%" stop-color="#1083D5"/>
                  <stop offset="60%" stop-color="#7FAFF1"/>
                  <stop offset="100%" stop-color="#C8DEF7"/>
                </linearGradient>
              </defs>
              <circle cx="73" cy="73" r="70" fill="none" stroke="url(#scoreGrad)" stroke-width="5.5"/>
            </svg>
            """;
    }

    public static string BuildBulbOnIconSvg()
    {
        return """
            <svg width="16" height="16" viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg">
              <line x1="8" y1="0.7" x2="8" y2="2.4" stroke="#111" stroke-width="1.15" stroke-linecap="round"/>
              <line x1="2.2" y1="2.3" x2="3.4" y2="3.5" stroke="#111" stroke-width="1.15" stroke-linecap="round"/>
              <line x1="13.8" y1="2.3" x2="12.6" y2="3.5" stroke="#111" stroke-width="1.15" stroke-linecap="round"/>
              <line x1="0.8" y1="7.1" x2="2.5" y2="7.1" stroke="#111" stroke-width="1.15" stroke-linecap="round"/>
              <line x1="13.5" y1="7.1" x2="15.2" y2="7.1" stroke="#111" stroke-width="1.15" stroke-linecap="round"/>
              <path d="M6.1 11.2C6.1 9.8 4.6 8.8 4.6 6.9C4.6 5.1 6.1 3.6 8 3.6C9.9 3.6 11.4 5.1 11.4 6.9C11.4 8.8 9.9 9.8 9.9 11.2Z" fill="#111"/>
              <rect x="6.2" y="11.2" width="3.6" height="1.2" rx="0.45" fill="#111"/>
              <rect x="6.5" y="12.6" width="3" height="1" rx="0.4" fill="#111"/>
            </svg>
            """;
    }

    public static string BuildTrackerStepIconSvg(int index)
    {
        var pathMarkup = index switch
        {
            0 => "<rect x='2.8' y='2.3' width='10.4' height='11.2' rx='1.8' fill='#FFFFFF'/>"
                + "<path d='M6 5.1c.33-.47 1.08-.44 1.36.06.22.38.08.84-.24 1.14L6 7.25l-1.12-.94c-.32-.3-.46-.76-.24-1.14.28-.5 1.03-.53 1.36-.06z' fill='#1083D5'/>"
                + "<line x1='4.9' y1='9.55' x2='11.1' y2='9.55' stroke='#1083D5' stroke-width='1.15' stroke-linecap='round'/>"
                + "<line x1='4.9' y1='11.2' x2='9.8' y2='11.2' stroke='#1083D5' stroke-width='1.15' stroke-linecap='round'/>",
            1 => "<path d='M3.2 2.25h3.9v1.55h-.7v8.05a1.35 1.35 0 0 1-1.35 1.35H5.2a1.35 1.35 0 0 1-1.35-1.35V3.8h-.65z' fill='none' stroke='#FFFFFF' stroke-width='1.35' stroke-linejoin='round'/>"
                + "<path d='M8.95 2.25h3.9v1.55h-.7v8.05a1.35 1.35 0 0 1-1.35 1.35h-.1a1.35 1.35 0 0 1-1.35-1.35V3.8h-.65z' fill='none' stroke='#FFFFFF' stroke-width='1.35' stroke-linejoin='round'/>"
                + "<rect x='4.45' y='4.15' width='1.65' height='7.9' rx='0.55' fill='#FFFFFF'/>"
                + "<rect x='10.2' y='9.85' width='1.65' height='2.2' rx='0.55' fill='#FFFFFF'/>"
                + "<line x1='3.95' y1='6.1' x2='4.9' y2='6.1' stroke='#1083D5' stroke-width='1.05' stroke-linecap='round'/>"
                + "<line x1='3.95' y1='8.15' x2='4.9' y2='8.15' stroke='#1083D5' stroke-width='1.05' stroke-linecap='round'/>"
                + "<line x1='9.7' y1='6.35' x2='10.7' y2='6.35' stroke='#FFFFFF' stroke-width='1.05' stroke-linecap='round'/>"
                + "<line x1='9.7' y1='8.4' x2='10.7' y2='8.4' stroke='#FFFFFF' stroke-width='1.05' stroke-linecap='round'/>",
            2 => "<path d='M8 1.8l1.84 3.77 4.16.6-3 2.94.71 4.15L8 11.3l-3.71 1.96.71-4.15-3-2.94 4.16-.6z' fill='#FFFFFF'/>",
            3 => "<rect x='2.2' y='2.6' width='11.6' height='10.8' rx='1.6' fill='none' stroke='#FFFFFF' stroke-width='1.5'/>"
                + "<line x1='2.2' y1='6.2' x2='13.8' y2='6.2' stroke='#FFFFFF' stroke-width='1.25'/>"
                + "<line x1='2.2' y1='9.8' x2='13.8' y2='9.8' stroke='#FFFFFF' stroke-width='1.25'/>"
                + "<line x1='8' y1='2.6' x2='8' y2='13.4' stroke='#FFFFFF' stroke-width='1.25'/>",
            4 => "<path d='M8 13c-.3 0-.57-.1-.79-.29-3.38-2.95-5.15-4.5-5.15-6.93 0-1.89 1.5-3.38 3.4-3.38 1.08 0 2.05.51 2.54 1.31.49-.8 1.46-1.31 2.54-1.31 1.9 0 3.4 1.49 3.4 3.38 0 2.43-1.77 3.98-5.15 6.93-.22.19-.49.29-.79.29z' fill='#FFFFFF'/>",
            5 => "<path d='M10.95 5.1L6.6 9.45a2.2 2.2 0 1 1-3.11-3.11l4.8-4.8a2.75 2.75 0 0 1 3.89 3.89l-5.1 5.1a1.38 1.38 0 0 1-1.95-1.95L9.7 4.01' fill='none' stroke='#FFFFFF' stroke-width='1.5' stroke-linecap='round' stroke-linejoin='round'/>",
            _ => string.Empty
        };

        return FormattableString.Invariant($"""
            <svg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 16 16'>
                {pathMarkup}
            </svg>
            """);
    }

    public static string BuildTransparentDotHitAreaSvg(float size)
    {
        var sizeText = size.ToString("0.###", CultureInfo.InvariantCulture);

        return FormattableString.Invariant($"""
            <svg xmlns='http://www.w3.org/2000/svg' width='{sizeText}' height='{sizeText}' viewBox='0 0 {sizeText} {sizeText}'>
                <rect x='0' y='0' width='{sizeText}' height='{sizeText}' fill='#FFFFFF' fill-opacity='0.01'/>
            </svg>
            """);
    }

    public static string BuildRoundedDashedBorderSvg(Size size)
    {
        const float strokeWidth = 3.3f;
        const float cornerRadius = 36f;
        const float dashLength = 15f;
        const float dashGap = 12f;

        var width = Math.Max(0f, size.Width - strokeWidth);
        var height = Math.Max(0f, size.Height - strokeWidth);
        var radius = Math.Min(cornerRadius, Math.Min(width, height) / 2f);
        var x = strokeWidth / 2f;
        var y = strokeWidth / 2f;

        return FormattableString.Invariant($"""
            <svg xmlns='http://www.w3.org/2000/svg' width='{size.Width}' height='{size.Height}' viewBox='0 0 {size.Width} {size.Height}'>
                <rect x='{x}' y='{y}' width='{width}' height='{height}' rx='{radius}' ry='{radius}' fill='none' stroke='#2C2C2C' stroke-width='{strokeWidth}' stroke-dasharray='{dashLength} {dashGap}' />
            </svg>
            """);
    }
}
