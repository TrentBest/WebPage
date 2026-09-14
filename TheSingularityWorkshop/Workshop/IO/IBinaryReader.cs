namespace TheSingularityWorkshop.Workshop.IO;

public interface IBinaryReader
{
    int Read(Span<byte> buffer);
}
