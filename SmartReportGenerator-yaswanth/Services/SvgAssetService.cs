namespace SmartReportGenerator.Services;

public class SvgAssetService
{
    private readonly string _svgBasePath;
    private readonly Dictionary<string, string> _inlineSvgs = new();
    private readonly Dictionary<string, string> _base64Svgs = new();

    public SvgAssetService(string svgBasePath)
    {
        _svgBasePath = svgBasePath;
        LoadAll();
    }

    /// <summary>Raw SVG markup for direct embedding in HTML.</summary>
    public IReadOnlyDictionary<string, string> Inline => _inlineSvgs;

    /// <summary>Base64 data-URI strings for use in img src attributes.</summary>
    public IReadOnlyDictionary<string, string> Base64Uri => _base64Svgs;

    private void LoadAll()
    {
        // Inline SVGs (embedded directly in HTML)
        LoadInline("cover_art", "cover-art.svg");
        LoadInline("logo", "logo.svg");
        LoadInline("footer_stripe", "footer-stripe.svg");
        LoadInline("gauge", "gauge.svg");

        // Stepper icons
        LoadInline("stepper_health_summary", Path.Combine("stepper", "health-summary.svg"));
        LoadInline("stepper_test_summary", Path.Combine("stepper", "test-summary.svg"));
        LoadInline("stepper_action_plan", Path.Combine("stepper", "action-plan.svg"));
        LoadInline("stepper_clinical_data", Path.Combine("stepper", "clinical-data.svg"));
        LoadInline("stepper_personalised", Path.Combine("stepper", "personalised.svg"));
        LoadInline("stepper_cheatsheet", Path.Combine("stepper", "cheatsheet.svg"));

        // Base64 icons (used in <img> tags)
        LoadBase64("action_test_tube", Path.Combine("icons", "action-test-tube.svg"));
        LoadBase64("suggestion_brain", Path.Combine("icons", "suggestion-brain.svg"));
        LoadBase64("exercise", Path.Combine("icons", "exercise.svg"));
        LoadBase64("lose_weight", Path.Combine("icons", "lose-weight.svg"));
        LoadBase64("avoid_sweets", Path.Combine("icons", "avoid-sweets.svg"));
        LoadBase64("carbohydrates", Path.Combine("icons", "carbohydrates.svg"));
        LoadBase64("protein", Path.Combine("icons", "protein.svg"));
        LoadBase64("vegetables", Path.Combine("icons", "vegetables.svg"));
    }

    private void LoadInline(string key, string relativePath)
    {
        var fullPath = Path.Combine(_svgBasePath, relativePath);
        if (File.Exists(fullPath))
            _inlineSvgs[key] = File.ReadAllText(fullPath).Trim();
    }

    private void LoadBase64(string key, string relativePath)
    {
        var fullPath = Path.Combine(_svgBasePath, relativePath);
        if (File.Exists(fullPath))
        {
            var bytes = File.ReadAllBytes(fullPath);
            _base64Svgs[key] = "data:image/svg+xml;base64," + Convert.ToBase64String(bytes);
        }
    }
}
