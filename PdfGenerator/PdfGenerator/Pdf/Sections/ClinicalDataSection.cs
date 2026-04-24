using PdfGenerator.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Pdf.Sections;

internal static class ClinicalDataSection
{
    public static void Compose(
        IDocumentContainer container,
        SmartReport report,
        List<ClinicalDataGroup> groups,
        string sectionLinkName,
        Action<PageDescriptor> configureStandardPage,
        Action<IContainer, int, string> composeProgressTracker,
        Action<IContainer, ClinicalDataGroup> composeClinicalGroup,
        Func<string, float, float> headingRuleWidth)
    {
        container.Page(page =>
        {
            configureStandardPage(page);
            page.Header().Section(sectionLinkName).Element(c => composeProgressTracker(c, 3, sectionLinkName));
            page.Content().PaddingTop(22).Column(column =>
            {
                column.Spacing(22);
                column.Item().Column(header =>
                {
                    header.Spacing(3);
                    header.Item().Text(report.ClinicalData.Title).FontSize(22);
                    header.Item().Width(headingRuleWidth(report.ClinicalData.Title, 22)).LineHorizontal(1).LineColor("#CFCFCF");
                });

                foreach (var group in groups)
                    column.Item().Element(c => composeClinicalGroup(c, group));
            });
        });
    }
}
