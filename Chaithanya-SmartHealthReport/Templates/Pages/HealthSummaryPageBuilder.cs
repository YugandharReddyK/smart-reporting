using System.Text;
using SmartHealthReport.Models;
using SmartHealthReport.Templates.Components;
using static SmartHealthReport.Templates.Helpers.HtmlHelpers;

namespace SmartHealthReport.Templates.Pages;

public static class HealthSummaryPageBuilder
{
    public static string Build(ReportData data)
    {
        var sb = new StringBuilder($"""
            <div class="page" id="page-health-summary">
                <div class="page-content">
                    {StepperComponent.Render("Health Summary")}
                    <h2 class="section-title">Health Score</h2>
                    <hr class="title-divider" style="width:55%;border-color:#ccc;">
                    <div class="score-section">
                        <div class="score-text">
                            <p style="font-size:13px;color:#000000;line-height:1.6;">This section summarizes your current health status as a single score out of 100, based on the findings in this report. A higher score indicates better overall health relative to the reference ranges used in this assessment.</p>
                        </div>
                        <div class="score-circle">
                            <div class="score-dots-container"></div>
                            <div class="gauge-wrapper">
                                <svg class="gauge-svg" width="200" height="200" viewBox="0 0 220 220">
                                    <defs>
                                        <linearGradient id="blueGaugeGradient" x1="0%" y1="0%" x2="100%" y2="0%">
                                            <stop offset="30%" stop-color="#73A6FB"/>
                                            <stop offset="70%" stop-color="#0041C2"/>
                                        </linearGradient>
                                    </defs>
                                    <circle cx="110" cy="110" r="100" fill="none" stroke="url(#blueGaugeGradient)" stroke-width="6" stroke-linecap="round" stroke-dasharray="628" stroke-dashoffset="0"/>
                                    <circle class="score-ring" cx="110" cy="110" r="100" fill="none" stroke="url(#blueGaugeGradient)" stroke-width="6" stroke-linecap="round" stroke-dasharray="628" stroke-dashoffset="628"/>
                                </svg>
                            </div>
                            <div class="score-value">
                                <span class="score-num num">0</span>
                                <span class="label">out of 100</span>
                            </div>
                        </div>
                    </div>
                    <h2 class="section-title">Your Health Summary</h2>
                    <hr class="title-divider" style="width:50%;border-color:#ccc;">
                    <p class="section-desc">Congratulations, We have successfully completed your health diagnosis. This is a big step towards staying on top of your health and identify potential to improve!</p>
                    <p class="section-desc">Here&#x2019;s a breakdown of your overall health picture.</p>
                    <table class="param-table">
                        <thead><tr><th>Parameter</th><th>Previous</th><th>Current</th><th>Trend</th></tr></thead>
                        <tbody>
            """);

        foreach (var p in data.HealthSummaryParameters)
        {
            sb.Append($"""<tr class="{RowClass(p.Trend)}"><td><span class="param-icon">{Encode(p.Icon)}</span> {Encode(p.Name)}</td><td>{p.Previous:0}</td><td>{p.Current:0}</td><td>{TrendHtml(p.Trend)}</td></tr>""");
            sb.Append('\n');
        }

        sb.Append($"</tbody></table></div>{FooterComponent.Render()}</div>\n");
        return sb.ToString();
    }
}
