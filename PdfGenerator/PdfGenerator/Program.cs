using PdfGenerator.Models;
using PdfGenerator.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Text.Json;

QuestPDF.Settings.License = LicenseType.Community; QuestPDF.Settings.EnableDebugging = true;


var defaultInputPath = ResolveDefaultInputPath();

var inputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : defaultInputPath;

var outputPath = args.Length > 1
    ? Path.GetFullPath(args[1])
    : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"health-report-{DateTime.Now:yyyyMMdd-HHmmss}.pdf");

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Input JSON file not found: {inputPath}");
    Console.Error.WriteLine("Usage: JsonPdfConsoleApp <input-json-path> <output-pdf-path>");
    return 1;
}

var json = await File.ReadAllTextAsync(inputPath);

var reportEnvelope = JsonSerializer.Deserialize<ReportEnvelope>(json, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

if (reportEnvelope?.Report is null)
{
    Console.Error.WriteLine("The JSON file does not contain a valid report object.");
    return 1;
}

ValidateReport(reportEnvelope.Report);

var outputDirectory = Path.GetDirectoryName(outputPath);
if (!string.IsNullOrWhiteSpace(outputDirectory))
    Directory.CreateDirectory(outputDirectory);

var document = new PatientReportDocument(reportEnvelope.Report);
document.GeneratePdf(outputPath);

Console.WriteLine($"Input JSON : {inputPath}");
Console.WriteLine($"Output PDF : {outputPath}");
return 0;

static void ValidateReport(SmartReport report)
{
    if (string.IsNullOrWhiteSpace(report.Patient.Name))
        throw new InvalidOperationException("Patient name is required.");

    if (string.IsNullOrWhiteSpace(report.Patient.ReportId))
        throw new InvalidOperationException("Patient report id is required.");

    if (report.HealthSummary.Rows.Count == 0)
        throw new InvalidOperationException("At least one health summary row is required.");
}

static string ResolveDefaultInputPath()
{
    var searchRoots = new[]
    {
        AppContext.BaseDirectory,
        Directory.GetCurrentDirectory()
    };

    foreach (var root in searchRoots.Distinct(StringComparer.OrdinalIgnoreCase))
    {
        var current = new DirectoryInfo(root);

        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "sample-report.json");

            if (File.Exists(candidate))
                return candidate;

            current = current.Parent;
        }
    }

    return Path.Combine(AppContext.BaseDirectory, "sample-report.json");
}
