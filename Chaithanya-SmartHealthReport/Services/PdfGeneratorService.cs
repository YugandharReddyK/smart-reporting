using Microsoft.Playwright;
using SmartHealthReport.Models;

namespace SmartHealthReport.Services;

public class PdfGeneratorService : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    private async Task EnsureInitializedAsync()
    {
        if (_browser != null) return;

        await _initLock.WaitAsync();
        try
        {
            if (_browser != null) return;
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<byte[]> GenerateAsync(ReportData data)
    {
        await EnsureInitializedAsync();

        var html = HtmlTemplateBuilder.Build(data);

        await using var context = await _browser!.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.SetContentAsync(html, new PageSetContentOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        // Wait for Chart.js canvases and QR code to finish rendering.
        // report.js sets window.__chartsReady = true after all charts
        // are drawn and the QR code is injected into the DOM.
        await page.WaitForFunctionAsync("() => window.__chartsReady === true", null,
            new PageWaitForFunctionOptions { Timeout = 30000 });

        var pdfBytes = await page.PdfAsync(new PagePdfOptions
        {
            Format = "A4",
            PrintBackground = true,
            Margin = new Margin
            {
                Top = "0",
                Bottom = "0",
                Left = "0",
                Right = "0"
            }
        });

        await page.CloseAsync();
        return pdfBytes;
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser != null)
            await _browser.CloseAsync();
        _playwright?.Dispose();
    }
}
