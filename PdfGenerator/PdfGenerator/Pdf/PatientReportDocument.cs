using PdfGenerator.Models;
using PdfGenerator.Pdf.Graphics;
using PdfGenerator.Pdf.Sections;
using PdfGenerator.Pdf.Shared;
using QRCoder;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;

namespace PdfGenerator.Pdf;

public sealed class PatientReportDocument(SmartReport report) : IDocument
{
    private static readonly bool DocumentFontsLoaded = TryRegisterDocumentFonts();

    private static readonly string[] SectionLinkNames =
    [
        "HealthSummary",
        "TestSummary",
        "ActionPlan",
        "ClinicalData",
        "PersonalisedGuidance",
        "CheatSheet"
    ];
    private const string TitleFontFamily = "Bebas Neue";
    private const string CoverTitleFontFamily = "Exo 2 Thin";
    private const string BodyFontFamily = "Patrick Hand";

    private const string IconDroplet = "\uf043";
    private const string IconFlask = "\uf0c3";
    private const string IconVials = "\uf493";
    private const string IconLightbulb = "\uf0eb";
    private const string IconBolt = "\uf0e7";
    private const string IconCircle = "\uf111";

    private static readonly string[] StepTitles =
    [
        "Health\nSummary",
        "Test\nSummary",
        "Action\nPlan",
        "Clinical\nData",
        "Personalised\nSuggestions",
        "Doctor\nCheat-sheet"
    ];

    private readonly string? _logoSvgPath = IsSvgAsset(report.Branding.LogoPath) ? TryResolveAssetPath(report.Branding.LogoPath) : null;
    private readonly string? _footerLogoSvgPath = TryResolveAssetPath("assets/logo-footer.svg");
    private readonly byte[]? _logoBytes = IsSvgAsset(report.Branding.LogoPath) ? null : TryLoadAsset(report.Branding.LogoPath);
    private readonly byte[]? _coverHeartBytes = TryLoadAsset(report.Branding.CoverHeartPath);
    private readonly byte[]? _footerRibbonBytes = TryLoadAsset(report.Branding.FooterRibbonPath);
    private readonly string? _scoreHaloSvgPath = TryResolveAssetPath("assets/score-halo.svg");
    private readonly string? _tipExerciseIconSvgPath = TryResolveAssetPath("assets/tips/exercise.svg");
    private readonly string? _tipLoseWeightIconSvgPath = TryResolveAssetPath("assets/tips/lose-weight.svg");
    private readonly string? _tipAvoidSweetsIconSvgPath = TryResolveAssetPath("assets/tips/avoid-sweets.svg");
    private readonly string? _tipCarbohydratesIconSvgPath = TryResolveAssetPath("assets/tips/carbohydrates.svg");
    private readonly string? _tipProteinIconSvgPath = TryResolveAssetPath("assets/tips/protein.svg");
    private readonly string? _tipVegetablesFiberIconSvgPath = TryResolveAssetPath("assets/tips/vegetables.svg");
    private readonly string? _actionTestTubeIconSvgPath = TryResolveAssetPath("assets/tips/action-test-tube.svg");
    private readonly string? _actionSuggestionIconSvgPath = TryResolveAssetPath("assets/tips/suggestion-brain.svg");

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        _ = DocumentFontsLoaded;

        ComposeCoverPage(container);
        ComposeIntroductionPage(container);

        var healthChunks = Chunk(report.HealthSummary.Rows, 6);
        for (var index = 0; index < healthChunks.Count; index++)
            ComposeHealthSummaryPage(container, healthChunks[index], index == 0);

        ComposeTestSummaryPage(container, report.TestSummary.Cards);

        foreach (var cardChunk in Chunk(report.ActionPlan.Cards, 2))
            ComposeActionPlanPage(container, cardChunk);

        foreach (var groupChunk in Chunk(report.ClinicalData.Groups, 2))
            ComposeClinicalDataPage(container, groupChunk);

        var guidanceChartChunks = Chunk(report.PersonalisedGuidance.Charts, 4);
        if (guidanceChartChunks.Count == 0)
        {
            ComposeGuidancePage(container, report.PersonalisedGuidance.Charts, true);
        }
        else
        {
            for (var index = 0; index < guidanceChartChunks.Count; index++)
                ComposeGuidancePage(container, guidanceChartChunks[index], includeTips: index == 0);
        }

        ComposeCheatSheetPage(container);
    }

    private void ComposeCoverPage(IDocumentContainer container)
    {
        CoverSection.Compose(
            container,
            report,
            CoverTitleFontFamily,
            TitleFontFamily,
            ComposeCoverFooter,
            ComposeCoverHeart);
    }

    private void ComposeIntroductionPage(IDocumentContainer container)
    {
        IntroductionSection.Compose(
            container,
            report,
            ConfigureStandardPage,
            ComposeHeadingBlock);
    }

    private void ComposeHealthSummaryPage(IDocumentContainer container, List<HealthMetricRow> rows, bool includeIntro)
    {
        HealthSummarySection.Compose(
            container,
            report,
            rows,
            includeIntro,
            SectionLinkNames[0],
            ConfigureStandardPage,
            ComposeProgressTracker,
            ComposeHeadingBlock,
            ComposeScoreCard,
            ComposeHealthMetricsTable);
    }

    private void ComposeTestSummaryPage(IDocumentContainer container, List<TestSummaryCard> cards)
    {
        TestSummarySection.Compose(
            container,
            report,
            cards,
            SectionLinkNames[1],
            ConfigureStandardPage,
            ComposeProgressTracker,
            ComposeTestCard,
            LayoutHelper.HeadingRuleWidth);
    }

    private void ComposeActionPlanPage(IDocumentContainer container, List<ActionPlanCard> cards)
    {
        ActionPlanSection.Compose(
            container,
            report,
            cards,
            SectionLinkNames[2],
            ConfigureStandardPage,
            ComposeProgressTracker,
            ComposeActionCard,
            LayoutHelper.HeadingRuleWidth);
    }

    private void ComposeClinicalDataPage(IDocumentContainer container, List<ClinicalDataGroup> groups)
    {
        ClinicalDataSection.Compose(
            container,
            report,
            groups,
            SectionLinkNames[3],
            ConfigureStandardPage,
            ComposeProgressTracker,
            ComposeClinicalGroup,
            LayoutHelper.HeadingRuleWidth);
    }

    private void ComposeGuidancePage(IDocumentContainer container, List<GuidanceChart> charts, bool includeTips)
    {
        GuidanceSection.Compose(
            container,
            report,
            charts,
            includeTips,
            SectionLinkNames[4],
            ConfigureStandardPage,
            ComposeProgressTracker,
            ComposeGuidanceChart,
            GuidanceTipSvgPath,
            CondenseTipDescription,
            LayoutHelper.HeadingRuleWidth,
            IconLightbulb);
    }

    private void ComposeCheatSheetPage(IDocumentContainer container)
    {
        CheatSheetSection.Compose(
            container,
            report,
            SectionLinkNames[5],
            BodyFontFamily,
            ConfigureStandardPage,
            ComposeProgressTracker,
            LayoutHelper.HeadingRuleWidth,
            GenerateQrCode);
    }

    private void ConfigureStandardPage(PageDescriptor page)
    {
        page.Size(PageSizes.A4);
        page.Margin(36);
        page.MarginBottom(10);
        page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial, "Segoe UI Emoji", "Font Awesome 6 Free Solid").FontColor(Colors.Black));
        page.Footer().Element(ComposeStandardFooter);
    }

    private void ComposeCoverFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.ConstantItem(190).AlignBottom().Element(ComposeLogo);
            row.RelativeItem();
        });
    }

    private void ComposeStandardFooter(IContainer container)
    {
        container.PaddingRight(-36).Row(row =>
        {
            row.ConstantItem(100).PaddingBottom(10).AlignBottom().Element(ComposeFooterLogo);
            row.RelativeItem();
            row.ConstantItem(430).AlignRight().AlignBottom().Height(48).Element(ComposeFooterRibbon);
        });
    }

    private void ComposeFooterLogo(IContainer container)
    {
        var brandedLogoPath = report.Branding.LogoPath.Replace('\\', '/');
        if (!string.IsNullOrWhiteSpace(_logoSvgPath))
        {
            if (brandedLogoPath.Equals("assets/logo.svg", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(_footerLogoSvgPath))
            {
                container.Svg(_footerLogoSvgPath).FitArea();
                return;
            }

            container.Svg(_logoSvgPath).FitArea();
            return;
        }

        if (_logoBytes is null)
        {
            container.Text(report.Branding.OrganisationName).FontSize(18).FontColor("#1083D5").SemiBold();
            return;
        }

        container.Image(_logoBytes);
    }

    private void ComposeProgressTracker(IContainer container, int activeIndex, string currentSection)
    {
        const int trackerSlotSize = 32;

        container.PaddingTop(4).PaddingBottom(2).Column(column =>
        {
            column.Spacing(5);
            column.Item().Layers(layers =>
            {
                layers.Layer().PaddingTop(trackerSlotSize / 2f).PaddingHorizontal(44).LineHorizontal(1.2f).LineColor("#C3C3C3").LineDashPattern([3, 3]);

                layers.PrimaryLayer().Row(row =>
                {
                    for (var index = 0; index < StepTitles.Length; index++)
                    {
                        var isActive = index == activeIndex;
                        var dotSize = isActive ? trackerSlotSize : 16;
                        var hitAreaSize = isActive ? dotSize : 24;

                        row.RelativeItem().Height(trackerSlotSize).AlignMiddle().Element(item =>
                        {
                            item.AlignCenter().Width(hitAreaSize).Height(hitAreaSize).Element(dot =>
                            {
                                dot.SectionLink(SectionLinkNames[index])
                                    .AlignMiddle()
                                    .Element(iconContainer =>
                                    {
                                        iconContainer.Layers(hitLayers =>
                                        {
                                            // Keep a full-size transparent payload so every dot gets a full clickable PDF annotation area.
                                            hitLayers.PrimaryLayer().AlignCenter().AlignMiddle().Width(hitAreaSize).Height(hitAreaSize)
                                                .Svg(SvgIconFactory.BuildTransparentDotHitAreaSvg(hitAreaSize));

                                            hitLayers.Layer().AlignCenter().AlignMiddle().Width(dotSize).Height(dotSize)
                                                .Background(isActive ? "#1083D5" : "#E1E1E1")
                                                .Border(1)
                                                .BorderColor(isActive ? "#1083D5" : "#D2D2D2")
                                                .CornerRadius(dotSize / 2)
                                                .AlignMiddle()
                                                .Element(visualDot =>
                                                {
                                                    if (isActive)
                                                        visualDot.AlignCenter().AlignMiddle().Width(20).Height(20).Svg(SvgIconFactory.BuildTrackerStepIconSvg(index));
                                                });
                                        });
                                    });
                            });
                        });
                    }
                });
            });

            column.Item().Row(row =>
            {
                for (var index = 0; index < StepTitles.Length; index++)
                {
                    row.RelativeItem().AlignCenter().Text(StepTitles[index]).FontSize(8).FontColor(index == activeIndex ? Colors.Black : "#9C9C9C").AlignCenter();
                }
            });
        });
    }

    private void ComposeHeadingBlock(IContainer container, string title, string body)
    {
        container.Column(column =>
        {
            column.Spacing(4);
            column.Item().Text(title).FontSize(19);
            column.Item().Width(LayoutHelper.HeadingRuleWidth(title, 19)).LineHorizontal(1).LineColor("#CFCFCF");

            if (!string.IsNullOrWhiteSpace(body))
                column.Item().Text(body).FontSize(9.8f).LineHeight(1.3f);
        });
    }

    private void ComposeScoreCard(IContainer container)
    {
        container.AlignCenter().Width(240).Height(240).Layers(layers =>
        {
            layers.Layer().AlignCenter().AlignMiddle().Element(ComposeScoreHalo);

            layers.PrimaryLayer()
                .AlignCenter()
                .AlignMiddle()
                .Width(146)
                .Height(146)
                .Layers(scoreRingLayers =>
                {
                    scoreRingLayers.Layer()
                        .AlignCenter().AlignMiddle()
                        .Width(146).Height(146)
                        .Svg(SvgIconFactory.BuildScoreGradientRingSvg());

                    scoreRingLayers.PrimaryLayer()
                        .AlignCenter()
                        .AlignMiddle()
                        .Width(136)
                        .Height(136)
                        .CornerRadius(68)
                        .AlignMiddle()
                        .Column(column =>
                        {
                            column.Spacing(4);
                            column.Item().AlignCenter().Text(report.HealthSummary.ScoreValue.ToString()).FontSize(34).Bold();
                            column.Item().AlignCenter().Text($"out of {report.HealthSummary.ScoreMax}").FontSize(12.5f).FontColor("#666666");
                        });
                });
        });
    }

    private void ComposeScoreHalo(IContainer container)
    {
        if (_scoreHaloSvgPath is not null)
            container.AlignCenter().AlignMiddle().Width(240).Height(240).Svg(_scoreHaloSvgPath).FitArea();
    }

    private void ComposeHealthMetricsTable(IContainer container, List<HealthMetricRow> rows)
    {
        container.Column(tableColumn =>
        {
            tableColumn.Spacing(8);

            tableColumn.Item().Background("#DDDDDD").CornerRadius(12).PaddingVertical(8).PaddingHorizontal(14).Row(header =>
            {
                header.RelativeItem(3).Text("Parameter");
                header.RelativeItem(2).AlignCenter().Text("Previous");
                header.RelativeItem(2).AlignCenter().Text("Current");
                header.RelativeItem(1).AlignCenter().Text("Trend");
            });

            foreach (var row in rows)
            {
                var palette = GetPalette(row.Status);
                var trendSymbol = StatusUiHelper.TrendSymbol(row.Trend);

                tableColumn.Item().Background(palette.Background).CornerRadius(12).PaddingTop(9).PaddingBottom(0).PaddingHorizontal(14).Row(data =>
                {
                    data.RelativeItem(3).Text($"{ParameterIcon(row.Name)} {row.Name}").FontColor(Colors.Black);
                    data.RelativeItem(2).AlignCenter().Text(row.Previous);
                    data.RelativeItem(2).AlignCenter().Text(row.Current);
                    data.RelativeItem(1).AlignCenter().AlignMiddle().PaddingBottom(4).Text(trendSymbol).FontColor(palette.Strong).FontSize(StatusUiHelper.HealthTrendIconFontSize(row.Trend)).SemiBold();
                });
            }
        });
    }

    private void ComposeTestCard(IContainer container, TestSummaryCard card)
    {
        var palette = GetPalette(card.Status);

        container.Background(palette.Background)
            .CornerRadius(24)
            .PaddingTop(18)
            .PaddingLeft(18)
            .PaddingRight(18)
            .PaddingBottom(18)
            .Column(column =>
            {
                column.Spacing(8);
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"{ParameterIcon(card.Name)} {card.Name}").FontSize(14).SemiBold().FontColor(Colors.Black);
                    row.ConstantItem(100).Background(palette.Pill).CornerRadius(16).PaddingVertical(5).PaddingHorizontal(8).AlignCenter().AlignMiddle().Text(text =>
                    {
                        text.Span(StatusUiHelper.StatusIcon(card.Status)).FontSize(StatusUiHelper.StatusIconFontSize(card.Status, 11f)).FontColor(Colors.White);
                        text.Span($" {StatusUiHelper.DisplayStatus(card.Status)}").FontSize(11).FontColor(Colors.White).SemiBold();
                    });
                });
                column.Item().Text($"Value: {card.Value}").FontSize(12);
                column.Item().Text($"Range: {card.Range}").FontSize(12);
            });
    }

    private void ComposeActionCard(IContainer container, ActionPlanCard card)
    {
        container.Background(GetPalette(card.Status).Background)
            .CornerRadius(20)
            .Padding(20)
            .Column(column =>
            {
                column.Spacing(8);
                column.Item().Width(20).Height(20).Element(icon =>
                {
                    if (!string.IsNullOrWhiteSpace(_actionTestTubeIconSvgPath))
                        icon.Svg(_actionTestTubeIconSvgPath).FitArea();
                    else
                        icon.AlignCenter().AlignMiddle().Text(IconVials).FontSize(17).FontColor(Colors.Black);
                });
                column.Item().Text(card.Name).FontSize(17).FontColor(Colors.Black);
                column.Item().Text(card.Summary).FontSize(11).LineHeight(1.45f);
                column.Item().PaddingTop(1).Width(20).Height(20).Element(icon =>
                {
                    if (!string.IsNullOrWhiteSpace(_actionSuggestionIconSvgPath))
                        icon.Svg(_actionSuggestionIconSvgPath).FitArea();
                    else
                        icon.Svg(SvgIconFactory.BuildBulbOnIconSvg());
                });
                column.Item().Text("Suggestions").FontSize(15).FontColor(Colors.Black);
                column.Item().Text(card.Suggestions).FontSize(11).LineHeight(1.45f);
            });
    }

    private void ComposeClinicalGroup(IContainer container, ClinicalDataGroup group)
    {
        var groupPalette = GetPalette(group.Status);
        var isBorderlineStatus = group.Status.Trim().Equals("borderline", StringComparison.OrdinalIgnoreCase)
            || group.Status.Trim().Equals("neutral", StringComparison.OrdinalIgnoreCase);
        var badgeBackground = isBorderlineStatus ? groupPalette.Background : groupPalette.Pill;
        var badgeForeground = isBorderlineStatus ? groupPalette.Strong : "#FFFFFF";

        container.Column(column =>
        {
            column.Spacing(8);
            column.Item().Row(row =>
            {
                row.RelativeItem().Row(title =>
                {
                    title.ConstantItem(34).Width(34).Height(34).Background("#78F2A0").CornerRadius(17).AlignMiddle().Text(ClinicalGroupIcon(group.Name)).FontSize(16).FontColor(Colors.Black).AlignCenter();
                    title.RelativeItem().PaddingLeft(8).Text(group.Name).FontSize(18).Bold();
                });
                row.AutoItem().Background(badgeBackground).CornerRadius(16).PaddingTop(6).PaddingBottom(5).PaddingHorizontal(14).AlignCenter().AlignMiddle().Text(text =>
                {
                    text.Span(StatusUiHelper.StatusIcon(group.Status)).FontSize(StatusUiHelper.StatusIconFontSize(group.Status, 11f)).FontColor(badgeForeground);
                    text.Span($" {StatusUiHelper.DisplayStatus(group.Status)}").FontSize(11).FontColor(Colors.Black).SemiBold();
                });
            });

            column.Item().PaddingTop(4).Column(rowsColumn =>
            {
                rowsColumn.Spacing(6);

                rowsColumn.Item().Background("#DDDDDD").CornerRadius(12).PaddingVertical(10).PaddingHorizontal(12).Row(header =>
                {
                    header.RelativeItem(3).Text("Test Name");
                    header.RelativeItem(1).AlignCenter().Text("Result");
                    header.RelativeItem(1).AlignCenter().Text("Unit");
                    header.RelativeItem(3).Text("Range");
                    header.RelativeItem(1).AlignCenter().Text("Level");
                });

                foreach (var row in group.Rows)
                {
                    var palette = GetPalette(row.Level);

                    rowsColumn.Item().Background(palette.Background).CornerRadius(12).PaddingVertical(12).PaddingHorizontal(12).Row(data =>
                    {
                        data.RelativeItem(3).Text(row.TestName);
                        data.RelativeItem(1).AlignCenter().Text(row.Result);
                        data.RelativeItem(1).AlignCenter().Text(row.Unit);
                        data.RelativeItem(3).Text(row.Range).FontSize(9.5f);
                        data.RelativeItem(1).AlignCenter().Text(StatusUiHelper.TrendSymbol(row.Level)).FontColor(palette.Strong).FontSize(StatusUiHelper.ClinicalLevelIconFontSize(row.Level)).SemiBold();
                    });
                }
            });
        });
    }

    private void ComposeGuidanceChart(IContainer container, GuidanceChart chart)
    {
        var palette = GetPalette(chart.Status);
        var normalizedStatus = chart.Status.Trim().ToLowerInvariant();
        var useLightBadge = normalizedStatus is "normal" or "up" or "borderline" or "neutral";
        var badgeBackground = normalizedStatus is "abnormal" or "down"
            ? palette.Background
            : (useLightBadge ? palette.Background : palette.Pill);
        var badgeForeground = normalizedStatus is "abnormal" or "down"
            ? palette.Pill
            : (useLightBadge ? palette.Strong : "#FFFFFF");

        container.ShowEntire().Column(column =>
        {
            column.Spacing(8);
            column.Item().Row(row =>
            {
                row.RelativeItem().Text(chart.Title).FontSize(13).SemiBold();
                row.AutoItem().Background(badgeBackground).CornerRadius(16).PaddingTop(5).PaddingBottom(4).PaddingHorizontal(14).AlignCenter().AlignMiddle().Text(text =>
                {
                    text.Span(StatusUiHelper.StatusIcon(chart.Status)).FontSize(StatusUiHelper.StatusIconFontSize(chart.Status, 12f)).FontColor(badgeForeground);
                    text.Span($" {StatusUiHelper.DisplayStatus(chart.Status)}").FontSize(12).FontColor(Colors.Black).SemiBold();
                });
            });

            column.Item().Background("#FFFFFF").CornerRadius(10).Padding(4).MinHeight(128).Element(c =>
            {
                if (chart.DataPoints.Count > 0)
                    ComposeLineChart(c, chart);
                else
                    c.AlignCenter().AlignMiddle().Text(SparklineByStatus(chart.Status)).FontSize(30).FontColor(Colors.Black);
            });

            if (!string.IsNullOrWhiteSpace(chart.Description))
                column.Item().Text(chart.Description).FontSize(9).LineHeight(1.28f);
        });
    }

    private void ComposeLineChart(IContainer container, GuidanceChart chart)
    {
        var palette = GetPalette(chart.Status);
        var chartSvg = ChartSvgBuilder.GenerateLineChartSvg(chart, palette.Background, palette.Strong);

        if (!string.IsNullOrWhiteSpace(chartSvg))
            container.Svg(chartSvg);
    }

    private void ComposeLogo(IContainer container)
    {
        if (!string.IsNullOrWhiteSpace(_logoSvgPath))
            container.Svg(_logoSvgPath).FitArea();
        else if (_logoBytes is null)
            container.Text(report.Branding.OrganisationName).FontSize(18).FontColor("#1083D5").SemiBold();
        else
            container.Image(_logoBytes);
    }

    private void ComposeLabIcon(IContainer container)
    {
        container.AlignCenter().AlignMiddle().Text(IconFlask).FontSize(14).FontColor(Colors.Black);
    }

    private void ComposeBulbIcon(IContainer container)
    {
        container.AlignCenter().AlignMiddle().Text(IconLightbulb).FontSize(14).FontColor(Colors.Black);
    }

    private void ComposeDropletIcon(IContainer container)
    {
        container.AlignCenter().AlignMiddle().Text(IconDroplet).FontSize(12).FontColor(Colors.Black);
    }

    private void ComposeCoverHeart(IContainer container)
    {
        if (_coverHeartBytes is not null)
            container.Image(_coverHeartBytes);
    }

    private void ComposeFooterRibbon(IContainer container)
    {
        if (_footerRibbonBytes is not null)
            container.PaddingTop(2).AlignRight().Image(_footerRibbonBytes).FitHeight();
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container.Background("#E5E5E5").PaddingVertical(8).PaddingHorizontal(8).DefaultTextStyle(x => x.SemiBold());
    }

    private static IContainer BodyCell(IContainer container, string background)
    {
        return container.Background(background).PaddingVertical(10).PaddingHorizontal(8);
    }

    private static string ParameterIcon(string value)
    {
        var text = value.Trim().ToLowerInvariant();

        if (text.Contains("glucose") || text.Contains("sugar") || text.Contains("cholesterol") || text.Contains("creatinine"))
            return IconDroplet;

        if (text.Contains("liver") || text.Contains("kidney") || text.Contains("iron") || text.Contains("pancreas"))
            return IconDroplet;
        
        if (text.Contains("electrolytes"))
            return IconBolt;

        return IconDroplet;
    }

    private string? GuidanceTipSvgPath(string title, string description)
    {
        var normalizedTitle = title.Trim().ToLowerInvariant();

        if (normalizedTitle.Contains("exercise") || normalizedTitle.Contains("walk") || normalizedTitle.Contains("activity"))
            return _tipExerciseIconSvgPath;

        if (normalizedTitle.Contains("lose") || normalizedTitle.Contains("weight") || normalizedTitle.Contains("obesity"))
            return _tipLoseWeightIconSvgPath;

        if (normalizedTitle.Contains("sugar") || normalizedTitle.Contains("sweet") || normalizedTitle.Contains("soft drink") || normalizedTitle.Contains("soda"))
            return _tipAvoidSweetsIconSvgPath;

        if (normalizedTitle.Contains("carb") || normalizedTitle.Contains("rice") || normalizedTitle.Contains("wheat"))
            return _tipCarbohydratesIconSvgPath;

        if (normalizedTitle.Contains("protein") || normalizedTitle.Contains("paneer") || normalizedTitle.Contains("milk") || normalizedTitle.Contains("curd"))
            return _tipProteinIconSvgPath;

        if (normalizedTitle.Contains("vegetable") || normalizedTitle.Contains("fiber") || normalizedTitle.Contains("salad") || normalizedTitle.Contains("beans"))
            return _tipVegetablesFiberIconSvgPath;

        var normalizedDescription = description.Trim().ToLowerInvariant();

        if (normalizedDescription.Contains("exercise") || normalizedDescription.Contains("walk") || normalizedDescription.Contains("activity"))
            return _tipExerciseIconSvgPath;

        if (normalizedDescription.Contains("lose") || normalizedDescription.Contains("weight") || normalizedDescription.Contains("obesity"))
            return _tipLoseWeightIconSvgPath;

        if (normalizedDescription.Contains("sugar") || normalizedDescription.Contains("sweet") || normalizedDescription.Contains("soft drink") || normalizedDescription.Contains("soda"))
            return _tipAvoidSweetsIconSvgPath;

        if (normalizedDescription.Contains("carb") || normalizedDescription.Contains("rice") || normalizedDescription.Contains("wheat"))
            return _tipCarbohydratesIconSvgPath;

        if (normalizedDescription.Contains("protein") || normalizedDescription.Contains("paneer") || normalizedDescription.Contains("milk") || normalizedDescription.Contains("curd"))
            return _tipProteinIconSvgPath;

        if (normalizedDescription.Contains("vegetable") || normalizedDescription.Contains("fiber") || normalizedDescription.Contains("salad") || normalizedDescription.Contains("beans"))
            return _tipVegetablesFiberIconSvgPath;

        return null;
    }

    private static string CondenseTipDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return string.Empty;

        var compact = description.Replace("\r", " ").Replace("\n", " ").Trim();
        var sentenceEnd = compact.IndexOf('.');

        if (sentenceEnd > 0)
            compact = compact[..sentenceEnd].Trim();

        if (compact.Length <= 62)
            return compact;

        var cutIndex = compact.LastIndexOf(' ', 62);
        if (cutIndex < 32)
            cutIndex = 62;

        return $"{compact[..cutIndex].TrimEnd(',', ';', ' ')}...";
    }

    private static string SparklineByStatus(string status)
    {
        return status.Trim().ToLowerInvariant() switch
        {
            "normal" or "up" => "â–â–‚â–ƒâ–„â–…â–†â–…â–„",
            "borderline" or "neutral" => "â–‚â–ƒâ–„â–„â–ƒâ–„â–ƒâ–‚",
            _ => "â–†â–…â–…â–„â–ƒâ–‚â–‚â–"
        };
    }

    private static string ClinicalGroupIcon(string groupName)
    {
        var name = groupName.Trim().ToLowerInvariant();

        if (name.Contains("glucose") || name.Contains("cholesterol"))
            return IconDroplet;

        return IconCircle;
    }

    private static bool TryRegisterDocumentFonts()
    {
        var allLoaded = true;

        allLoaded &= TryRegisterFont("assets/fonts/Exo2-Variable.ttf");
        allLoaded &= TryRegisterFont("assets/fonts/Rajdhani-Light.ttf");
        allLoaded &= TryRegisterFont("assets/fonts/BarlowCondensed-Thin.ttf");
        allLoaded &= TryRegisterFont("assets/fonts/BarlowCondensed-Light.ttf");
        allLoaded &= TryRegisterFont("assets/fonts/Lato-Thin.ttf");
        allLoaded &= TryRegisterFont("assets/fonts/BebasNeue-Regular.ttf");
        allLoaded &= TryRegisterFont("assets/fonts/PatrickHand-Regular.ttf");
        allLoaded &= TryRegisterFont("assets/fonts/fa-solid-900.ttf");

        return allLoaded;
    }

    private static bool TryRegisterFont(string relativePath)
    {
        foreach (var root in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() }.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var current = new DirectoryInfo(root);

            while (current is not null)
            {
                var candidate = Path.Combine(current.FullName, relativePath.Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(candidate))
                {
                    using var fontStream = File.OpenRead(candidate);
                    FontManager.RegisterFont(fontStream);
                    return true;
                }

                current = current.Parent;
            }
        }

        return false;
    }

    private static StatusPalette GetPalette(string status)
    {
        return ReportTheme.GetPalette(status);
    }

    private static byte[]? TryLoadAsset(string path)
    {
        var resolvedPath = TryResolveAssetPath(path);
        return resolvedPath is null ? null : File.ReadAllBytes(resolvedPath);
    }

    private static string? TryResolveAssetPath(string path)
    {
        foreach (var root in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() }.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var current = new DirectoryInfo(root);

            while (current is not null)
            {
                var candidate = Path.IsPathRooted(path)
                    ? path
                    : Path.Combine(current.FullName, path.Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(candidate))
                    return candidate;

                current = current.Parent;
            }
        }

        return null;
    }

    private static bool IsSvgAsset(string path)
    {
        return !string.IsNullOrWhiteSpace(path) && path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);
    }

    private static byte[]? GenerateQrCode(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        var code = new PngByteQRCode(data);
        return code.GetGraphic(12);
    }

    private static List<List<T>> Chunk<T>(IReadOnlyCollection<T> source, int size)
    {
        if (source.Count == 0)
            return [new List<T>()];

        return source.Chunk(size).Select(x => x.ToList()).ToList();
    }

}
 











