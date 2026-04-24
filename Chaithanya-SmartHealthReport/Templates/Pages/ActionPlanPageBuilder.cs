using System.Text;
using SmartHealthReport.Models;
using SmartHealthReport.Templates.Components;
using static SmartHealthReport.Templates.Helpers.HtmlHelpers;

namespace SmartHealthReport.Templates.Pages;

public static class ActionPlanPageBuilder
{
    public static string Build(ReportData data)
    {
        var sb = new StringBuilder($"""
            <div class="page" id="page-action-plan">
                <div class="page-content">
                    {StepperComponent.Render("Action Plan")}
                    <h2 class="section-title">Your Parameters That Need Attention</h2>
                    <hr class="title-divider" style="width:80%;border-color:#ccc;">
            """);

        foreach (var a in data.ActionPlan)
        {
            string cardCls = a.Severity == "critical" ? "critical" : "warning";
            string valStr = !string.IsNullOrEmpty(a.Value) ? $": {Encode(a.Value)}" : "";

            sb.Append($"""
                <div class="action-card {cardCls}">
                    <div class="action-icon">{TestTubeIcon()}</div>
                    <h3 class="action-title">{Encode(a.ParameterName)}{valStr}</h3>
                    <p class="action-desc">{Encode(a.Description)}</p>
                    <div class="suggestion-icon">{SuggestionIcon()}</div>
                    <div class="suggestion-header">Suggestions</div>
                    <p class="suggestion-text">{Encode(a.Suggestion)}</p>
                </div>
                """);
        }

        sb.Append($"</div>{FooterComponent.Render()}</div>\n");
        return sb.ToString();
    }
}
