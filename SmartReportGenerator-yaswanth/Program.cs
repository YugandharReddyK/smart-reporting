using System.Diagnostics;
using System.Text.Json;
using SmartReportGenerator.Models;
using SmartReportGenerator.Services;

var basePath = AppDomain.CurrentDomain.BaseDirectory;
var jsonPath = args.Length > 0 ? args[0] : Path.Combine(basePath, "Data", "sample-report.json");
var downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
var outputPath = args.Length > 1 ? args[1] : GetUniqueFilePath(Path.Combine(downloadsFolder, "SmartHealthReport.pdf"));
var templatePath = Path.Combine(basePath, "Templates", "report-template.html");

Console.WriteLine("=== Smart Health Report Generator ===");
Console.WriteLine($"JSON Input:  {jsonPath}");
Console.WriteLine($"PDF Output:  {outputPath}");
Console.WriteLine();

var totalSw = Stopwatch.StartNew();

// Load JSON
if (!File.Exists(jsonPath))
{
    Console.Error.WriteLine($"Error: JSON file not found at '{jsonPath}'");
    return 1;
}

var jsonContent = await File.ReadAllTextAsync(jsonPath);
var reportData = JsonSerializer.Deserialize<ReportData>(jsonContent, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

if (reportData is null)
{
    Console.Error.WriteLine("Error: Failed to deserialize report data.");
    return 1;
}

// Render HTML
if (!File.Exists(templatePath))
{
    Console.Error.WriteLine($"Error: Template not found at '{templatePath}'");
    return 1;
}

var templateSw = Stopwatch.StartNew();
var svgAssets = new SvgAssetService(Path.Combine(basePath, "Assets", "Svgs"));
var templateService = new TemplateService();
var html = templateService.RenderTemplate(templatePath, reportData, svgAssets);
templateSw.Stop();

// Generate PDF
var pdfService = new PdfGeneratorService();
var (pdfBytes, timing) = await pdfService.GeneratePdfAsync(html);

// Save
var directory = Path.GetDirectoryName(outputPath);
if (!string.IsNullOrEmpty(directory))
    Directory.CreateDirectory(directory);
await File.WriteAllBytesAsync(outputPath, pdfBytes);

totalSw.Stop();

// Print results
Console.WriteLine();
Console.WriteLine("Done! Report generated successfully.");
Console.WriteLine($"PDF Size: {pdfBytes.Length / 1024.0 / 1024.0:F2} MB");
Console.WriteLine();
Console.WriteLine($"  Template Render:    {templateSw.Elapsed.TotalMilliseconds,8:F1} ms");
Console.WriteLine($"  New Page:           {timing.NewPageMs,8:F1} ms");
Console.WriteLine($"  SetContent+DOM:     {timing.SetContentMs,8:F1} ms");
Console.WriteLine($"  Wait Chart.js:      {timing.WaitChartJsMs,8:F1} ms");
Console.WriteLine($"  Chart Render:       {timing.ChartRenderDelayMs,8:F1} ms");
Console.WriteLine($"  PDF Render:         {timing.PdfRenderMs,8:F1} ms");
Console.WriteLine($"  Total:              {totalSw.Elapsed.TotalMilliseconds,8:F1} ms ({totalSw.Elapsed.TotalSeconds:F2}s)");

return 0;

static string GetUniqueFilePath(string filePath)
{
    if (!File.Exists(filePath))
        return filePath;

    var directory = Path.GetDirectoryName(filePath)!;
    var name = Path.GetFileNameWithoutExtension(filePath);
    var extension = Path.GetExtension(filePath);
    var counter = 1;

    string newPath;
    do
    {
        newPath = Path.Combine(directory, $"{name}_{counter}{extension}");
        counter++;
    } while (File.Exists(newPath));

    return newPath;
}