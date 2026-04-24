using Scriban;
using Scriban.Runtime;
using System.Text.Json;
using SmartReportGenerator.Models;

namespace SmartReportGenerator.Services;

public class TemplateService
{
    public string RenderTemplate(string templatePath, ReportData data, SvgAssetService? svgAssets = null)
    {
        var templateContent = File.ReadAllText(templatePath);
        var template = Template.Parse(templateContent);

        if (template.HasErrors)
        {
            var errors = string.Join("\n", template.Messages.Select(m => m.ToString()));
            throw new InvalidOperationException($"Template parsing errors:\n{errors}");
        }

        // Convert the model to a ScriptObject for Scriban
        var scriptObject = new ScriptObject();
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;
        ImportJsonElement(scriptObject, dict);

        // Inject inline JS libraries (eliminates CDN calls)
        var assetsPath = Path.Combine(Path.GetDirectoryName(templatePath)!, "..", "Assets");
        var chartJsPath = Path.Combine(assetsPath, "chart.umd.min.js");
        var qrCodeJsPath = Path.Combine(assetsPath, "qrcode.min.js");
        scriptObject["chartjs_inline"] = File.Exists(chartJsPath) ? File.ReadAllText(chartJsPath) : "";
        scriptObject["qrcode_inline"] = File.Exists(qrCodeJsPath) ? File.ReadAllText(qrCodeJsPath) : "";

        // Inject SVG assets as template variables
        if (svgAssets is not null)
        {
            var svgObj = new ScriptObject();
            foreach (var kvp in svgAssets.Inline)
                svgObj[kvp.Key] = kvp.Value;
            scriptObject["svg"] = svgObj;

            var svgUriObj = new ScriptObject();
            foreach (var kvp in svgAssets.Base64Uri)
                svgUriObj[kvp.Key] = kvp.Value;
            scriptObject["svg_uri"] = svgUriObj;
        }

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);
        context.MemberRenamer = member => ConvertToSnakeCase(member.Name);

        return template.Render(context);
    }

    private static void ImportJsonElement(ScriptObject target, Dictionary<string, JsonElement> dict)
    {
        foreach (var kvp in dict)
        {
            var key = ConvertToSnakeCase(kvp.Key);
            target[key] = ConvertJsonElement(kvp.Value);
        }
    }

    private static object? ConvertJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                if (element.TryGetInt32(out var intVal)) return intVal;
                if (element.TryGetInt64(out var longVal)) return longVal;
                return element.GetDouble();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
                return null;
            case JsonValueKind.Object:
                var obj = new ScriptObject();
                foreach (var prop in element.EnumerateObject())
                {
                    obj[ConvertToSnakeCase(prop.Name)] = ConvertJsonElement(prop.Value);
                }
                return obj;
            case JsonValueKind.Array:
                var list = new ScriptArray();
                foreach (var item in element.EnumerateArray())
                {
                    list.Add(ConvertJsonElement(item));
                }
                return list;
            default:
                return element.ToString();
        }
    }

    private static string ConvertToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0) result.Append('_');
                result.Append(char.ToLowerInvariant(c));
            }
            else
            {
                result.Append(c);
            }
        }
        return result.ToString();
    }
}
