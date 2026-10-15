using System;
using System.Runtime.InteropServices;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Digitens;

/// <summary>
/// FSM_API context for one Digiten. This reference-type adapter owns identity/lifecycle;
/// the frequently updated world record is the separate blittable <see cref="DigitenState"/>.
/// </summary>
/// <remarks>
/// This is the first CPU reference model in the WebPage host, not yet a standalone NuGet
/// contract or a claim of GPU execution. Keep device-facing state free of strings,
/// collections, object references, and host/UI types.
/// </remarks>
public sealed class Digiten : IStateContext
{
    public Digiten(ulong id)
    {
        if (id == 0)
            throw new ArgumentOutOfRangeException(nameof(id), "A Digiten requires a non-zero stable identity.");

        Name = $"Digiten:{id:x16}";
        State = new DigitenState
        {
            Id = id,
            LifeStage = DigitenLifeStage.Child,
            Activity = DigitenActivity.Idle,
            Employment = DigitenEmploymentStatus.NotOfWorkingAge,
            Health = byte.MaxValue,
            Hunger = 0,
            Rest = 0,
            Social = 0,
            Safety = 0,
            Purpose = 0
        };
        IsValid = true;
    }

    /// <summary>Stable FSM_API context name. Device records refer to entities by numeric ID.</summary>
    public string Name { get; set; }

    /// <summary>Controls whether FSM_API should continue to consider this context active.</summary>
    public bool IsValid { get; set; }

    /// <summary>The compact, reference-free record intended for bulk simulation.</summary>
    public DigitenState State { get; private set; }

    /// <summary>Replace mutable state without changing this context's stable identity.</summary>
    public void SetState(in DigitenState state)
    {
        if (!IsValid)
            throw new InvalidOperationException("An invalidated Digiten cannot be updated.");

        if (state.Id != State.Id)
            throw new ArgumentException("A state update cannot change a Digiten's stable identity.", nameof(state));

        State = state;
    }

    public void Invalidate() => IsValid = false;
}

/// <summary>
/// Compact primitive-only state for a Digiten. IDs address shared tables for household,
/// home, workplace, school, employer, and occupation instead of embedding object graphs.
/// </summary>
/// <remarks>
/// Layout is intentionally explicit and reference-free for bulk transfer experiments.
/// The current CPU contract is 68 bytes; that is a starting measurement, not a promise
/// that every future attribute belongs in the hot record. Detailed skills, relationships,
/// histories, inventories, and narrative belong in keyed side tables or MicroBundles.
/// Position is world state; movement integration remains the responsibility of Physics.
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct DigitenState
{
    public ulong Id;

    public uint HouseholdId;
    public uint HomeId;
    public uint WorkplaceId;
    public uint SchoolId;
    public uint EmployerId;
    public uint OccupationId;

    public float X;
    public float Y;
    public float Z;

    /// <summary>Age in days; demographic reports can derive age bands from this value.</summary>
    public uint AgeDays;

    /// <summary>Integer minor currency units; the world profile defines the currency.</summary>
    public int MoneyCents;

    /// <summary>Normalized need meters: 0 means satisfied, 255 means urgent.</summary>
    public byte Hunger;
    public byte Rest;
    public byte Social;
    public byte Safety;
    public byte Purpose;

    /// <summary>Normalized aggregate condition; detailed health can live in an extension.</summary>
    public byte Health;

    /// <summary>Coarse aggregate skill signal; individual skills belong in a keyed skill table.</summary>
    public byte SkillScore;

    public DigitenEducationLevel Education;
    public DigitenActivity Activity;
    public DigitenLifeStage LifeStage;
    public DigitenEmploymentStatus Employment;
    public byte ScheduleSlot;
    public DigitenStateFlags Flags;

    // Explicit tail bytes make the record's four-byte-aligned transfer size stable.
    public byte Reserved0;
    public byte Reserved1;
    public byte Reserved2;
}

public enum DigitenLifeStage : byte
{
    Infant,
    Child,
    Teen,
    Adult,
    Elder
}

public enum DigitenActivity : byte
{
    Idle,
    Sleeping,
    Eating,
    Socializing,
    Studying,
    Commuting,
    Working,
    JobSeeking,
    Shopping,
    Caring,
    Recreation,
    Recovering
}

public enum DigitenEmploymentStatus : byte
{
    NotOfWorkingAge,
    Student,
    Employed,
    SeekingWork,
    SelfEmployed,
    Retired,
    UnableToWork
}

public enum DigitenEducationLevel : byte
{
    None,
    Primary,
    Secondary,
    Vocational,
    Undergraduate,
    Graduate,
    Doctoral
}

[Flags]
public enum DigitenStateFlags : byte
{
    None = 0,
    HasHome = 1 << 0,
    HasWorkplace = 1 << 1,
    EnrolledInSchool = 1 << 2,
    HasHousehold = 1 << 3,
    IsVisitor = 1 << 4,
    NeedsAssistance = 1 << 5
}
