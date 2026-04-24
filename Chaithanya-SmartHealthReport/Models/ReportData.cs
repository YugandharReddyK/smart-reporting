namespace SmartHealthReport.Models;

public class ReportData
{
    public PatientInfo Patient { get; set; } = new();
    public string ReportId { get; set; } = string.Empty;
    public string ReportUrl { get; set; } = string.Empty;
    public int HealthScore { get; set; }
    public List<HealthParameter> HealthSummaryParameters { get; set; } = new();
    public List<TestCard> TestSummaryCards { get; set; } = new();
    public List<ActionItem> ActionPlan { get; set; } = new();
    public List<ClinicalSection> ClinicalData { get; set; } = new();
    public List<GuidanceItem> PersonalisedGuidance { get; set; } = new();
    public List<string> DoctorQuestions { get; set; } = new();
}

public class PatientInfo
{
    public string Name { get; set; } = string.Empty;
}

public class HealthParameter
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public double Previous { get; set; }
    public double Current { get; set; }
    public string Trend { get; set; } = string.Empty;
}

public class TestCard
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public string Trend { get; set; } = string.Empty;
}

public class ActionItem
{
    public string ParameterName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Suggestion { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}

public class ClinicalSection
{
    public string CategoryName { get; set; } = string.Empty;
    public string OverallStatus { get; set; } = string.Empty;
    public List<ClinicalTest> Tests { get; set; } = new();
}

public class ClinicalTest
{
    public string TestName { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Range { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}

public class GuidanceItem
{
    public string TestName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> DietTips { get; set; } = new();
    public List<string> LifestyleTips { get; set; } = new();
}
