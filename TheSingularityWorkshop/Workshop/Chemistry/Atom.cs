using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Chemistry;

public enum ElementOrigin
{
    Natural,
    Synthetic,
    Fictional
}

/// <summary>
/// Platform-neutral elemental data. Presentation is deliberately absent.
/// </summary>
public sealed class Atom
{
    public Atom(
        string name,
        string symbol,
        int atomicNumber,
        double atomicWeight,
        IReadOnlyList<string>? electronConfiguration,
        int neutrons,
        int protons,
        int electrons,
        string oxidationStates,
        string stateAtStp,
        string category,
        ElementOrigin origin = ElementOrigin.Natural)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Atom name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentException("Atomic symbol is required.", nameof(symbol));
        if (atomicNumber <= 0) throw new ArgumentOutOfRangeException(nameof(atomicNumber));
        if (protons != atomicNumber) throw new ArgumentException("Protons must equal atomic number (Z).", nameof(protons));
        if (electrons < 0) throw new ArgumentOutOfRangeException(nameof(electrons));
        if (neutrons < 0) throw new ArgumentOutOfRangeException(nameof(neutrons));

        Name = name.Trim();
        Symbol = symbol.Trim();
        AtomicNumber = atomicNumber;
        AtomicWeight = atomicWeight;
        ElectronConfiguration = electronConfiguration is null
            ? Array.Empty<string>()
            : new List<string>(electronConfiguration).AsReadOnly();
        Neutrons = neutrons;
        Protons = protons;
        Electrons = electrons;
        OxidationStates = oxidationStates ?? string.Empty;
        StateAtStp = stateAtStp ?? string.Empty;
        Category = category ?? string.Empty;
        Origin = origin;
    }

    public string Name { get; }
    public string Symbol { get; }
    public int AtomicNumber { get; }
    public double AtomicWeight { get; }
    public IReadOnlyList<string> ElectronConfiguration { get; }
    public int Neutrons { get; }
    public int Protons { get; }
    public int Electrons { get; }
    public string OxidationStates { get; }
    public string StateAtStp { get; }
    public string Category { get; }
    public ElementOrigin Origin { get; }
    public bool IsFictional => Origin == ElementOrigin.Fictional;

    public override string ToString() => $"{Name} ({Symbol}), Z={AtomicNumber}, Ar={AtomicWeight}";
}
