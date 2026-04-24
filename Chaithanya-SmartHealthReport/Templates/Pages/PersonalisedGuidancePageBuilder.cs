using System.Text;
using SmartHealthReport.Models;
using SmartHealthReport.Templates.Components;
using static SmartHealthReport.Templates.Helpers.HtmlHelpers;

namespace SmartHealthReport.Templates.Pages;

public static class PersonalisedGuidancePageBuilder
{
    public static string Build(ReportData data, int chartCount)
    {
        var items = data.PersonalisedGuidance;

        var sb = new StringBuilder($"""
            <div class="page" id="page-guidance">
                <div class="page-content">
                    {StepperComponent.Render("Personalised Suggestions")}
                    <h2 class="section-title">Personalised Guidance</h2>
                    <hr class="title-divider" style="width:50%;border-color:#ccc;">
                    <div class="charts-grid">
            """);

        // Top 2 charts
        for (int i = 0; i < Math.Min(2, items.Count); i++)
        {
            var item = items[i];
            sb.Append($"""
                <div class="chart-card">
                    <div class="chart-header">
                        <span class="chart-name">{Encode(item.TestName)}</span>
                        <span class="status-badge {StatusBadgeClass(item.Status)}"><span class="badge-dot">{TrendHtml(StatusTrend(item.Status))}</span> {StatusLabel(item.Status)}</span>
                    </div>
                    <div class="chart-canvas-wrap chart-img-{i}" style="width:320px;height:140px;"></div>
                </div>
                """);
        }
        sb.Append("</div>\n");

        // Description + tips from first item
        if (items.Count > 0)
        {
            var first = items[0];
            sb.Append($"""<p class="chart-desc">{Encode(first.Description)}</p>""");
            sb.Append('\n');

            sb.Append(BuildTipsSection(first));
        }

        // Bottom charts (3rd item onward)
        if (items.Count > 2)
        {
            sb.Append("""<div class="charts-grid lower-charts">""");
            for (int i = 2; i < items.Count; i++)
            {
                var item = items[i];
                sb.Append($"""
                    <div class="chart-card">
                        <div class="chart-header">
                            <span class="chart-name">{Encode(item.TestName)}</span>
                            <span class="status-badge {StatusBadgeClass(item.Status)}"><span class="badge-dot">{TrendHtml(StatusTrend(item.Status))}</span> {StatusLabel(item.Status)}</span>
                        </div>
                        <div class="chart-canvas-wrap chart-img-{i}" style="width:320px;height:140px;"></div>
                    </div>
                    """);
            }
            sb.Append("</div>\n");
        }

        sb.Append($"</div>{FooterComponent.Render()}</div>\n");
        return sb.ToString();
    }

    private static string BuildTipsSection(GuidanceItem item)
    {
        var sb = new StringBuilder();
        sb.Append("""<div class="tips-section"><h3>Diet &amp; Lifestyle Tips</h3><div class="tips-grid">""");

        var tipIcons = new[] { "exercise.svg", "obesidad.svg", "liquor.svg" };
        for (int i = 0; i < item.LifestyleTips.Count; i++)
        {
            string icon = i < tipIcons.Length ? SvgIcon(tipIcons[i], 40) : "";
            sb.Append($"""<div class="tip-card"><div class="tip-icon">{icon}</div><div class="tip-desc">{Encode(item.LifestyleTips[i])}</div></div>""");
        }
        sb.Append("</div>");

        var dietLabels = new[] { "Carbohydrates", "Protein", "Vegetables &amp; Fiber" };
        var dietIcons = new[] { "low-carb-diet_6192227.svg", "protein.svg", "vegetables.svg" };
        sb.Append("""<div class="tips-grid" style="margin-top:10px;">""");

        for (int i = 0; i < item.DietTips.Count && i < dietLabels.Length; i++)
        {
            sb.Append($"""<div class="tip-card"><div class="tip-icon">{SvgIcon(dietIcons[i], 35)}</div><div><div class="tip-title">{dietLabels[i]}</div><div class="tip-desc">{Encode(item.DietTips[i])}</div></div></div>""");
        }
        sb.Append("</div></div>\n");
        return sb.ToString();
    }
}
