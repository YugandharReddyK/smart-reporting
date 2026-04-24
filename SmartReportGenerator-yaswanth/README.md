# Smart Health Report Generator

A .NET 8 console application that generates professionally styled health report PDFs from JSON data. It uses **Scriban** for HTML templating and **PuppeteerSharp** (headless Chromium) for PDF rendering, with **Chart.js** charts and **QR codes** embedded inline.

## Features

- 8-page health report with cover, how-to guide, health summary, test summary, action plan, clinical data, guidance, and cheat sheet
- Interactive Chart.js line charts rendered as vector graphics
- QR code generation embedded in the report
- SVG icons and stepper navigation
- All assets (JS libraries, SVGs, fonts) bundled inline — no external CDN dependencies
- Detailed timing breakdown for performance analysis

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Google Chrome installed (used as fallback if PuppeteerSharp's Chromium download is incomplete)

## Project Structure

```
SmartReportGenerator/
├── Program.cs                  # Entry point
├── SmartReportGenerator.csproj # Project config
├── Models/
│   └── ReportData.cs           # JSON data model
├── Services/
│   ├── PdfGeneratorService.cs  # Chromium-based PDF generation
│   ├── TemplateService.cs      # Scriban HTML template rendering
│   └── SvgAssetService.cs      # SVG icon loader
├── Templates/
│   └── report-template.html    # 8-page Scriban HTML template
├── Data/
│   └── sample-report.json      # Sample input data
└── Assets/
    ├── chart.umd.min.js        # Chart.js 4.4.4 (bundled)
    ├── qrcode.min.js           # QRCode generator 1.4.4 (bundled)
    └── Svgs/                   # SVG icons and stepper assets
```

## How to Run

### Build

```bash
dotnet build
```

### Run with Default Paths

Uses `Data/sample-report.json` as input and outputs to your **Downloads** folder (`~/Downloads/SmartHealthReport.pdf`):

```bash
dotnet run
```

If `SmartHealthReport.pdf` already exists in Downloads, subsequent runs create `SmartHealthReport_1.pdf`, `SmartHealthReport_2.pdf`, etc.

Or run the compiled DLL directly (faster, skips build step):

```bash
dotnet bin/Debug/net8.0/SmartReportGenerator.dll
```

### Run with Custom Paths

```bash
dotnet run -- "<path-to-json>" "<path-to-output-pdf>"
```

Or with the DLL:

```bash
dotnet bin/Debug/net8.0/SmartReportGenerator.dll "C:\path\to\input.json" "C:\path\to\output.pdf"
```

**Arguments:**

| Arg | Description | Default |
|-----|-------------|---------|
| 1   | JSON input file path | `bin/.../Data/sample-report.json` |
| 2   | PDF output file path | `~/Downloads/SmartHealthReport.pdf` |

## Performance Benchmarks

Tested on a standard development machine (Windows). Times measured using DLL execution (`dotnet SmartReportGenerator.dll`).

### 8 Pages (Single Report)

| Metric | Value |
|--------|-------|
| **PDF Size** | **0.90 MB** |
| **Total Time** | **~3.6s** |
| Template Render | ~182 ms |
| New Page | ~127 ms |
| SetContent + DOM | ~332 ms |
| Wait Chart.js | ~55 ms |
| Chart Render | ~5 ms |
| PDF Render | ~1,826 ms |

### 152 Pages (19x Duplicated Report)

| Metric | Value |
|--------|-------|
| **PDF Size** | **15.55 MB** |
| **Total Time** | **~24s** |
| Template Render | ~182 ms |
| HTML Duplication | ~69 ms |
| New Page | ~127 ms |
| SetContent + DOM | ~1,314 ms |
| Wait Chart.js | ~161 ms |
| Chart Render | ~21 ms |
| PDF Render | ~21,046 ms |

### Bottleneck Analysis

| Step | 8 Pages | 152 Pages | Notes |
|------|---------|-----------|-------|
| Template Render | 5% | 0.8% | Scriban is fast |
| SetContent + DOM | 9% | 5.5% | Chromium parses HTML |
| **PDF Render** | **51%** | **88%** | **Main bottleneck** — Chromium's internal layout/rasterization |
| Browser overhead | 35% | 5.3% | One-time cost, amortized at scale |

> **PDF Render is the dominant cost** at scale. This is Chromium's internal `page.pdf()` operation — paginating and rasterizing A4 pages. It cannot be optimized from the application side.

## NuGet Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| [PuppeteerSharp](https://www.puppeteersharp.com/) | 24.40.0 | Headless Chromium for PDF generation |
| [Scriban](https://github.com/scriban/scriban) | 7.1.0 | HTML template engine |

## Output Example

```
=== Smart Health Report Generator ===
JSON Input:  ...\Data\sample-report.json
PDF Output:  ...\Downloads\SmartHealthReport.pdf

Done! Report generated successfully.
PDF Size: 0.90 MB

  Template Render:       182.1 ms
  New Page:              126.9 ms
  SetContent+DOM:        332.2 ms
  Wait Chart.js:          55.3 ms
  Chart Render:            5.2 ms
  PDF Render:           1826.4 ms
  Total:                3601.0 ms (3.60s)
```
