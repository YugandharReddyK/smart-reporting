$report = @{
    Report = @{
        Branding = @{
            OrganisationName = "Agilus Diagnostics"
            LogoPath = "assets/logo.svg"
            CoverHeartPath = "assets/cover-heart.png"
            FooterRibbonPath = "assets/footer-ribbon.png"
        }
        GeneratedOn = "2023-11-01T12:00:00Z"
        Patient = @{ Name = "John Doe"; Age = 45; Gender = "Male"; ReportId = "123" }
        Cover = @{ GreetingName = "John" }
        Introduction = @{ OverviewItems = @() }
        HealthSummary = @{
            Rows = (1..180 | ForEach-Object { @{ Name = "M"; Previous = "1"; Current = "2"; Trend = "Up"; Status = "N" } })
        }
        TestSummary = @{ Cards = @() }
        ActionPlan = @{
            Cards = (1..70 | ForEach-Object { @{ Name = "A"; Summary = "S"; Suggestions = "G"; Status = "C" } })
        }
        ClinicalData = @{
            Groups = (1..132 | ForEach-Object { @{ Name = "Group "; Status = "N"; Rows = @( @{ TestName = "T1" } ) } })
        }
        PersonalisedGuidance = @{
            Charts = (1..60 | ForEach-Object { @{ Title = "C"; DataPoints = @( @{ Date = "D1"; Value = 1 } ) } })
            Tips = (1..6 | ForEach-Object { @{ Title = "T"; Description = "D" } })
        }
        CheatSheet = @{ Questions = @("Q1") }
    }
}
$json = $report | ConvertTo-Json -Depth 10
[System.IO.File]::WriteAllText("sample-150pages-report.json", $json, (New-Object System.Text.UTF8Encoding $false))
Write-Host "h=180, a=70, c=60, g=132"
$pages = 4 + [Math]::Ceiling(180/6) + [Math]::Ceiling(70/2) + [Math]::Ceiling(60/2) + [Math]::Ceiling(132/4)
Write-Host "pages=$pages"
