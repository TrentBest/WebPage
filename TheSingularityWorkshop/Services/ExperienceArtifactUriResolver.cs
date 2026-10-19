namespace TheSingularityWorkshop.Services;

/// <summary>
/// Resolves a traversal-free relative Experience manifest path against either the
/// application's configured HttpClient base address or an explicitly trusted HTTPS
/// artifact base URI. This selects static JSON artifacts; it does not load executable code.
/// </summary>
public sealed class ExperienceArtifactUriResolver
{
    private readonly Uri? _baseUri;

    public ExperienceArtifactUriResolver(Uri? baseUri = null)
    {
        if (baseUri is null)
            return;

        if (!baseUri.IsAbsoluteUri ||
            !string.Equals(baseUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
            !string.IsNullOrEmpty(baseUri.UserInfo) ||
            !string.IsNullOrEmpty(baseUri.Query) ||
            !string.IsNullOrEmpty(baseUri.Fragment))
        {
            throw new ArgumentException(
                "The Experience artifact base must be an absolute HTTPS URI without credentials, query, or fragment.",
                nameof(baseUri));
        }

        var builder = new UriBuilder(baseUri);
        if (!builder.Path.EndsWith("/", StringComparison.Ordinal))
            builder.Path += "/";
        _baseUri = builder.Uri;
    }

    public Uri Resolve(string relativeManifestPath)
    {
        ValidateRelativePath(relativeManifestPath);
        return _baseUri is null
            ? new Uri(relativeManifestPath, UriKind.Relative)
            : new Uri(_baseUri, relativeManifestPath);
    }

    private static void ValidateRelativePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (path.StartsWith("/", StringComparison.Ordinal) ||
            path.Contains("://", StringComparison.Ordinal) ||
            path.Contains('\\') ||
            path.Contains('?') ||
            path.Contains('#') ||
            path.Contains('%') ||
            path.Split('/').Any(segment => segment is "" or "." or "..") ||
            path.Any(char.IsControl))
        {
            throw new ArgumentException(
                "Manifest selection must be a relative path without traversal, encoded segments, query, or fragment.",
                nameof(path));
        }
    }
}
