using System.Text;
using SmartHealthReport.Models;
using SmartHealthReport.Templates.Components;
using static SmartHealthReport.Templates.Helpers.HtmlHelpers;

namespace SmartHealthReport.Templates.Pages;

public static class ClinicalDataPageBuilder
{
    public static string Build(ReportData data)
    {
        var sb = new StringBuilder($"""
            <div class="page" id="page-clinical-data">
                <div class="page-content">
                    {StepperComponent.Render("Clinical Data")}
                    <h2 class="section-title">Clinical Data</h2>
                    <hr class="title-divider" style="width:30%;border-color:#ccc;">
            """);

        foreach (var cat in data.ClinicalData)
        {
            sb.Append($"""
                <div class="clinical-category">
                    <div class="category-header">
                        <span class="category-name"><span class="category-icon">{DropletIcon(16)}</span>{Encode(cat.CategoryName)}</span>
                        <span class="status-badge {StatusBadgeClass(cat.OverallStatus)}"><span class="badge-dot">{TrendHtml(cat.OverallStatus)}</span> {StatusLabel(cat.OverallStatus)}</span>
                    </div>
                    <table class="clinical-table">
                        <thead><tr><th>Test Name</th><th>Result</th><th>Unit</th><th>Range</th><th>Level</th></tr></thead>
                        <tbody>
                """);

            foreach (var t in cat.Tests)
            {
                string rowCls = t.Level switch { "normal" => "row-normal", "abnormal" => "row-abnormal", _ => "row-borderline" };
                string trendKey = t.Level switch { "normal" => "up", "abnormal" => "down", _ => "same" };
                string trendHtml = TrendHtml(trendKey);
                string rangeText = t.Range.Replace(" | ", "\n");
                sb.Append($"""<tr class="{rowCls}"><td>{Encode(t.TestName)}</td><td>{Encode(t.Result)}</td><td>{Encode(t.Unit)}</td><td class="range-cell">{Encode(rangeText)}</td><td>{trendHtml}</td></tr>""");
                sb.Append('\n');
            }

            sb.Append("</tbody></table></div>\n");
        }

        sb.Append($"</div>{FooterComponent.Render()}</div>\n");
        return sb.ToString();
    }
}
