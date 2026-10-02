using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Chemistry;

/// <summary>
/// Fluent construction boundary for <see cref="Atom"/>.
/// The builder is domain construction; GUI presentation belongs to AtomGuiBuilder.
/// </summary>
public sealed class AtomBuilder
{
    private readonly string _initialName;
    private readonly int _initialAtomicNumber;
    private readonly string _initialSymbol;
    private double _atomicWeight;
    private IReadOnlyList<string> _electronConfiguration = Array.Empty<string>();
    private int _neutrons;
    private int _protons;
    private int _electrons;
    private string _oxidationStates = string.Empty;
    private string _stateAtStp = string.Empty;
    private string _category = string.Empty;
    private ElementOrigin _origin = ElementOrigin.Natural;

    public AtomBuilder(string name, int atomicNumber, string symbol)
    {
        _initialName = string.IsNullOrWhiteSpace(name) ? "Hydrogen" : name.Trim();
        _initialAtomicNumber = atomicNumber > 0 ? atomicNumber : 1;
        _initialSymbol = string.IsNullOrWhiteSpace(symbol) ? "H" : symbol.Trim();
        _protons = _initialAtomicNumber;
        _electrons = _initialAtomicNumber;
    }

    public AtomBuilder WithAtomicWeight(double value) { _atomicWeight = value; return this; }
    public AtomBuilder WithName(string value) { _nameOverride = value; return this; }
    public AtomBuilder WithSymbol(string value) { _symbolOverride = value; return this; }
    public AtomBuilder WithElectronConfiguration(IEnumerable<string> value) { _electronConfiguration = new List<string>(value ?? Array.Empty<string>()); return this; }
    public AtomBuilder WithNeutrons(int value) { _neutrons = value; return this; }
    public AtomBuilder WithProtons(int value) { _protons = value; return this; }
    public AtomBuilder WithElectrons(int value) { _electrons = value; return this; }
    public AtomBuilder WithOxidationStates(string value) { _oxidationStates = value ?? string.Empty; return this; }
    public AtomBuilder WithStateAtStp(string value) { _stateAtStp = value ?? string.Empty; return this; }
    public AtomBuilder WithCategory(string value) { _category = value ?? string.Empty; return this; }
    public AtomBuilder WithOrigin(ElementOrigin value) { _origin = value; return this; }

    private string? _nameOverride;
    private string? _symbolOverride;

    public Atom Build() => new(
        _nameOverride ?? _initialName,
        _symbolOverride ?? _initialSymbol,
        _initialAtomicNumber,
        _atomicWeight,
        _electronConfiguration,
        _neutrons,
        _protons,
        _electrons,
        _oxidationStates,
        _stateAtStp,
        _category,
        _origin);
}
