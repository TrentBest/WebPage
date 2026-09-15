namespace TheSingularityWorkshop.Workshop.IO;

/// <summary>
/// Platform-neutral binary stream contract used by Workshop serialization and storage.
/// Implementations may wrap memory, files, pipes, network streams, or browser resources.
/// </summary>
public interface IBinaryStream : IDisposable
{
    bool CanRead { get; }
    bool CanWrite { get; }
    long Position { get; set; }
    long Length { get; }

    int Read(Span<byte> buffer);
    void Write(ReadOnlySpan<byte> buffer);
    void Flush();
}
