using System.Text;
using SmartHealthReport.Models;
using SmartHealthReport.Services;
using SmartHealthReport.Templates;
using SmartHealthReport.Templates.Pages;

namespace SmartHealthReport.Services;

public static class HtmlTemplateBuilder
{
    // Cache the static head HTML (fonts, CSS, JS never change at runtime)
    private static readonly Lazy<string> _cachedHead = new(BuildHead);

    public static string Build(ReportData data)
    {
        var sb = new StringBuilder();
        sb.Append(_cachedHead.Value);
        sb.Append("<body>\n");

        // Pre-render charts once with SkiaSharp (defined once in CSS, reused across all iterations)
        var chartImages = ChartRenderer.RenderAllCharts(data);

        // Inject chart images as CSS background classes (each base64 appears once, not 19x)
        sb.Append("<style>\n");
        for (int i = 0; i < chartImages.Length; i++)
            sb.Append($".chart-img-{i}{{background-image:url('data:image/png;base64,{chartImages[i]}');background-size:100% 100%;background-repeat:no-repeat;}}\n");
        sb.Append("</style>\n");

            sb.Append(CoverPageBuilder.Build(data));
            sb.Append(HowToReadPageBuilder.Build());
            sb.Append(HealthSummaryPageBuilder.Build(data));
            sb.Append(TestSummaryPageBuilder.Build(data));
            sb.Append(ActionPlanPageBuilder.Build(data));
            sb.Append(ClinicalDataPageBuilder.Build(data));
            sb.Append(PersonalisedGuidancePageBuilder.Build(data, chartImages.Length));
            sb.Append(DoctorCheatSheetPageBuilder.Build(data));
        
        
        sb.Append(ScriptDataBuilder.Build(data));
        sb.Append("</body></html>");
        return sb.ToString();
    }

    private static string BuildHead()
    {
        var fontsCss = AssetLoader.Load("fonts.css");
        var css = AssetLoader.Load("report.css");
        var qrJs = AssetLoader.Load("qrcode.min.js");
        return "<!DOCTYPE html>\n"
            + "<html lang=\"en\">\n"
            + "<head>\n"
            + "    <meta charset=\"UTF-8\">\n"
            + "    <title>Smart Health Report</title>\n"
            + "    <script>\n" + qrJs + "\n    </script>\n"
            + "    <style>\n"
            + fontsCss + "\n"
            + css + "\n"
            + "    </style>\n"
            + "</head>\n";
    }
}
