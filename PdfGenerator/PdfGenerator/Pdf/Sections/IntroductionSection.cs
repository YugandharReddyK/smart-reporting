using PdfGenerator.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Pdf.Sections;

internal static class IntroductionSection
{
    public static void Compose(
        IDocumentContainer container,
        SmartReport report,
        Action<PageDescriptor> configureStandardPage,
        Action<IContainer, string, string> composeHeadingBlock)
    {
        container.Page(page =>
        {
            configureStandardPage(page);
            page.Content().PaddingTop(24).Column(column =>
            {
                column.Spacing(16);
                column.Item().Element(c => composeHeadingBlock(c, report.Introduction.AnalyticsTitle, report.Introduction.AnalyticsText));
                column.Item().Element(c => composeHeadingBlock(c, report.Introduction.ReadTitle, report.Introduction.ReadText));

                foreach (var item in report.Introduction.OverviewItems)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(itemColumn =>
                        {
                            itemColumn.Spacing(4);
                            itemColumn.Item().Text($"▪  {item.Title}").FontSize(11).FontColor("#2583D2");
                            itemColumn.Item().PaddingLeft(14).Text(item.Description).FontSize(9.5f).LineHeight(1.3f);
                        });
                        row.ConstantItem(58).Height(58).Background("#E5E5E5");
                    });
                }
            });
        });
    }
}
