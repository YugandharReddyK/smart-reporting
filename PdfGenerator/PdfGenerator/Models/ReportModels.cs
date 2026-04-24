namespace PdfGenerator.Models;

public sealed class ReportEnvelope
{
    public SmartReport Report { get; set; } = new();
}

public sealed class SmartReport
{
    public BrandingInfo Branding { get; set; } = new();
    public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
    public PatientInfo Patient { get; set; } = new();
    public CoverPage Cover { get; set; } = new();
    public IntroductionPage Introduction { get; set; } = new();
    public HealthSummaryPage HealthSummary { get; set; } = new();
    public TestSummaryPage TestSummary { get; set; } = new();
    public ActionPlanPage ActionPlan { get; set; } = new();
    public ClinicalDataPage ClinicalData { get; set; } = new();
    public GuidancePage PersonalisedGuidance { get; set; } = new();
    public CheatSheetPage CheatSheet { get; set; } = new();
}

public sealed class BrandingInfo
{
    public string OrganisationName { get; set; } = "Agilus Diagnostics";
    public string LogoPath { get; set; } = "assets/logo.svg";
    public string CoverHeartPath { get; set; } = "assets/cover-heart.png";
    public string FooterRibbonPath { get; set; } = "assets/footer-ribbon.png";
}

public sealed class PatientInfo
{
    public string ReportId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}

public sealed class CoverPage
{
    public string GreetingName { get; set; } = string.Empty;
    public string PrimaryTitle { get; set; } = "SMART";
    public string SecondaryTitle { get; set; } = "HEALTH REPORT";
}

public sealed class IntroductionPage
{
    public string AnalyticsTitle { get; set; } = "Agilus Health Analytics Report";
    public string AnalyticsText { get; set; } = string.Empty;
    public string ReadTitle { get; set; } = "How to Read the Report";
    public string ReadText { get; set; } = string.Empty;
    public List<SectionOverviewItem> OverviewItems { get; set; } = [];
}

public sealed class SectionOverviewItem
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class HealthSummaryPage
{
    public string ScoreTitle { get; set; } = "Health Score";
    public string ScoreDescription { get; set; } = string.Empty;
    public int ScoreValue { get; set; }
    public int ScoreMax { get; set; } = 100;
    public string SummaryTitle { get; set; } = "Your Health Summary";
    public string SummaryIntro { get; set; } = string.Empty;
    public List<HealthMetricRow> Rows { get; set; } = [];
}

public sealed class HealthMetricRow
{
    public string Name { get; set; } = string.Empty;
    public string Previous { get; set; } = string.Empty;
    public string Current { get; set; } = string.Empty;
    public string Trend { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public sealed class TestSummaryPage
{
    public string Title { get; set; } = "Your Important Parameters at a Glance";
    public List<TestSummaryCard> Cards { get; set; } = [];
}

public sealed class TestSummaryCard
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public sealed class ActionPlanPage
{
    public string Title { get; set; } = "Your Parameters That Need Attention";
    public List<ActionPlanCard> Cards { get; set; } = [];
}

public sealed class ActionPlanCard
{
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Suggestions { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public sealed class ClinicalDataPage
{
    public string Title { get; set; } = "Clinical Data";
    public List<ClinicalDataGroup> Groups { get; set; } = [];
}

public sealed class ClinicalDataGroup
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<ClinicalDataRow> Rows { get; set; } = [];
}

public sealed class ClinicalDataRow
{
    public string TestName { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}

public sealed class GuidancePage
{
    public string Title { get; set; } = "Personalised Guidance";
    public string TipsTitle { get; set; } = "Diet & Lifestyle Tips";
    public List<GuidanceChart> Charts { get; set; } = [];
    public List<GuidanceTip> Tips { get; set; } = [];
}

public sealed class GuidanceChart
{
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ChartDataPoint> DataPoints { get; set; } = [];
    public string? YAxisLabel { get; set; } = string.Empty;
    public string? CurrentValueLabel { get; set; } = string.Empty;
}

public sealed class ChartDataPoint
{
    public string Date { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public bool IsCurrentPoint { get; set; } = false;
}

public sealed class GuidanceTip
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class CheatSheetPage
{
    public string Title { get; set; } = "Cheat-Sheet";
    public string PromptTitle { get; set; } = "Things you can ask the doctor";
    public List<string> Questions { get; set; } = [];
    public string QrTitle { get; set; } = "Scan to View Your Report Online";
    public string QrUrl { get; set; } = string.Empty;
}
