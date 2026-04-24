using System.Text;

namespace SmartHealthReport.Templates.Components;

public static class StepperComponent
{
    private static readonly (string Label, string Icon, string PageId)[] Steps =
    [
        ("Health Summary",           @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""22"" height=""22"" viewBox=""0 0 300 300"" fill=""none""><path d=""M250 0C277.614 3.2213e-06 300 22.3858 300 50V250C300 277.614 277.614 300 250 300H50C22.3858 300 8.0543e-07 277.614 0 250V50C3.22172e-06 22.3858 22.3858 8.05326e-07 50 0H250ZM55 213C49.4772 213 45 217.477 45 223V233C45 238.523 49.4772 243 55 243H205C210.523 243 215 238.523 215 233V223C215 217.477 210.523 213 205 213H55ZM55 158C49.4772 158 45 162.477 45 168V178C45 183.523 49.4772 188 55 188H235C240.523 188 245 183.523 245 178V168C245 162.477 240.523 158 235 158H55ZM93.9648 52.4658C85.1465 43.6479 70.6111 43.886 61.499 52.998C52.3868 62.1103 52.1485 76.6466 60.9668 85.4648L94.4971 118.995L127.496 85.9971C136.608 76.8848 136.847 62.3486 128.028 53.5303C119.21 44.712 104.674 44.9503 95.5615 54.0625L93.9648 52.4658Z"" fill=""white""/></svg>", "page-health-summary"),
        ("Test Summary",             @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""22"" height=""22"" viewBox=""0 0 261 227"" fill=""none""><path fill-rule=""evenodd"" clip-rule=""evenodd"" d=""M115 0c2.76 0 5 2.24 5 5v11.62c0 2.76-2.24 5-5 5h-5V177c0 27.61-22.39 50-50 50S10 204.61 10 177V21.62H5c-2.76 0-5-2.24-5-5V5c0-2.76 2.24-5 5-5h110zM40 136c-2.76 0-5 2.24-5 5s2.24 5 5 5h15c2.76 0 5-2.24 5-5s-2.24-5-5-5H40zm0-32c-2.76 0-5 2.24-5 5s2.24 5 5 5h15c2.76 0 5-2.24 5-5s-2.24-5-5-5H40zm0-32c-2.76 0-5 2.24-5 5s2.24 5 5 5h15c2.76 0 5-2.24 5-5s-2.24-5-5-5H40zm0-47c-2.76 0-5 2.24-5 5v20c0 2.76 2.24 5 5 5h40c2.76 0 5-2.24 5-5V30c0-2.76-2.24-5-5-5H40z"" fill=""white""/><path fill-rule=""evenodd"" clip-rule=""evenodd"" d=""M256 0c2.76 0 5 2.24 5 5v11.62c0 2.76-2.24 5-5 5h-5V177c0 27.61-22.39 50-50 50s-50-22.39-50-50V21.62h-5c-2.76 0-5-2.24-5-5V5c0-2.76 2.24-5 5-5h110zM181 25c-2.76 0-5 2.24-5 5v48c.84-.63 1.87-1 3-1h16c2.76 0 5 2.24 5 5s-2.24 5-5 5h-16c-1.13 0-2.16-.37-3-1v23c.84-.63 1.87-1 3-1h15c2.76 0 5 2.24 5 5s-2.24 5-5 5h-15c-1.13 0-2.16-.37-3-1v23c.84-.63 1.87-1 3-1h15c2.76 0 5 2.24 5 5s-2.24 5-5 5h-15c-1.13 0-2.16-.37-3-1v17c0 2.76 2.24 5 5 5h40c2.76 0 5-2.24 5-5V30c0-2.76-2.24-5-5-5h-40z"" fill=""white""/></svg>", "page-test-summary"),
        ("Action Plan",              @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""22"" height=""22"" viewBox=""0 0 87 83"" fill=""none""><path d=""M41.5 1.38C42.1-.46 44.7-.46 45.3 1.38l8.87 27.31c.27.82 1.04 1.38 1.9 1.38h28.72c1.94 0 2.74 2.48 1.18 3.62L62.74 50.58c-.7.51-1 1.41-.73 2.24l8.87 27.31c.6 1.84-1.51 3.37-3.08 2.23L44.58 65.48c-.7-.51-1.65-.51-2.35 0L18.99 82.36c-1.57 1.14-3.68-.39-3.08-2.24l8.87-27.31c.27-.82-.02-1.73-.73-2.24L.83 33.69c-1.57-1.14-.81-3.62 1.18-3.62h28.72c.87 0 1.63-.56 1.9-1.38L41.5 1.38z"" fill=""white""/></svg>", "page-action-plan"),
        ("Clinical Data",            @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""22"" height=""22"" viewBox=""0 0 100 100""><rect x=""10"" y=""20"" width=""80"" height=""60"" rx=""8"" ry=""8"" fill=""none"" stroke=""white"" stroke-width=""6""/><line x1=""10"" y1=""40"" x2=""90"" y2=""40"" stroke=""white"" stroke-width=""5""/><line x1=""10"" y1=""57"" x2=""90"" y2=""57"" stroke=""white"" stroke-width=""5""/><line x1=""50"" y1=""20"" x2=""50"" y2=""80"" stroke=""white"" stroke-width=""5""/></svg>", "page-clinical-data"),
        ("Personalised Suggestions", @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""22"" height=""22"" viewBox=""0 0 24 24""><path fill=""white"" d=""M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 5.42 4.42 3 7.5 3c1.74 0 3.41.81 4.5 2.09C13.09 3.81 14.76 3 16.5 3 19.58 3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z""/></svg>", "page-guidance"),
        ("Doctor Cheat-sheet",       @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""22"" height=""22"" viewBox=""0 0 24 24"" fill=""none"" stroke=""#fff"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><path d=""M21.44 11.05l-9.19 9.19a6 6 0 0 1-8.49-8.49l9.19-9.19a4 4 0 0 1 5.66 5.66l-9.2 9.19a2 2 0 0 1-2.83-2.83l8.49-8.48""/></svg>", "page-cheatsheet")
    ];

    public static string Render(string activeStep)
    {
        var sb = new StringBuilder("<div class=\"stepper\">\n");
        foreach (var (label, icon, pageId) in Steps)
        {
            bool active = label == activeStep;
            string dotCls = active ? "stepper-dot active" : "stepper-dot";
            string lblCls = active ? "stepper-label active" : "stepper-label";
            sb.Append($"""<a href="#{pageId}" class="stepper-item"><div class="{dotCls}">{icon}</div><span class="{lblCls}">{label}</span></a>""");
            sb.Append('\n');
        }
        sb.Append("</div>\n");
        return sb.ToString();
    }
}
