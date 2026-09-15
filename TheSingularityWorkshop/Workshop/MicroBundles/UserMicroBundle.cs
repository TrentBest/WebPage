using System;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Owns the visitor identity contract for a Workshop Experience.
/// Authentication transport can evolve independently; the Experience only
/// consumes identity intent and the display name presented above the avatar.
/// </summary>
public sealed class UserMicroBundle : IDisposable
{
    public const int BundleId = 2102;

    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public UserMicroBundle()
    {
        _lifecycle = new MicroBundle(BundleId, "WORKSHOP USER", new WebMicroBundleProvider());
        SignInAnonymously();
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;

    public UserIdentity Identity { get; private set; } = null!;
    public bool IsAuthenticated => Identity.IsAuthenticated;
    public bool IsAnonymous => Identity.IsAnonymous;
    public string DisplayName => Identity.DisplayName;

    /// <summary>Creates a stable anonymous visitor identity for this Experience.</summary>
    public void SignInAnonymously()
    {
        if (_disposed) return;
        Identity = UserIdentity.Anonymous();
    }

    /// <summary>
    /// Promotes the visitor to an account-shaped identity. Persistence and
    /// credential verification belong to the eventual host authentication provider.
    /// </summary>
    public void CreateAccount(string displayName)
    {
        if (_disposed) return;
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("A display name is required.", nameof(displayName));

        Identity = new UserIdentity(
            Guid.NewGuid(),
            displayName.Trim(),
            IsAuthenticated: true,
            IsAnonymous: false);
    }

    public void Update() => _lifecycle.Update();

    public void Invalidate() => _lifecycle.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;
        _lifecycle.Dispose();
        _disposed = true;
    }
}

/// <summary>Host-neutral identity data exposed by the Workshop user capability.</summary>
public sealed record UserIdentity(
    Guid Id,
    string DisplayName,
    bool IsAuthenticated,
    bool IsAnonymous)
{
    public static UserIdentity Anonymous() =>
        new(Guid.NewGuid(), "Anonymous", IsAuthenticated: false, IsAnonymous: true);
}
