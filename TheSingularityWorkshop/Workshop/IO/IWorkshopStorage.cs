namespace TheSingularityWorkshop.Workshop.IO;

public interface IWorkshopStorage
{
    string? Get(string key);
    ValueTask SetAsync(string key, string value);
    ValueTask RemoveAsync(string key);
}
