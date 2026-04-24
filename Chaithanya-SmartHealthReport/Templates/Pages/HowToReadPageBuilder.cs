using System.Text;
using SmartHealthReport.Templates.Components;
using static SmartHealthReport.Templates.Helpers.HtmlHelpers;

namespace SmartHealthReport.Templates.Pages;

public static class HowToReadPageBuilder
{
    private static readonly (string Name, string Description)[] Sections =
    [
        ("Health Summary",           "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur."),
        ("Test Summary",             "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur."),
        ("Action Plan",              "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur."),
        ("Clinical Data",            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur."),
        ("Personalised Suggestions", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur."),
        ("Doctor Cheat-sheet",       "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.")
    ];

    public static string Build()
    {
        var sb = new StringBuilder("""
            <div class="page" id="page-howto">
                <div class="page-content">
                    <h1 class="report-title">Agilus Health Analytics Report</h1>
                    <hr class="title-divider">
                    <p class="section-desc">The investigations were conducted at Agilus Diagnostics using standardized, quality-controlled laboratory protocols, and the results should be interpreted in conjunction with the patient&#x2019;s clinical findings and other diagnostic information.</p>
                    <h2 class="section-title" style="margin-top:30px;">How to Read the Report</h2>
                      <hr class="title-divider">
                    <p class="section-desc">Below are the sections which depict what you can expect from this report, how you can read this report and use it for your wellbeing.</p>
                    <ul class="sections-list">
            """);

        foreach (var (name, desc) in Sections)
        {
            sb.Append($"""
                <li>
                    <span class="sec-name">{Encode(name)}</span>
                    <div class="sec-desc">
                        <span>{Encode(desc)}</span>
                        <div class="sec-placeholder"></div>
                    </div>
                </li>
                """);
            sb.Append('\n');
        }

        sb.Append($"</ul></div>{FooterComponent.Render()}</div>\n");
        return sb.ToString();
    }
}
