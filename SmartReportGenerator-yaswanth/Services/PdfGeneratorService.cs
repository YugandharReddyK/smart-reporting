using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Diagnostics;

namespace SmartReportGenerator.Services;

public class PdfTimingBreakdown
{
    public double NewPageMs { get; set; }
    public double SetContentMs { get; set; }
    public double WaitChartJsMs { get; set; }
    public double ChartRenderDelayMs { get; set; }
    public double PdfRenderMs { get; set; }
    public double TotalMs { get; set; }
}

public class PdfGeneratorService
{
    public async Task<(byte[] PdfBytes, PdfTimingBreakdown Timing)> GeneratePdfAsync(string htmlContent)
    {
        var totalSw = Stopwatch.StartNew();
        var timing = new PdfTimingBreakdown();

        // Try PuppeteerSharp's bundled Chromium first, fall back to system Chrome
        string? executablePath = null;
        var fetcher = new BrowserFetcher();
        var installed = fetcher.GetInstalledBrowsers().FirstOrDefault();
        if (installed is not null && File.Exists(installed.GetExecutablePath()))
        {
            // Verify the download is complete by checking for icudtl.dat
            var chromeDir = Path.GetDirectoryName(installed.GetExecutablePath())!;
            if (!File.Exists(Path.Combine(chromeDir, "icudtl.dat")))
            {
                // Incomplete download — try re-downloading
                Directory.Delete(chromeDir, true);
                await fetcher.DownloadAsync();
                installed = fetcher.GetInstalledBrowsers().FirstOrDefault();
                chromeDir = installed is not null ? Path.GetDirectoryName(installed.GetExecutablePath())! : "";
                if (installed is null || !File.Exists(Path.Combine(chromeDir, "icudtl.dat")))
                {
                    // Still broken — use system Chrome
                    executablePath = FindSystemChrome();
                }
            }
        }
        else
        {
            await fetcher.DownloadAsync();
            installed = fetcher.GetInstalledBrowsers().FirstOrDefault();
            var chromeDir = installed is not null ? Path.GetDirectoryName(installed.GetExecutablePath())! : "";
            if (installed is null || !File.Exists(Path.Combine(chromeDir, "icudtl.dat")))
            {
                executablePath = FindSystemChrome();
            }
        }

        // Launch browser
        var launchOptions = new LaunchOptions
        {
            Headless = true,
            Args = ["--no-sandbox", "--disable-setuid-sandbox", "--disable-gpu"]
        };
        if (executablePath is not null)
            launchOptions.ExecutablePath = executablePath;

        await using var browser = await Puppeteer.LaunchAsync(launchOptions);

        // 1. New page
        var sw = Stopwatch.StartNew();
        await using var page = await browser.NewPageAsync();
        sw.Stop();
        timing.NewPageMs = sw.Elapsed.TotalMilliseconds;

        // 2. Set HTML content + wait for DOM
        sw.Restart();
        await page.SetContentAsync(htmlContent, new NavigationOptions
        {
            WaitUntil = [WaitUntilNavigation.DOMContentLoaded],
            Timeout = 180000
        });
        sw.Stop();
        timing.SetContentMs = sw.Elapsed.TotalMilliseconds;

        // 3. Wait for Chart.js to be available
        sw.Restart();
        try
        {
            await page.WaitForFunctionAsync("() => typeof Chart !== 'undefined'", new WaitForFunctionOptions { Timeout = 30000 });
        }
        catch
        {
            Console.WriteLine("  [WARN] Chart.js wait timed out, proceeding...");
        }
        sw.Stop();
        timing.WaitChartJsMs = sw.Elapsed.TotalMilliseconds;

        // 4. Wait for chart canvases to render
        sw.Restart();
        try
        {
            await page.WaitForFunctionAsync(@"() => {
                const canvases = document.querySelectorAll('canvas');
                if (canvases.length === 0) return true;
                return Array.from(canvases).every(c => {
                    const ctx = c.getContext('2d');
                    if (!ctx) return true;
                    const data = ctx.getImageData(0, 0, c.width, c.height).data;
                    return data.some(v => v !== 0);
                });
            }", new WaitForFunctionOptions { Timeout = 30000, PollingInterval = 200 });
        }
        catch
        {
            Console.WriteLine("  [WARN] Canvas render wait timed out, proceeding...");
        }
        sw.Stop();
        timing.ChartRenderDelayMs = sw.Elapsed.TotalMilliseconds;

        // 5. Generate PDF
        sw.Restart();
        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "0px",
                Bottom = "0px",
                Left = "0px",
                Right = "0px"
            },
            PreferCSSPageSize = true
        });
        sw.Stop();
        timing.PdfRenderMs = sw.Elapsed.TotalMilliseconds;

        totalSw.Stop();
        timing.TotalMs = totalSw.Elapsed.TotalMilliseconds;

        return (pdfBytes, timing);
    }

    private static string? FindSystemChrome()
    {
        string[] candidates =
        [
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Google", "Chrome", "Application", "chrome.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google", "Chrome", "Application", "chrome.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "Application", "chrome.exe")
        ];
        var found = candidates.FirstOrDefault(File.Exists);
        if (found is not null)
            Console.WriteLine($"  [INFO] Using system Chrome: {found}");
        return found;
    }
}