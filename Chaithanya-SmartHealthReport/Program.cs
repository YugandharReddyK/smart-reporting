using System.Text.Json;
using SmartHealthReport.Data;
using SmartHealthReport.Models;
using SmartHealthReport.Services;

Console.WriteLine("Smart Health Report Generator (Playwright)");
Console.WriteLine("===========================================");
var sw = System.Diagnostics.Stopwatch.StartNew();
// Load data from hardcoded JSON
var json = SampleData.GetJson();
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var reportData = JsonSerializer.Deserialize<ReportData>(json, options)!;

Console.WriteLine($"Patient: {reportData.Patient.Name}");
Console.WriteLine($"Report ID: {reportData.ReportId}");
Console.WriteLine($"Health Score: {reportData.HealthScore}/100");

// Generate PDF
Console.WriteLine();
Console.WriteLine("Launching headless Chromium...");

await using var pdfService = new PdfGeneratorService();
var pdfBytes = await pdfService.GenerateAsync(reportData);


var downloadsPath = Environment.GetEnvironmentVariable("REPORT_OUTPUT_DIR")
    ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
var fileName = $"SmartHealthReport_{reportData.ReportId}.pdf";
var filePath = Path.Combine(downloadsPath, fileName);

await File.WriteAllBytesAsync(filePath, pdfBytes);
sw.Stop();
Console.WriteLine();
Console.WriteLine($"PDF generated successfully!");
Console.WriteLine($"Location: {filePath}");
Console.WriteLine($"Size: {pdfBytes.Length / 1024.0:F1} KB");
Console.WriteLine($"PDF Generation Time: {sw.Elapsed.TotalSeconds:F2} seconds");
