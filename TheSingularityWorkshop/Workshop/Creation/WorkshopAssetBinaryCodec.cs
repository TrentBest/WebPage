using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.IO;

namespace TheSingularityWorkshop.Workshop.Creation;

/// <summary>
/// Binary codec for named Workshop assets.
/// The format is intentionally small and explicit so the serialized artifact is independent
/// of Blazor, WPF, Unity, or any other renderer.
/// </summary>
public static class WorkshopAssetBinaryCodec
{
    private const uint Magic = 0x57534131; // WSA1
    private const byte Version = 1;

    public static byte[] Serialize(WorkshopAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        using var stream = new MemoryBinaryStream();
        Write(asset, stream);
        return stream.ToArray();
    }

    public static WorkshopAsset Deserialize(ReadOnlySpan<byte> data)
    {
        if (data.Length == 0)
            throw new InvalidDataException("Workshop asset data is empty.");

        using var stream = new MemoryBinaryStream(data.ToArray());
        return Read(stream);
    }

    public static void Write(WorkshopAsset asset, IBinaryStream stream)
    {
        ArgumentNullException.ThrowIfNull(asset);
        ArgumentNullException.ThrowIfNull(stream);

        WriteUInt32(stream, Magic);
        stream.Write(new[] { Version });
        WriteString(stream, asset.Name);
        WriteNode(stream, asset.Gui);

        stream.Write(new[] { (byte)(asset.Fsm is null ? 0 : 1) });
        if (asset.Fsm is not null)
            WriteFsm(stream, asset.Fsm);
    }

    public static WorkshopAsset Read(IBinaryStream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (ReadUInt32(stream) != Magic)
            throw new InvalidDataException("The Workshop asset signature is invalid.");

        var version = ReadByte(stream);
        if (version != Version)
            throw new InvalidDataException($"Unsupported Workshop asset version: {version}.");

        var name = ReadString(stream);
        var gui = ReadNode(stream);
        var hasFsm = ReadByte(stream) != 0;
        var fsm = hasFsm ? ReadFsm(stream) : null;

        return new WorkshopAsset(name, gui, fsm);
    }

    private static void WriteFsm(IBinaryStream stream, FsmBlueprint fsm)
    {
        WriteString(stream, fsm.Name);
        WriteCount(stream, fsm.States.Count);
        foreach (var state in fsm.States)
            WriteString(stream, state);

        WriteString(stream, fsm.InitialState);
        WriteCount(stream, fsm.Transitions.Count);
        foreach (var transition in fsm.Transitions)
        {
            WriteString(stream, transition.From);
            WriteString(stream, transition.To);
            WriteString(stream, transition.Condition);
        }
    }

    private static FsmBlueprint ReadFsm(IBinaryStream stream)
    {
        var name = ReadString(stream);
        var stateCount = ReadCount(stream);
        var states = new string[stateCount];
        for (var i = 0; i < stateCount; i++)
            states[i] = ReadString(stream);

        var initialState = ReadString(stream);
        var transitionCount = ReadCount(stream);
        var transitions = new FsmTransitionBlueprint[transitionCount];

        for (var i = 0; i < transitionCount; i++)
            transitions[i] = new FsmTransitionBlueprint(
                ReadString(stream),
                ReadString(stream),
                ReadString(stream));

        return new FsmBlueprint(name, states, initialState, transitions);
    }

    private static void WriteNode(IBinaryStream stream, GuiNode node)
    {
        WriteString(stream, node.Kind);
        WriteString(stream, node.Id);
        WriteNullableString(stream, node.Text);
        WriteNullableString(stream, node.Source);

        WriteCount(stream, node.Properties.Count);
        foreach (var property in node.Properties)
        {
            WriteString(stream, property.Key);
            WriteString(stream, property.Value);
        }

        WriteCount(stream, node.Children.Count);
        foreach (var child in node.Children)
            WriteNode(stream, child);
    }

    private static GuiNode ReadNode(IBinaryStream stream)
    {
        var kind = ReadString(stream);
        var id = ReadString(stream);
        var text = ReadNullableString(stream);
        var source = ReadNullableString(stream);

        var propertyCount = ReadCount(stream);
        var properties = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var i = 0; i < propertyCount; i++)
            properties.Add(ReadString(stream), ReadString(stream));

        var childCount = ReadCount(stream);
        var children = new GuiNode[childCount];
        for (var i = 0; i < childCount; i++)
            children[i] = ReadNode(stream);

        return new GuiNode(kind, id, text, source, properties, children);
    }

    private static void WriteString(IBinaryStream stream, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        WriteCount(stream, bytes.Length);
        stream.Write(bytes);
    }

    private static void WriteNullableString(IBinaryStream stream, string? value)
    {
        stream.Write(new[] { (byte)(value is null ? 0 : 1) });
        if (value is not null)
            WriteString(stream, value);
    }

    private static string ReadString(IBinaryStream stream)
    {
        var length = ReadCount(stream);
        var bytes = ReadExact(stream, length);
        return Encoding.UTF8.GetString(bytes);
    }

    private static string? ReadNullableString(IBinaryStream stream)
        => ReadByte(stream) == 0 ? null : ReadString(stream);

    private static void WriteCount(IBinaryStream stream, int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        WriteUInt32(stream, checked((uint)value));
    }

    private static int ReadCount(IBinaryStream stream)
    {
        var value = ReadUInt32(stream);
        if (value > int.MaxValue)
            throw new InvalidDataException("Workshop asset collection is too large.");

        return (int)value;
    }

    private static void WriteUInt32(IBinaryStream stream, uint value)
    {
        Span<byte> bytes = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, value);
        stream.Write(bytes);
    }

    private static uint ReadUInt32(IBinaryStream stream)
    {
        var bytes = ReadExact(stream, 4);
        return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    private static byte ReadByte(IBinaryStream stream)
    {
        Span<byte> bytes = stackalloc byte[1];
        if (stream.Read(bytes) != 1)
            throw new EndOfStreamException();

        return bytes[0];
    }

    private static byte[] ReadExact(IBinaryStream stream, int length)
    {
        if (length < 0)
            throw new InvalidDataException("Negative binary length.");

        var bytes = new byte[length];
        var offset = 0;
        while (offset < length)
        {
            var read = stream.Read(bytes.AsSpan(offset));
            if (read <= 0)
                throw new EndOfStreamException();

            offset += read;
        }

        return bytes;
    }
}
