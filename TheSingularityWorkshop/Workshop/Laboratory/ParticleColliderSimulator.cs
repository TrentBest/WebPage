using System;

namespace TheSingularityWorkshop.Workshop.Laboratory;

public readonly record struct ColliderParticle(
    string Id,
    double MassKg,
    double ChargeCoulombs,
    double EnergyJoules);

public readonly record struct ColliderCollisionResult(
    long EventId,
    double CenterOfMassEnergyJoules,
    double TotalInputEnergyJoules,
    double EnergyResidualJoules,
    bool ConservesEnergy);

/// <summary>
/// Deterministic particle-collision model for laboratory experiments.
/// This is a conservation-law simulator, not a detector or quantum-field solver.
/// </summary>
public sealed class ParticleColliderSimulator
{
    private long _eventId;

    public ColliderParticle? First { get; private set; }
    public ColliderParticle? Second { get; private set; }
    public ColliderCollisionResult? LastCollision { get; private set; }

    public void Configure(ColliderParticle first, ColliderParticle second)
    {
        if (first.MassKg <= 0 || second.MassKg <= 0)
            throw new ArgumentOutOfRangeException(nameof(first), "Particle masses must be positive.");
        if (first.EnergyJoules < 0 || second.EnergyJoules < 0)
            throw new ArgumentOutOfRangeException(nameof(first), "Particle energies cannot be negative.");

        First = first;
        Second = second;
        LastCollision = null;
    }

    public ColliderCollisionResult Collide(double centerOfMassEnergyJoules)
    {
        if (First is null || Second is null)
            throw new InvalidOperationException("Configure both particles before collision.");
        if (centerOfMassEnergyJoules < 0)
            throw new ArgumentOutOfRangeException(nameof(centerOfMassEnergyJoules));

        var total = First.Value.EnergyJoules + Second.Value.EnergyJoules;
        var residual = total - centerOfMassEnergyJoules;
        var result = new ColliderCollisionResult(
            ++_eventId,
            centerOfMassEnergyJoules,
            total,
            residual,
            Math.Abs(residual) <= Math.Max(1e-12, total * 1e-12));

        LastCollision = result;
        return result;
    }
}
