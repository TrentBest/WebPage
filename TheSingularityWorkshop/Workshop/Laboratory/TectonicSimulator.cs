using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Laboratory;

public readonly record struct TectonicPlate(
    string Id,
    IReadOnlyList<int> CellIndices,
    double VelocityXMPerYear,
    double VelocityYMPerYear);

public readonly record struct TectonicEvent(
    long EventId,
    double Magnitude,
    double EnergyJoules,
    double MaximumDisplacementM,
    IReadOnlyList<double> DisplacementM);

/// <summary>
/// Deterministic reduced tectonic substrate. Plates carry horizontal motion;
/// coincident or neighboring plate motion can produce a synthetic fault event.
/// It is intentionally renderer- and geology-engine neutral.
/// </summary>
public sealed class TectonicSimulator
{
    private readonly int _cellCount;
    private readonly List<TectonicPlate> _plates = [];
    private long _eventId;

    public TectonicSimulator(int cellCount)
    {
        if (cellCount <= 0) throw new ArgumentOutOfRangeException(nameof(cellCount));
        _cellCount = cellCount;
    }

    public IReadOnlyList<TectonicPlate> Plates => _plates;

    public void AddPlate(TectonicPlate plate)
    {
        if (string.IsNullOrWhiteSpace(plate.Id))
            throw new ArgumentException("Plate id is required.", nameof(plate));
        if (plate.CellIndices.Count == 0)
            throw new ArgumentException("A plate must contain at least one cell.", nameof(plate));

        foreach (var index in plate.CellIndices)
            if (index < 0 || index >= _cellCount)
                throw new ArgumentOutOfRangeException(nameof(plate));

        _plates.Add(plate);
    }

    public TectonicEvent SimulateFault(
        int sourceCell,
        double slipM,
        double coupling,
        double timeStepYears = 1)
    {
        if (sourceCell < 0 || sourceCell >= _cellCount)
            throw new ArgumentOutOfRangeException(nameof(sourceCell));
        if (slipM < 0) throw new ArgumentOutOfRangeException(nameof(slipM));
        if (coupling < 0) throw new ArgumentOutOfRangeException(nameof(coupling));
        if (timeStepYears <= 0) throw new ArgumentOutOfRangeException(nameof(timeStepYears));

        var displacement = new double[_cellCount];
        var participating = new List<TectonicPlate>();

        foreach (var plate in _plates)
        {
            if (!plate.CellIndices.Contains(sourceCell))
                continue;

            participating.Add(plate);
            foreach (var index in plate.CellIndices)
                displacement[index] += slipM * coupling;
        }

        var relativeSpeed = 0d;
        if (participating.Count > 1)
        {
            for (var i = 1; i < participating.Count; i++)
            {
                var a = participating[i - 1];
                var b = participating[i];
                relativeSpeed += Math.Sqrt(
                    Math.Pow(a.VelocityXMPerYear - b.VelocityXMPerYear, 2) +
                    Math.Pow(a.VelocityYMPerYear - b.VelocityYMPerYear, 2));
            }
        }

        var strainRate = relativeSpeed / Math.Max(1, participating.Count) / 100_000d;
        var magnitude = Math.Log10(Math.Max(1, slipM * 1_000_000d)) + strainRate * timeStepYears;
        var energy = Math.Pow(10, Math.Max(0, magnitude) + 4.8);
        var maximum = 0d;
        foreach (var value in displacement)
            maximum = Math.Max(maximum, Math.Abs(value));

        return new TectonicEvent(++_eventId, magnitude, energy, maximum, displacement);
    }
}
