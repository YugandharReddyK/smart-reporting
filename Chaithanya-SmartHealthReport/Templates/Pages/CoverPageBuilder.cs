using SmartHealthReport.Models;
using SmartHealthReport.Templates.Components;
using static SmartHealthReport.Templates.Helpers.HtmlHelpers;

namespace SmartHealthReport.Templates.Pages;

public static class CoverPageBuilder
{
    public static string Build(ReportData data)
    {
        var heartSvg = AssetLoader.Load("heart.svg");

        return $"""
            <div class="page" id="page-cover">
                <div class="page-content">
                    <p class="cover-patient">{Encode(data.Patient.Name)}</p>
                    <hr class="cover-divider">
                    <h1 class="cover-title">SMART<br/>HEALTH REPORT</h1>
                    <hr class="cover-divider">
                    <p class="cover-id">{Encode(data.ReportId)}</p>
                    <div class="cover-heart">
                        {heartSvg}
                    </div>
                </div>
                <div class="first-page-logo">{FooterComponent.Render(showStripe: false)}</div>
            </div>
            """;
    }
}
