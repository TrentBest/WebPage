using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Laboratory;

/// <summary>
/// A user-configurable laboratory room. Unlike the canonical catalog, a configurable
/// room starts empty and acquires its scientific identity, instruments, and experiments
/// through explicit configuration.
/// </summary>
public sealed class ConfigurableLaboratoryRoom
{
    private readonly List<LaboratoryInstrument> _instruments = [];
    private readonly List<LaboratoryExperiment> _experiments = [];

    public ConfigurableLaboratoryRoom(string id, string name = "Empty Laboratory")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name;
    }

    public string Id { get; }
    public string Name { get; private set; }
    public LaboratoryDiscipline? Discipline { get; private set; }
    public string Description { get; private set; } = "Unconfigured laboratory space.";
    public IReadOnlyList<LaboratoryInstrument> Instruments => _instruments;
    public IReadOnlyList<LaboratoryExperiment> Experiments => _experiments;

    public ConfigurableLaboratoryRoom Configure(
        string name,
        LaboratoryDiscipline discipline,
        string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        Name = name;
        Discipline = discipline;
        Description = description;
        return this;
    }

    public ConfigurableLaboratoryRoom AddInstrument(LaboratoryInstrument instrument)
    {
        ArgumentNullException.ThrowIfNull(instrument);
        if (_instruments.Any(x => string.Equals(x.Id, instrument.Id, StringComparison.Ordinal)))
            throw new InvalidOperationException($"Instrument '{instrument.Id}' is already configured in room '{Id}'.");

        _instruments.Add(instrument);
        return this;
    }

    public ConfigurableLaboratoryRoom AddExperiment(LaboratoryExperiment experiment)
    {
        ArgumentNullException.ThrowIfNull(experiment);
        if (_experiments.Any(x => string.Equals(x.Id, experiment.Id, StringComparison.Ordinal)))
            throw new InvalidOperationException($"Experiment '{experiment.Id}' is already configured in room '{Id}'.");

        _experiments.Add(experiment);
        return this;
    }

    public void Clear()
    {
        _instruments.Clear();
        _experiments.Clear();
        Discipline = null;
        Name = "Empty Laboratory";
        Description = "Unconfigured laboratory space.";
    }
}

/// <summary>
/// Factory for empty research space. The returned rooms are intentionally blank;
/// users decide what scientific capability belongs there.
/// </summary>
public static class ConfigurableLaboratory
{
    public static ConfigurableLaboratoryRoom CreateEmptyRoom(string id) =>
        new(id);
}
