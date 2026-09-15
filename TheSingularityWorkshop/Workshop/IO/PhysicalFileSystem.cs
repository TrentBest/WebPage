using System.IO;

namespace TheSingularityWorkshop.Workshop.IO;

public sealed class PhysicalFileSystem : IFileSystem
{
    public bool Exists(string path) => File.Exists(path);
    public void Delete(string path) => File.Delete(path);
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public IBinaryStream OpenRead(string path) =>
        new StreamBinaryStream(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));

    public IBinaryStream OpenWrite(string path) =>
        new StreamBinaryStream(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None));
}
