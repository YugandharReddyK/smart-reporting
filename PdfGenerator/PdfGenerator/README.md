# PDF Generator - Health Report

A .NET 10.0 console application that generates professional, multi-page health report PDFs from JSON data using the QuestPDF library.

## Quick Start

### Prerequisites
- .NET 10.0 SDK or later

### Basic Usage

#### Without specifying output path (default to Downloads):
```bash
dotnet run
```

PDFs are saved to: `%UserProfile%\Downloads\health-report-{timestamp}.pdf`

#### With custom output path:
```bash
dotnet run -- <input-json-path> <output-pdf-path>
```

**Examples:**
```bash
dotnet run -- sample-report.json ./output.pdf
dotnet run -- sample-150pages-report.json C:\Users\YourName\Documents\report.pdf
```

Or run the compiled DLL directly:
```bash
dotnet bin\Debug\net10.0\PdfGenerator.dll sample-report.json output.pdf
```

---

## Performance Benchmarks

### 8-Page Report (Minimal)
| Metric | Value |
|--------|-------|
| **Input** | `sample-report.json` (1 clinical group) |
| **Output Size** | 0.61 MB |
| **Generation Time** | 0.886 seconds (~0.11 sec/page) |

### 150-Page Report (Full)
| Metric | Value |
|--------|-------|
| **Input** | `sample-150pages-report.json` (130 clinical groups + expanded text) |
| **Output Size** | 3.81 MB |
| **Generation Time** | 3.395 seconds (~0.023 sec/page) |

### Performance Summary
- **Throughput:** ~44 pages/second
- **Size Factor:** ~0.025 MB per page (text-only content)
- **Memory Usage:** Minimal (~50 MB for 150-page generation)

---

## Output Locations

### Default Output
When no output path is specified, PDFs are saved to the **Downloads** folder:
```
C:\Users\{YourUsername}\Downloads\health-report-20260422-143015.pdf
```

### Custom Output
Specify any path as the second argument:
```bash
dotnet run -- sample-report.json ~/Documents/my-report.pdf
```

The `benchmark-output/` directory is used only for local benchmark/test outputs and can be safely cleaned.

---

## File Structure

```
PdfGenerator/
├── Program.cs                           # Entry point, input/output path handling
├── benchmark.ps1                        # Skia chart benchmark helper
├── gen_json.ps1                         # Script to regenerate large sample JSON
├── Models/
│   └── ReportModels.cs                  # JSON data model definitions
├── Pdf/
│   ├── PatientReportDocument.cs         # Main PDF composition orchestrator
│   ├── Sections/                        # Report section builders
│   ├── Graphics/                        # Skia chart rendering helpers
│   └── Shared/                          # Layout and theme helpers
├── assets/
│   ├── fonts/                           # TrueType font files for custom typography
│   ├── tips/                            # Icon assets for guidance tips
│   ├── logo.svg                         # Branding logo
│   ├── logo-footer.svg                  # Footer-optimized logo variant
│   ├── cover-heart.png                  # Cover page decoration
│   ├── footer-ribbon.png                # Footer graphic
│   └── score-halo.svg                   # Health score visualization
├── benchmark-output/                    # Local generated benchmark/test PDFs
├── sample-report.json                   # 8-page minimal sample (1 clinical group)
├── sample-150pages-report.json          # 150-page sample data (130 clinical groups)
├── PdfGenerator.csproj                  # .NET project configuration
└── README.md                            # This file
```

---

## JSON Input Format

The application expects JSON in the following structure:

```json
{
  "report": {
    "branding": {
      "organisationName": "Agilus Diagnostics",
      "logoPath": "assets/logo.svg",
      "coverHeartPath": "assets/cover-heart.png",
      "footerRibbonPath": "assets/footer-ribbon.png"
    },
    "generatedOn": "2026-04-22T00:00:00Z",
    "patient": {
      "reportId": "250000514300377",
      "name": "Patient Name",
      "age": 39,
      "gender": "Male",
      "dateOfBirth": "1986-08-15",
      "provider": "Hospital Name"
    },
    "cover": { ... },
    "introduction": { ... },
    "healthSummary": { ... },
    "testSummary": { ... },
    "actionPlan": { ... },
    "clinicalData": {
      "title": "Clinical Data",
      "groups": [
        {
          "name": "Test Group Name",
          "status": "normal|borderline|abnormal",
          "rows": [
            {
              "testName": "Test Name",
              "result": "105",
              "unit": "mg/dL",
              "range": "100-200 normal",
              "level": "normal|borderline|abnormal"
            }
          ]
        }
      ]
    },
    "personalisedGuidance": { ... },
    "cheatSheet": { ... }
  }
}
```

See [sample-150pages-report.json](sample-150pages-report.json) for a complete example.

---

## Features

### ✅ Implemented
- **150-Page Generation:** Text-heavy content with 130 clinical data groups
- **Professional Layout:** Multi-section reports with:
  - Cover page with branding and patient greeting
  - Introduction & section overview
  - Health summary with numerical scoring
  - Test summary with status cards (normal/borderline/abnormal)
  - Action plan with clinical guidance and suggestions
  - Clinical data tables (chunked 2 per page)
  - Personalized guidance with charts and lifestyle tips
  - Doctor cheat-sheet with QR code
- **Custom Fonts:** Supports TrueType fonts (Bebas Neue, Exo 2, Patrick Hand, Lato, Barlow Condensed, Font Awesome)
- **Fast Generation:** ~3.4 seconds for 150-page reports
- **Flexible Output:** Default to Downloads folder or specify custom path
- **Responsive Layout:** Automatic page breaks and content chunking
- **QR Code Generation:** Auto-generated for cheat-sheet sections

---

## Build & Run

### Debug Build
```bash
dotnet build
dotnet run -- sample-report.json output.pdf
```

### Release Build
```bash
dotnet build -c Release
dotnet bin/Release/net10.0/PdfGenerator.dll sample-report.json output.pdf
```

### Benchmarking
```powershell
$sw = [System.Diagnostics.Stopwatch]::StartNew()
dotnet run -- sample-150pages-report.json output.pdf
$sw.Stop()
Write-Output "Time: $($sw.Elapsed.TotalSeconds) seconds"
```

### Cleanup Generated Files
```powershell
Remove-Item -Recurse -Force bin, obj -ErrorAction SilentlyContinue
Remove-Item -Force output.txt, test-output.pdf, benchmark-output\*.pdf -ErrorAction SilentlyContinue
```

---

## Sample Data

### Minimal Sample (8 Pages)
- **File:** `sample-report.json`
- **Content:** 1 clinical group + all fixed sections (cover, intro, health summary, etc.)
- **Size:** ~0.61 MB
- **Time:** ~0.9 seconds
- **Use Case:** Testing, quick validation

**Generate:**
```bash
dotnet run -- sample-report.json ~/Downloads/minimal-report.pdf
```

### Full Sample (150 Pages)
- **File:** `sample-150pages-report.json`
- **Content:** 130 clinical groups + expanded text narrative
- **Size:** ~3.81 MB
- **Time:** ~3.4 seconds
- **Use Case:** Stress testing, full-featured example

**Generate:**
```bash
dotnet run -- sample-150pages-report.json ~/Downloads/full-report.pdf
```

---

## Notes

- **Page Calculation:** Pages = 6 (fixed) + health pages + action pages + clinical pages
  - Health pages = ceil(health_summary_rows / 6)
  - Action pages = ceil(action_plan_cards / 2)
  - Clinical pages = ceil(clinical_groups / 2)

- **Text Expansion:** Adding more text to descriptions in JSON increases page count without adding new structured data rows

- **Image Assets:** All branding images (logos, ribbons) can be omitted; the app will skip them gracefully if missing

- **Fonts:** Font files are embedded and registered at startup; missing fonts automatically fall back to system defaults

- **Output Path:** If not specified, PDF is saved to `%UserProfile%\Downloads\` with a timestamp. Use absolute paths for custom locations.

---

## Troubleshooting

### PDF not generating
- Check input JSON syntax: `python -m json.tool sample-report.json`
- Check full sample JSON syntax: `python -m json.tool sample-150pages-report.json`
- Verify asset paths exist: `ls assets/`
- Check build: `dotnet build`
- Ensure patient name and report ID are present in JSON

### Missing fonts in PDF
- Verify font files exist in `assets/fonts/`
- Check font file permissions
- Fonts will fallback to Arial if missing

### Custom output path not working
- Ensure the output directory exists or let the app create it
- Check file permissions on the output directory
- Use absolute paths for clarity: `C:\Users\YourName\Documents\report.pdf`

### QR code not appearing
- QR code generation is automatic
- If missing, check that CheatSheet.QrUrl is populated in JSON

---

## License

This project uses QuestPDF (Community License).

---

## Contact & Support

For questions or issues, refer to the source code comments in `Program.cs` and `Pdf/PatientReportDocument.cs`.
