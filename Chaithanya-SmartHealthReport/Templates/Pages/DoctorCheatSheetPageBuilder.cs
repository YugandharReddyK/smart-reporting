using System.Text;
using SmartHealthReport.Models;
using SmartHealthReport.Templates.Components;
using static SmartHealthReport.Templates.Helpers.HtmlHelpers;

namespace SmartHealthReport.Templates.Pages;

public static class DoctorCheatSheetPageBuilder
{
    public static string Build(ReportData data)
    {
        var sb = new StringBuilder($"""
            <div class="page" id="page-cheatsheet">
                <div class="page-content">
                    {StepperComponent.Render("Doctor Cheat-sheet")}
                    <h2 class="section-title">Cheat-Sheet</h2>
                    <hr class="title-divider" style="width:180px;border-color:#ccc;">
                    <div class="cheat-card">
                        <div class="cheat-tab">Things you can ask the doctor</div>
                        <ul class="cheat-questions">
            """);

        foreach (var q in data.DoctorQuestions)
        {
            sb.Append($"""<li><div class="checkbox-icon"></div><span>{Encode(q)}</span></li>""");
            sb.Append('\n');
        }

        sb.Append($"""
                        </ul>
                    </div>
                    <div class="qr-section">
                        <h3>Scan to View Your Report Online</h3>
                        <div class="qr-code"></div>
                        <p class="qr-url">{Encode(data.ReportUrl)}</p>
                    </div>
                </div>
                {FooterComponent.Render()}
            </div>
            """);
        return sb.ToString();
    }
}
