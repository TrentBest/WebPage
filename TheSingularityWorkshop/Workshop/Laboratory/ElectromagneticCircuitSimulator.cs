using System;

namespace TheSingularityWorkshop.Workshop.Laboratory;

public readonly record struct CircuitState(
    double VoltageVolts,
    double ResistanceOhms,
    double CurrentAmps);

public readonly record struct CircuitMeasurement(
    CircuitState State,
    double PowerWatts,
    double EnergyJoules);

/// <summary>
/// Deterministic Ohmic circuit bench. Voltage and resistance are inputs; current,
/// power, and accumulated energy are measurements derived from the model.
/// </summary>
public sealed class ElectromagneticCircuitSimulator
{
    public CircuitState State { get; private set; }

    public ElectromagneticCircuitSimulator(double voltageVolts, double resistanceOhms)
    {
        if (voltageVolts < 0) throw new ArgumentOutOfRangeException(nameof(voltageVolts));
        if (resistanceOhms <= 0) throw new ArgumentOutOfRangeException(nameof(resistanceOhms));
        State = new CircuitState(voltageVolts, resistanceOhms, voltageVolts / resistanceOhms);
    }

    public CircuitMeasurement Measure(double durationSeconds)
    {
        if (durationSeconds < 0) throw new ArgumentOutOfRangeException(nameof(durationSeconds));
        var power = State.VoltageVolts * State.CurrentAmps;
        return new CircuitMeasurement(State, power, power * durationSeconds);
    }
}
