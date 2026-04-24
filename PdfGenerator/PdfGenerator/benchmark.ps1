$outputPath = "benchmark-output/report-150pages-svg.pdf"

if (-not (Test-Path "benchmark-output")) {
    New-Item -ItemType Directory -Path "benchmark-output" | Out-Null
}

$sw = [System.Diagnostics.Stopwatch]::StartNew()
$process = Start-Process dotnet -ArgumentList "run -- sample-150pages-report.json $outputPath" -Wait -PassThru -NoNewWindow
$sw.Stop()

$sizeMB = if (Test-Path $outputPath) { (Get-Item $outputPath).Length / 1MB } else { 0 }

Write-Host "Renderer: SVG"
Write-Host "ExitCode: $($process.ExitCode)"
Write-Host "Time: $([Math]::Round($sw.Elapsed.TotalSeconds, 3)) s"
Write-Host "Output: $outputPath"
Write-Host "Size: $([Math]::Round($sizeMB, 3)) MB"
