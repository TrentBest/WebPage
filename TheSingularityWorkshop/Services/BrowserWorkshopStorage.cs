using Microsoft.JSInterop;
using TheSingularityWorkshop.Workshop.IO;

namespace TheSingularityWorkshop.Services;

public sealed class BrowserWorkshopStorage(IJSRuntime js) : IWorkshopStorage
{
    public ValueTask<string?> GetAsync(string key) =>
        js.InvokeAsync<string?>("localStorage.getItem", key);

    public ValueTask SetAsync(string key, string value) =>
        js.InvokeVoidAsync("localStorage.setItem", key, value);

    public ValueTask RemoveAsync(string key) =>
        js.InvokeVoidAsync("localStorage.removeItem", key);
}
