using System.Collections.Generic;
using System.IO;

namespace TheSingularityWorkshop.Workshop.IO;

public sealed class InMemoryFileSystem : IFileSystem
{
    private readonly Dictionary<string, byte[]> _files = new(StringComparer.Ordinal);

    public bool Exists(string path) => _files.ContainsKey(path);
    public void Delete(string path) => _files.Remove(path);
    public void CreateDirectory(string path) { }

    public IBinaryStream OpenRead(string path)
    {
        if (!_files.TryGetValue(path, out var data))
            throw new FileNotFoundException("The requested virtual file does not exist.", path);
        return new MemoryBinaryStream(data);
    }

    public IBinaryStream OpenWrite(string path) => new CommitStream(this, path);

    private sealed class CommitStream : MemoryBinaryStream
    {
        private readonly InMemoryFileSystem _owner;
        private readonly string _path;

        public CommitStream(InMemoryFileSystem owner, string path)
        {
            _owner = owner;
            _path = path;
        }

        public override void Dispose()
        {
            _owner._files[_path] = ToArray();
            base.Dispose();
        }
    }
}
