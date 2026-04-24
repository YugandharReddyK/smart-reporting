# Smart Health Report Generator

.NET 10 console app that generates styled PDF health reports using **Microsoft Playwright** (headless Chromium) and **SkiaSharp** for chart rendering. All assets (CSS, JS, fonts, SVGs) are bundled as embedded resources — **zero network calls** at runtime.

---

## Quick Start

```bash
dotnet restore
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
dotnet run
```

Output PDF is saved to `~/Downloads` by default (override with `REPORT_OUTPUT_DIR` environment variable).

---

## Performance

### 8-Page Report

| Metric          | Value            |
|-----------------|------------------|
| Generation Time | **~2.8 seconds** |
| PDF Size        | **~509 KB**      |

### 152-Page Report (19 × 8 pages)

| Metric          | Value            |
|-----------------|------------------|
| Generation Time | **~6.0 seconds** |
| PDF Size        | **~7,830 KB**    |

> Averaged over 3 runs in Release mode.

---

## Pages (per set)

1. **Cover** — Patient info, health score gauge
2. **How to Read** — Reading guide
3. **Health Summary** — Parameter cards with trends
4. **Test Summary** — Test result cards
5. **Action Plan** — Recommendations
6. **Clinical Data** — Lab results table
7. **Personalised Guidance** — SkiaSharp line charts + diet/lifestyle tips
8. **Doctor Cheat Sheet** — Questions for doctor

---

## Tech Stack

| Component  | Technology |
|------------|------------|
| Runtime    | .NET 10 Console App |
| PDF Engine | Microsoft Playwright 1.59.0 (headless Chromium) |
| Charts     | SkiaSharp 3.119.2 (server-side rendered to PNG) |
| QR Code    | qrcode-generator (bundled JS) |
| Fonts      | Google Fonts — base64 WOFF2 (embedded) |
| Container  | Docker multi-stage build |

---

## Project Structure

```
SmartHealthReport/
├── Program.cs                    # Entry point
├── SmartHealthReport.csproj      # Project config + embedded resources
├── Dockerfile                    # Multi-stage Docker build
├── Models/ReportData.cs          # Data models
├── Data/SampleData.cs            # Sample patient JSON
├── Services/
│   ├── PdfGeneratorService.cs    # Chromium lifecycle + PDF export
│   ├── ChartRenderer.cs          # SkiaSharp chart rendering
│   └── HtmlTemplateBuilder.cs    # Composes HTML from page builders
└── Templates/
    ├── AssetLoader.cs            # Cached embedded resource loader
    ├── ScriptDataBuilder.cs      # Chart data + JS injection
    ├── Assets/                   # CSS, JS, fonts, SVGs (all embedded)
    ├── Pages/                    # 8 page builders
    ├── Components/               # Stepper, Footer
    └── Helpers/                  # HTML encoding, SVG icons
```

---

## Docker

```bash
docker build -t smart-health-report .
docker run --rm -v $(pwd)/output:/output smart-health-report
```
