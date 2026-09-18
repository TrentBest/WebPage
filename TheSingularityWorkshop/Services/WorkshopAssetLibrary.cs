using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSingularityWorkshop.Workshop.Creation;
using TheSingularityWorkshop.Workshop.IO;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// Persistent library of named Workshop assets.
/// Browser storage carries the binary artifact as Base64 while the artifact itself remains binary.
/// The same service boundary can later be backed by a filesystem, server, or AnyApp repository.
/// </summary>
public sealed class WorkshopAssetLibrary(IWorkshopStorage storage)
{
    private const string Prefix = "workshop.asset.";

    public async ValueTask SaveAsync(WorkshopAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        var bytes = WorkshopAssetBinaryCodec.Serialize(asset);
        await storage.SetAsync(Key(asset.Name), Convert.ToBase64String(bytes));
    }

    public async ValueTask<WorkshopAsset?> LoadAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var encoded = await storage.GetAsync(Key(name));
        if (string.IsNullOrWhiteSpace(encoded))
            return null;

        return WorkshopAssetBinaryCodec.Deserialize(Convert.FromBase64String(encoded));
    }

    public async ValueTask DeleteAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        await storage.RemoveAsync(Key(name));
    }

    /// <summary>
    /// Returns names recorded by the library index.
    /// </summary>
    public async ValueTask<IReadOnlyList<string>> ListAsync()
    {
        var encoded = await storage.GetAsync(Prefix + "index");
        if (string.IsNullOrWhiteSpace(encoded))
            return Array.Empty<string>();

        return encoded.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();
    }

    private string Key(string name) => Prefix + name;
}
