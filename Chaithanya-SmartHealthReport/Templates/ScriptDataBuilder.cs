using System.Text;
using SmartHealthReport.Models;
using static SmartHealthReport.Templates.Helpers.HtmlHelpers;

namespace SmartHealthReport.Templates;

public static class ScriptDataBuilder
{
    public static string Build(ReportData data)
    {
        var reportJs = AssetLoader.Load("report.js");
        var qrUrl = $"https://reports.agilus.in/{JsEscape(data.ReportId)}";

        var sb = new StringBuilder();
        sb.Append("<script>\n");
        sb.Append($"window.__reportData = {{ healthScore: {data.HealthScore}, qrUrl: \"{qrUrl}\" }};\n");
        sb.Append("</script>\n");
        sb.Append($"<script>\n{reportJs}\n</script>\n");
        return sb.ToString();
    }
}
