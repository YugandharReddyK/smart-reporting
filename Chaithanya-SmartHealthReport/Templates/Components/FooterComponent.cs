namespace SmartHealthReport.Templates.Components;

public static class FooterComponent
{
    public static string Render(bool showStripe = true)
    {
        var logoSvg = AssetLoader.Load("logo.svg");
        var stripe = showStripe
            ? $"""<div class="footer-stripe">{AssetLoader.Load("color-trend.svg")}</div>"""
            : string.Empty;
        return $"""
            <div class="logo">
                {logoSvg}
            </div>
            {stripe}
            """;
    }
}
