using PdfGenerator.Models;
using PdfGenerator.Pdf.Shared;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Pdf.Sections;

internal static class GuidanceSection
{
    public static void Compose(
        IDocumentContainer container,
        SmartReport report,
        List<GuidanceChart> charts,
        bool includeTips,
        string sectionLinkName,
        Action<PageDescriptor> configureStandardPage,
        Action<IContainer, int, string> composeProgressTracker,
        Action<IContainer, GuidanceChart> composeGuidanceChart,
        Func<string, string, string?> guidanceTipSvgPath,
        Func<string, string> condenseTipDescription,
        Func<string, float, float> headingRuleWidth,
        string fallbackTipIcon)
    {
        container.Page(page =>
        {
            configureStandardPage(page);
            page.Header().Section(sectionLinkName).Element(c => composeProgressTracker(c, 4, sectionLinkName));
            page.Content().PaddingTop(22).Column(column =>
            {
                column.Spacing(20);
                column.Item().Column(header =>
                {
                    header.Spacing(3);
                    header.Item().Text(report.PersonalisedGuidance.Title).FontSize(22);
                    header.Item().Width(headingRuleWidth(report.PersonalisedGuidance.Title, 22)).LineHorizontal(1).LineColor("#CFCFCF");
                });

                var topCharts = charts.Take(2).ToList();
                var bottomCharts = charts.Skip(2).Take(2).ToList();

                if (topCharts.Count > 0)
                    column.Item().Element(c => ComposeChartRow(c, topCharts, composeGuidanceChart));

                if (includeTips && report.PersonalisedGuidance.Tips.Count > 0)
                {
                    column.Item().Text(report.PersonalisedGuidance.TipsTitle).FontSize(16);

                    for (var index = 0; index < report.PersonalisedGuidance.Tips.Count; index += 3)
                    {
                        var items = report.PersonalisedGuidance.Tips.Skip(index).Take(3).ToList();
                        column.Item().Row(row =>
                        {
                            for (var itemIndex = 0; itemIndex < items.Count; itemIndex++)
                            {
                                var item = items[itemIndex];
                                row.RelativeItem().Column(tipColumn =>
                                {
                                    var iconSvgPath = guidanceTipSvgPath(item.Title, item.Description);
                                    var shortDescription = condenseTipDescription(item.Description);

                                    tipColumn.Item().Row(tipRow =>
                                    {
                                        tipRow.ConstantItem(28)
                                            .Width(28)
                                            .Height(28)
                                            .AlignMiddle()
                                            .AlignCenter()
                                            .Element(iconContainer =>
                                            {
                                                if (!string.IsNullOrWhiteSpace(iconSvgPath))
                                                    iconContainer.Padding(2).Svg(iconSvgPath).FitArea();
                                                else
                                                    iconContainer.Text(fallbackTipIcon).FontSize(11).AlignCenter();
                                            });
                                        tipRow.RelativeItem().PaddingLeft(6).Text($"{item.Title} - {shortDescription}").FontSize(9).LineHeight(1.25f);
                                    });
                                });

                                if (itemIndex < items.Count - 1)
                                    row.ConstantItem(18);
                            }
                        });
                    }
                }

                if (bottomCharts.Count > 0)
                    column.Item().Element(c => ComposeChartRow(c, bottomCharts, composeGuidanceChart));
            });
        });
    }

    private static void ComposeChartRow(IContainer container, List<GuidanceChart> charts, Action<IContainer, GuidanceChart> composeGuidanceChart)
    {
        container.Row(row =>
        {
            for (var index = 0; index < charts.Count; index++)
            {
                var chart = charts[index];
                row.RelativeItem().Element(c => composeGuidanceChart(c, chart));

                if (index < charts.Count - 1)
                    row.ConstantItem(20);
            }
        });
    }

}
