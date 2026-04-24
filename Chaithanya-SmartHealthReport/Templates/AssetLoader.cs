using System.Collections.Concurrent;
using System.Reflection;

namespace SmartHealthReport.Templates;

public static class AssetLoader
{
    private static readonly Assembly _assembly = typeof(AssetLoader).Assembly;
    private static readonly ConcurrentDictionary<string, string> _cache = new();

    public static string Load(string fileName)
    {
        return _cache.GetOrAdd(fileName, key =>
        {
            var resourceName = _assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(key, StringComparison.OrdinalIgnoreCase))
                ?? throw new FileNotFoundException($"Embedded resource '{key}' not found.");

            using var stream = _assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        });
    }
}
