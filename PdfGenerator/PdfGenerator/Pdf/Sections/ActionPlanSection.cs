using PdfGenerator.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Pdf.Sections;

internal static class ActionPlanSection
{
    public static void Compose(
        IDocumentContainer container,
        SmartReport report,
        List<ActionPlanCard> cards,
        string sectionLinkName,
        Action<PageDescriptor> configureStandardPage,
        Action<IContainer, int, string> composeProgressTracker,
        Action<IContainer, ActionPlanCard> composeActionCard,
        Func<string, float, float> headingRuleWidth)
    {
        container.Page(page =>
        {
            configureStandardPage(page);
            page.Header().Section(sectionLinkName).Element(c => composeProgressTracker(c, 2, sectionLinkName));
            page.Content().PaddingTop(22).Column(column =>
            {
                column.Spacing(18);
                column.Item().Column(header =>
                {
                    header.Spacing(3);
                    header.Item().Text(report.ActionPlan.Title).FontSize(22);
                    header.Item().Width(headingRuleWidth(report.ActionPlan.Title, 22)).LineHorizontal(1).LineColor("#CFCFCF");
                });

                foreach (var card in cards)
                    column.Item().Element(c => composeActionCard(c, card));
            });
        });
    }
}
