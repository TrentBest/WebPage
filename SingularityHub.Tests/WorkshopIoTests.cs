using TheSingularityWorkshop.FSM_Serialization;
using TheSingularityWorkshop.Workshop.IO;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopIoTests
{
    [Fact]
    public void BinaryPackAndUnpackCanShareAStream()
    {
        var value = new TestValue(42);
        using var stream = new TheSingularityWorkshop.FSM_Serialization.StreamBinaryStream(new MemoryStream());

        value.Pack(stream);
        stream.Position = 0;

        var restored = new TestValue();
        restored.Unpack(stream);

        Assert.Equal(42, restored.Value);
    }

    [Fact]
    public void FileSystemCanBeExercisedWithoutThePhysicalFileSystem()
    {
        var files = new InMemoryFileSystem();

        using (var writer = files.OpenWrite("state.bin"))
        {
            writer.Write(new byte[] { 1, 2, 3, 4 });
            writer.Flush();
        }

        Assert.True(files.Exists("state.bin"));

        using var reader = files.OpenRead("state.bin");
        var buffer = new byte[4];
        Assert.Equal(4, reader.Read(buffer));
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, buffer);
    }

    [Fact]
    public void BinaryStreamCanPipeBytesBetweenImplementations()
    {
        using var source = new TheSingularityWorkshop.Workshop.IO.MemoryBinaryStream(new byte[] { 9, 8, 7, 6 });
        using var destination = new TheSingularityWorkshop.Workshop.IO.MemoryBinaryStream();

        var buffer = new byte[2];
        int read;
        while ((read = source.Read(buffer)) > 0)
            destination.Write(buffer.AsSpan(0, read));

        Assert.Equal(new byte[] { 9, 8, 7, 6 }, destination.ToArray());
    }

    private sealed class TestValue : IBinaryPackable, IBinaryUnpackable
    {
        public TestValue() { }
        public TestValue(int value) => Value = value;
        public int Value { get; private set; }

        public void Pack(TheSingularityWorkshop.FSM_Serialization.IBinaryStream stream)
        {
            Span<byte> bytes = stackalloc byte[4];
            BitConverter.TryWriteBytes(bytes, Value);
            stream.Write(bytes);
        }

        public void Unpack(TheSingularityWorkshop.FSM_Serialization.IBinaryStream stream)
        {
            Span<byte> bytes = stackalloc byte[4];
            if (stream.Read(bytes) != 4)
                throw new EndOfStreamException();
            Value = BitConverter.ToInt32(bytes);
        }
    }
}
