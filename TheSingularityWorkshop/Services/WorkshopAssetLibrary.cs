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
    private const string IndexKey = Prefix + "index";

    public async ValueTask SaveAsync(WorkshopAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        var bytes = WorkshopAssetBinaryCodec.Serialize(asset);
        await storage.SetAsync(Key(asset.Name), Convert.ToBase64String(bytes));

        var names = (await ListAsync()).ToHashSet(StringComparer.Ordinal);
        names.Add(asset.Name);
        await SaveIndexAsync(names);
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

        var names = (await ListAsync()).ToHashSet(StringComparer.Ordinal);
        names.Remove(name);
        await SaveIndexAsync(names);
    }

    /// <summary>Returns the names currently recorded by the library index.</summary>
    public async ValueTask<IReadOnlyList<string>> ListAsync()
    {
        var encoded = await storage.GetAsync(IndexKey);
        if (string.IsNullOrWhiteSpace(encoded))
            return Array.Empty<string>();

        return encoded
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();
    }

    private async ValueTask SaveIndexAsync(IEnumerable<string> names)
        => await storage.SetAsync(IndexKey, string.Join('\n', names.OrderBy(x => x, StringComparer.Ordinal)));

    private string Key(string name) => Prefix + name;
}
