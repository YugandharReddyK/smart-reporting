using PdfGenerator.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Pdf.Sections;

internal static class TestSummarySection
{
    public static void Compose(
        IDocumentContainer container,
        SmartReport report,
        List<TestSummaryCard> cards,
        string sectionLinkName,
        Action<PageDescriptor> configureStandardPage,
        Action<IContainer, int, string> composeProgressTracker,
        Action<IContainer, TestSummaryCard> composeTestCard,
        Func<string, float, float> headingRuleWidth)
    {
        container.Page(page =>
        {
            configureStandardPage(page);
            page.Header().Section(sectionLinkName).Element(c => composeProgressTracker(c, 1, sectionLinkName));
            page.Content().PaddingTop(22).Column(column =>
            {
                column.Spacing(18);
                column.Item().Column(header =>
                {
                    header.Spacing(3);
                    header.Item().Text(report.TestSummary.Title).FontSize(22);
                    header.Item().Width(headingRuleWidth(report.TestSummary.Title, 22)).LineHorizontal(1).LineColor("#CFCFCF");
                });

                for (var index = 0; index < cards.Count; index += 2)
                {
                    var left = cards[index];
                    TestSummaryCard? right = index + 1 < cards.Count ? cards[index + 1] : null;

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => composeTestCard(c, left));
                        row.ConstantItem(20);
                        row.RelativeItem().Element(c =>
                        {
                            if (right is not null)
                                composeTestCard(c, right);
                        });
                    });
                }
            });
        });
    }
}
