namespace SmartReportGenerator.Models;

/// <summary>
/// Root model for the entire Smart Health Report.
/// </summary>
public class ReportData
{
    public CoverInfo Cover { get; set; } = new();
    public ReportMeta Meta { get; set; } = new();
    public HealthSummary HealthSummary { get; set; } = new();
    public List<TestCard> TestSummaryCards { get; set; } = [];
    public List<ActionItem> ActionPlan { get; set; } = [];
    public List<ClinicalCategory> ClinicalData { get; set; } = [];
    public PersonalisedGuidance PersonalisedGuidance { get; set; } = new();
    public CheatSheet CheatSheet { get; set; } = new();
}

public class CoverInfo
{
    public string PatientName { get; set; } = string.Empty;
    public string ReportId { get; set; } = string.Empty;
    public string Title { get; set; } = "SMART";
    public string TitleSub { get; set; } = "HEALTH REPORT";
    public string LogoUrl { get; set; } = string.Empty;
}

public class ReportMeta
{
    public string OrganizationName { get; set; } = string.Empty;
    public string Disclaimer { get; set; } = string.Empty;
    public List<SectionInfo> Sections { get; set; } = [];
}

public class SectionInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconColor { get; set; } = "#333";
}

public class HealthSummary
{
    public int Score { get; set; }
    public int MaxScore { get; set; } = 100;
    public string Message { get; set; } = string.Empty;
    public string SubMessage { get; set; } = string.Empty;
    public List<ParameterRow> Parameters { get; set; } = [];
}

public class ParameterRow
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "💧";
    public string Previous { get; set; } = string.Empty;
    public string Current { get; set; } = string.Empty;
    /// <summary>up, down, same</summary>
    public string Trend { get; set; } = "same";
    /// <summary>normal, borderline, abnormal</summary>
    public string Status { get; set; } = "normal";
}

public class TestCard
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    /// <summary>normal, borderline, abnormal</summary>
    public string Status { get; set; } = "normal";
    /// <summary>up, down, same</summary>
    public string Trend { get; set; } = "same";
    public string Icon { get; set; } = "💧";
}

public class ActionItem
{
    public string ParameterName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Suggestion { get; set; } = string.Empty;
    /// <summary>warning, critical</summary>
    public string Severity { get; set; } = "warning";
}

public class ClinicalCategory
{
    public string CategoryName { get; set; } = string.Empty;
    public string Icon { get; set; } = "💧";
    /// <summary>normal, borderline, abnormal</summary>
    public string OverallStatus { get; set; } = "normal";
    public List<ClinicalTest> Tests { get; set; } = [];
}

public class ClinicalTest
{
    public string TestName { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    /// <summary>normal, borderline, abnormal</summary>
    public string Status { get; set; } = "normal";
    /// <summary>up, down, same</summary>
    public string Trend { get; set; } = "same";
}

public class PersonalisedGuidance
{
    public List<GuidanceChart> Charts { get; set; } = [];
    public List<LifestyleTip> DietTips { get; set; } = [];
}

public class GuidanceChart
{
    public string TestName { get; set; } = string.Empty;
    /// <summary>normal, borderline, abnormal</summary>
    public string Status { get; set; } = "normal";
    public List<ChartDataPoint> AverageData { get; set; } = [];
    public ChartDataPoint? PatientPoint { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ChartDataPoint
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
}

public class LifestyleTip
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "🏃";
}

public class CheatSheet
{
    public List<string> Questions { get; set; } = [];
    public string QrCodeUrl { get; set; } = string.Empty;
    public string ReportUrl { get; set; } = string.Empty;
}
