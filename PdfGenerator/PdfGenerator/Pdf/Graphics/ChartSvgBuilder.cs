using PdfGenerator.Models;
using System.Text;

namespace PdfGenerator.Pdf.Graphics;

internal static class ChartSvgBuilder
{
    public static string GenerateLineChartSvg(GuidanceChart chart, string backgroundColor, string lineColor)
    {
        if (chart.DataPoints.Count < 2)
            return string.Empty;

        const float svgWidth = 280f;
        const float svgHeight = 160f;
        const float leftPadding = 34f;
        const float rightPadding = 12f;
        const float topPadding = 14f;
        const float bottomPadding = 30f;
        const float plotInsetX = 2f;
        const float chartWidth = svgWidth - leftPadding - rightPadding;
        const float chartHeight = svgHeight - topPadding - bottomPadding;

        var dataPoints = chart.DataPoints.ToList();
        var axisScale = ChartScaleHelper.Create(dataPoints.Min(p => p.Value), dataPoints.Max(p => p.Value), minimumIntervals: 4);
        var range = axisScale.Max - axisScale.Min;

        var pointsCount = dataPoints.Count;
        var xStart = leftPadding + plotInsetX;
        var xEnd = leftPadding + chartWidth - plotInsetX;
        var xStep = (xEnd - xStart) / (pointsCount - 1);
        var points = new List<(float X, float Y)>();

        for (var i = 0; i < pointsCount; i++)
        {
            var point = dataPoints[i];
            var normalizedValue = (float)((point.Value - axisScale.Min) / range);
            var x = xStart + (i * xStep);
            var y = topPadding + chartHeight - (normalizedValue * chartHeight);

            points.Add((x, y));
        }

        var yAxisBottom = topPadding + chartHeight;
        var curvedLinePath = BuildSmoothCurvePath(points);
        var areaPath = BuildAreaPath(points, yAxisBottom);

        var svg = new StringBuilder();
        svg.Append($"<svg xmlns='http://www.w3.org/2000/svg' width='{svgWidth}' height='{svgHeight}' viewBox='0 0 {svgWidth} {svgHeight}'>");

        // Keep the bottom border slightly above the x-axis to avoid visual double-stroking.
        svg.Append($"<rect x='{leftPadding}' y='{topPadding}' width='{chartWidth}' height='{chartHeight - 1f}' fill='{backgroundColor}' stroke='#E0E0E0' stroke-width='1' rx='6'/>");
        svg.Append($"<path d='{areaPath}' fill='{lineColor}' fill-opacity='0.10'/>");

        var yAxisX = leftPadding;

        // Draw visible black chart axes.
        svg.Append($"<line x1='{yAxisX}' y1='{topPadding}' x2='{yAxisX}' y2='{yAxisBottom}' stroke='#000000' stroke-width='1.2'/>");

        for (var tickIndex = 0; tickIndex <= axisScale.IntervalCount; tickIndex++)
        {
            var ratio = axisScale.IntervalCount == 0 ? 0f : (float)tickIndex / axisScale.IntervalCount;
            var y = yAxisBottom - (ratio * chartHeight);
            var scaleValue = axisScale.ValueAt(tickIndex);

            svg.Append($"<text x='{leftPadding - 6}' y='{y + 2.8f:F1}' font-size='8' text-anchor='end' fill='#222'>{ChartScaleHelper.FormatLabel(scaleValue, axisScale.Decimals)}</text>");
        }

        svg.Append($"<path d='{curvedLinePath}' fill='none' stroke='{lineColor}' stroke-width='2' stroke-linejoin='round' stroke-linecap='round'/>");

        // Data points and "You" marker
        for (var i = 0; i < pointsCount; i++)
        {
            var point = dataPoints[i];
            var normalizedValue = (float)((point.Value - axisScale.Min) / range);
            var x = xStart + (i * xStep);
            var y = topPadding + chartHeight - (normalizedValue * chartHeight);

            svg.Append($"<circle cx='{x:F1}' cy='{y:F1}' r='2.5' fill='{lineColor}'/>");

            if (point.IsCurrentPoint)
            {
                svg.Append($"<circle cx='{x:F1}' cy='{y:F1}' r='5' fill='none' stroke='{lineColor}' stroke-width='2'/>");
                svg.Append($"<line x1='{x:F1}' y1='{y:F1}' x2='{x:F1}' y2='{yAxisBottom + 8}' stroke='#999' stroke-width='0.5' stroke-dasharray='2,2'/>");
                svg.Append($"<text x='{x:F1}' y='{yAxisBottom + 16}' font-size='7' text-anchor='middle' fill='#333'>You</text>");
            }

            if (pointsCount <= 8 || i % 2 == 0 || i == pointsCount - 1)
            {
                var dateLabel = point.Date;
                svg.Append($"<text x='{x:F1}' y='{yAxisBottom + 24}' font-size='7' text-anchor='middle' fill='#000'>{dateLabel}</text>");
            }
        }

        svg.Append($"<line x1='{leftPadding}' y1='{yAxisBottom}' x2='{leftPadding + chartWidth}' y2='{yAxisBottom}' stroke='#000000' stroke-width='1.2'/>");

        svg.Append("</svg>");
        return svg.ToString();
    }

    private static string BuildSmoothCurvePath(IReadOnlyList<(float X, float Y)> points)
    {
        if (points.Count == 0)
            return string.Empty;

        if (points.Count == 1)
            return FormattableString.Invariant($"M {points[0].X:F1},{points[0].Y:F1}");

        var path = new StringBuilder();
        path.Append(FormattableString.Invariant($"M {points[0].X:F1},{points[0].Y:F1}"));

        for (var i = 0; i < points.Count - 1; i++)
        {
            var p0 = i == 0 ? points[i] : points[i - 1];
            var p1 = points[i];
            var p2 = points[i + 1];
            var p3 = i + 2 < points.Count ? points[i + 2] : p2;

            var cp1x = p1.X + ((p2.X - p0.X) / 6f);
            var cp1y = p1.Y + ((p2.Y - p0.Y) / 6f);
            var cp2x = p2.X - ((p3.X - p1.X) / 6f);
            var cp2y = p2.Y - ((p3.Y - p1.Y) / 6f);

            path.Append(FormattableString.Invariant($" C {cp1x:F1},{cp1y:F1} {cp2x:F1},{cp2y:F1} {p2.X:F1},{p2.Y:F1}"));
        }

        return path.ToString();
    }

    private static string BuildAreaPath(IReadOnlyList<(float X, float Y)> points, float baselineY)
    {
        if (points.Count == 0)
            return string.Empty;

        var curvePath = BuildSmoothCurvePath(points);
        var lastPoint = points[^1];
        var firstPoint = points[0];

        return FormattableString.Invariant($"{curvePath} L {lastPoint.X:F1},{baselineY:F1} L {firstPoint.X:F1},{baselineY:F1} Z");
    }
}
