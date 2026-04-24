using System.Text;
using SmartHealthReport.Models;
using SmartHealthReport.Templates.Components;
using static SmartHealthReport.Templates.Helpers.HtmlHelpers;

namespace SmartHealthReport.Templates.Pages;

public static class TestSummaryPageBuilder
{
    public static string Build(ReportData data)
    {
        var sb = new StringBuilder($"""
            <div class="page" id="page-test-summary">
                <div class="page-content">
                    {StepperComponent.Render("Test Summary")}
                    <h2 class="section-title">Your Important Parameters at a Glance</h2>
                    <hr class="title-divider" style="width:85%;border-color:#ccc;">
                    <div class="cards-grid">
            """);

        foreach (var c in data.TestSummaryCards)
        {
            sb.Append($"""
                <div class="test-card {CardClass(c.Status)}">
                    <div class="card-header">
                        <span class="card-name"><span class="icon">{DropletIcon()}</span> {Encode(c.Name)}</span>
                        <span class="status-badge {StatusBadgeClass(c.Status)}">{TrendSymbol(c.Trend)} {StatusLabel(c.Status)}</span>
                    </div>
                    <p class="card-value">Value: <strong>{Encode(c.Value)}{Encode(c.Unit)}</strong></p>
                    <p class="card-range">Range: <strong>{Encode(c.Range)}</strong></p>
                </div>
                """);
        }

        sb.Append($"</div></div>{FooterComponent.Render()}</div>\n");
        return sb.ToString();
    }
}
