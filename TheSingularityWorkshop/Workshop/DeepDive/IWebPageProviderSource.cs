using TheSingularityWorkshop.MicroBundleDomain;

namespace TheSingularityWorkshop.Workshop.DeepDive;

/// <summary>
/// Optional provider collection boundary exposed only by the WebPage host.
/// The shared FSM_COS contract remains unaware of WebPage educational capabilities.
/// </summary>
public interface IWebPageProviderSource
{
    T? TryGetProvider<T>() where T : class;
}

/// <summary>
/// WebPage extension surface for acquiring optional providers from a composed MicroBundle.
/// </summary>
public static class WebPageProviderExtensions
{
    public static T? TryGetProvider<T>(
        this TheSingularityWorkshop.MicroBundleDomain.IMicroBundle microBundle)
        where T : class
        => microBundle is IWebPageProviderSource source
            ? source.TryGetProvider<T>()
            : null;
}
