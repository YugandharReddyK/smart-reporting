using PdfGenerator.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Pdf.Sections;

internal static class HealthSummarySection
{
    public static void Compose(
        IDocumentContainer container,
        SmartReport report,
        List<HealthMetricRow> rows,
        bool includeIntro,
        string sectionLinkName,
        Action<PageDescriptor> configureStandardPage,
        Action<IContainer, int, string> composeProgressTracker,
        Action<IContainer, string, string> composeHeadingBlock,
        Action<IContainer> composeScoreCard,
        Action<IContainer, List<HealthMetricRow>> composeHealthMetricsTable)
    {
        container.Page(page =>
        {
            configureStandardPage(page);
            page.Header().Section(sectionLinkName).Element(c => composeProgressTracker(c, 0, sectionLinkName));
            page.Content().PaddingTop(20).Column(column =>
            {
                column.Spacing(18);
                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => composeHeadingBlock(c, report.HealthSummary.ScoreTitle, report.HealthSummary.ScoreDescription));
                    row.ConstantItem(240).Element(composeScoreCard);
                });

                if (includeIntro)
                    column.Item().Element(c => composeHeadingBlock(c, report.HealthSummary.SummaryTitle, report.HealthSummary.SummaryIntro));

                column.Item().Element(c => composeHealthMetricsTable(c, rows));
            });
        });
    }
}
