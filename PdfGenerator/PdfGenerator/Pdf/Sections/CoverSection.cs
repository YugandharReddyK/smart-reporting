using PdfGenerator.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Pdf.Sections;

internal static class CoverSection
{
    public static void Compose(
        IDocumentContainer container,
        SmartReport report,
        string coverTitleFontFamily,
        string titleFontFamily,
        Action<IContainer> composeCoverFooter,
        Action<IContainer> composeCoverHeart)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(48);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial).FontColor(Colors.Grey.Darken2));
            page.Footer().Element(composeCoverFooter);

            page.Content().Column(column =>
            {
                column.Item().PaddingTop(14).Text(report.Cover.GreetingName).FontSize(19).FontColor("#8E8E8E");
                column.Item().PaddingTop(8).Width(430).LineHorizontal(2).LineColor("#FF1A12");
                column.Item().PaddingTop(16).Text(report.Cover.PrimaryTitle).FontFamily(coverTitleFontFamily, "Barlow Condensed", "Lato Thin", titleFontFamily, Fonts.Arial).FontSize(62).FontColor("#FF1A12");
                column.Item().PaddingTop(-4).Text(report.Cover.SecondaryTitle).FontFamily(coverTitleFontFamily, "Barlow Condensed", "Lato Thin", titleFontFamily, Fonts.Arial).FontSize(62).FontColor("#FF1A12");
                column.Item().PaddingTop(4).Width(430).LineHorizontal(2).LineColor("#FF1A12");
                column.Item().PaddingTop(8).Text(report.Patient.ReportId).FontSize(19).FontColor("#8E8E8E");
                column.Item().PaddingTop(56).AlignCenter().Width(320).Element(composeCoverHeart);
            });
        });
    }
}
