using SkiaSharp;
using SmartHealthReport.Models;

namespace SmartHealthReport.Services;

/// <summary>
/// Renders line charts as base64-encoded PNGs using SkiaSharp,
/// replacing client-side Chart.js rendering in headless Chromium.
/// </summary>
public static class ChartRenderer
{
    // Render at 2x for crisp PDF output (displayed at 320×140 in HTML)
    private const int Width = 640;
    private const int Height = 280;

    // Chart area (2x pixel coordinates)
    private const float ChartLeft = 60f;
    private const float ChartRight = 620f;
    private const float ChartTop = 44f;
    private const float ChartBottom = 230f;
    private const double YMin = 6.0;
    private const double YMax = 9.0;

    private static readonly string[] Labels =
        ["Dec 18", "Jan 19", "Feb 19", "Mar 19", "Apr 19", "May 19", "Jun 19"];

    public static string[] RenderAllCharts(ReportData data)
    {
        var result = new string[data.PersonalisedGuidance.Count];
        for (int i = 0; i < data.PersonalisedGuidance.Count; i++)
            result[i] = RenderChart(data.PersonalisedGuidance[i]);
        return result;
    }

    private static string RenderChart(GuidanceItem item)
    {
        var status = item.Status.Trim();

        double[] values = status == "Borderline"
            ? [6.7, 7.2, 8.3, 8.5, 8.1, 7.3, 6.8]
            : status == "Normal"
            ? [7.8, 8.0, 8.1, 8.2, 8.0, 7.8, 7.6]
            : [8.2, 8.5, 8.7, 8.3, 7.8, 7.4, 7.0];

        var lineColor = SKColor.Parse(status == "Normal" ? "#4CAF50"
            : status == "Abnormal" ? "#E53935" : "#866800");

        double bandMin = status == "Normal" ? 7.5 : status == "Abnormal" ? 7.0 : 7.5;
        double bandMax = status == "Normal" ? 9.0 : status == "Abnormal" ? 9.0 : 8.5;

        var bandColor = status == "Normal"
            ? new SKColor(154, 254, 187, 77)    // rgba(154,254,187,0.3)
            : status == "Abnormal"
            ? new SKColor(254, 174, 177, 77)    // rgba(254,174,177,0.3)
            : new SKColor(254, 248, 169, 102);  // rgba(254,248,169,0.4)

        const int patientIdx = 2;
        int[] dashLines = [2, 4];

        using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        float chartWidth = ChartRight - ChartLeft;
        float chartHeight = ChartBottom - ChartTop;
        float xStep = chartWidth / (values.Length - 1);

        float MapX(int idx) => ChartLeft + idx * xStep;
        float MapY(double val) => ChartBottom - (float)((val - YMin) / (YMax - YMin)) * chartHeight;

        // 1. Background band (normal range)
        using (var paint = new SKPaint { Color = bandColor, IsAntialias = true })
            canvas.DrawRect(ChartLeft, MapY(bandMax), chartWidth, MapY(bandMin) - MapY(bandMax), paint);

        // 2. Dashed vertical lines
        using (var paint = new SKPaint
        {
            Color = new SKColor(0xBB, 0xBB, 0xBB),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            PathEffect = SKPathEffect.CreateDash([10f, 8f], 0),
            IsAntialias = true
        })
        {
            foreach (var idx in dashLines)
                canvas.DrawLine(MapX(idx), ChartTop, MapX(idx), ChartBottom, paint);
        }

        // 3. Build smooth bezier path through data points
        var points = new SKPoint[values.Length];
        for (int i = 0; i < values.Length; i++)
            points[i] = new SKPoint(MapX(i), MapY(values[i]));

        using var linePath = BuildSmoothPath(points, 0.4f);

        // 4. Fill under curve
        using (var fillPath = new SKPath(linePath))
        {
            fillPath.LineTo(points[^1].X, ChartBottom);
            fillPath.LineTo(points[0].X, ChartBottom);
            fillPath.Close();
            using var paint = new SKPaint
            {
                Color = lineColor.WithAlpha(18),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            canvas.DrawPath(fillPath, paint);
        }

        // 5. Line stroke
        using (var paint = new SKPaint
        {
            Color = lineColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 5f,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        })
        {
            canvas.DrawPath(linePath, paint);
        }

        // 6. Patient marker dot
        using (var paint = new SKPaint { Color = lineColor, IsAntialias = true })
            canvas.DrawCircle(points[patientIdx], 16f, paint);

        // 7. Text labels
        var typeface = SKTypeface.FromFamilyName("Arial") ?? SKTypeface.Default;
        var boldTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold) ?? SKTypeface.Default;

        // "You" label above patient marker
        using (var font = new SKFont(boldTypeface, 22f))
        using (var paint = new SKPaint { Color = new SKColor(0x33, 0x33, 0x33), IsAntialias = true })
            DrawTextCentered(canvas, "You", points[patientIdx].X, points[patientIdx].Y - 24f, font, paint);

        // Y-axis tick labels (6, 7, 8, 9)
        using (var font = new SKFont(typeface, 18f))
        using (var paint = new SKPaint { Color = new SKColor(0x55, 0x55, 0x55), IsAntialias = true })
        {
            for (int v = (int)YMin; v <= (int)YMax; v++)
                DrawTextRight(canvas, v.ToString(), ChartLeft - 10f, MapY(v) + 6f, font, paint);
        }

        // X-axis date labels
        using (var font = new SKFont(typeface, 16f))
        using (var paint = new SKPaint { Color = new SKColor(0x55, 0x55, 0x55), IsAntialias = true })
        {
            for (int i = 0; i < Labels.Length; i++)
                DrawTextCentered(canvas, Labels[i], MapX(i), ChartBottom + 24f, font, paint);
        }

        // "Average" label at bottom center
        using (var font = new SKFont(typeface, 18f))
        using (var paint = new SKPaint { Color = new SKColor(0x88, 0x88, 0x88), IsAntialias = true })
            DrawTextCentered(canvas, "Average", (ChartLeft + ChartRight) / 2f, Height - 10f, font, paint);

        // Encode to PNG
        using var image = surface.Snapshot();
        using var pngData = image.Encode(SKEncodedImageFormat.Png, 90);
        return Convert.ToBase64String(pngData.ToArray());
    }

    private static void DrawTextCentered(SKCanvas canvas, string text, float x, float y, SKFont font, SKPaint paint)
    {
        float width = font.MeasureText(text);
        canvas.DrawText(text, x - width / 2f, y, font, paint);
    }

    private static void DrawTextRight(SKCanvas canvas, string text, float x, float y, SKFont font, SKPaint paint)
    {
        float width = font.MeasureText(text);
        canvas.DrawText(text, x - width, y, font, paint);
    }

    private static SKPath BuildSmoothPath(SKPoint[] pts, float tension)
    {
        var path = new SKPath();
        if (pts.Length < 2) return path;

        path.MoveTo(pts[0]);
        for (int i = 0; i < pts.Length - 1; i++)
        {
            var p0 = i > 0 ? pts[i - 1] : pts[i];
            var p1 = pts[i];
            var p2 = pts[i + 1];
            var p3 = i < pts.Length - 2 ? pts[i + 2] : pts[i + 1];

            var cp1 = new SKPoint(
                p1.X + tension * (p2.X - p0.X) / 3f,
                p1.Y + tension * (p2.Y - p0.Y) / 3f
            );
            var cp2 = new SKPoint(
                p2.X - tension * (p3.X - p1.X) / 3f,
                p2.Y - tension * (p3.Y - p1.Y) / 3f
            );

            path.CubicTo(cp1, cp2, p2);
        }

        return path;
    }
}
