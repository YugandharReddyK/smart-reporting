using PdfGenerator.Models;
using PdfGenerator.Pdf.Graphics;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Pdf.Sections;

internal static class CheatSheetSection
{
    public static void Compose(
        IDocumentContainer container,
        SmartReport report,
        string sectionLinkName,
        string bodyFontFamily,
        Action<PageDescriptor> configureStandardPage,
        Action<IContainer, int, string> composeProgressTracker,
        Func<string, float, float> headingRuleWidth,
        Func<string, byte[]?> generateQrCode)
    {
        container.Page(page =>
        {
            configureStandardPage(page);
            page.Header().Section(sectionLinkName).Element(c => composeProgressTracker(c, 5, sectionLinkName));
            page.Content().PaddingTop(22).Column(column =>
            {
                column.Spacing(22);
                column.Item().Column(header =>
                {
                    header.Spacing(3);
                    header.Item().Text(report.CheatSheet.Title).FontSize(16).Bold();
                    header.Item().Width(headingRuleWidth(report.CheatSheet.Title, 16)).LineHorizontal(1).LineColor("#CFCFCF");
                });

                column.Item().PaddingHorizontal(4).Layers(layers =>
                {
                    layers.Layer().PaddingTop(8).Svg(size => SvgIconFactory.BuildRoundedDashedBorderSvg(size));

                    layers.PrimaryLayer().PaddingTop(50).PaddingBottom(14).PaddingHorizontal(18).Column(box =>
                    {
                        box.Spacing(12);

                        foreach (var question in report.CheatSheet.Questions)
                        {
                            box.Item().Row(row =>
                            {
                                row.ConstantItem(34).Height(34).Border(3).BorderColor(Colors.Black).CornerRadius(11);
                                row.ConstantItem(14);
                                row.RelativeItem().PaddingTop(2).Text(question).FontFamily(bodyFontFamily, Fonts.Arial).FontSize(14).LineHeight(1.3f);
                            });
                        }
                    });

                    layers.Layer().PaddingLeft(20).PaddingRight(20).AlignTop().Row(top =>
                    {
                        top.ConstantItem(275)
                            .Background(Colors.Black)
                            .CornerRadius(18)
                            .PaddingVertical(6)
                            .PaddingHorizontal(12)
                            .AlignMiddle()
                            .Text(report.CheatSheet.PromptTitle)
                            .FontFamily(bodyFontFamily, Fonts.Arial)
                            .FontColor(Colors.White)
                            .FontSize(13);
                    });
                });

                column.Item().AlignCenter().Text(report.CheatSheet.QrTitle).FontSize(15).FontColor("#2583D2").SemiBold();

                var qrBytes = generateQrCode(report.CheatSheet.QrUrl);
                if (qrBytes is not null)
                    column.Item().AlignCenter().Width(140).Height(140).Image(qrBytes);

                if (!string.IsNullOrWhiteSpace(report.CheatSheet.QrUrl))
                    column.Item().AlignCenter().Text(report.CheatSheet.QrUrl).FontSize(10).FontColor("#2583D2");
            });
        });
    }
}
