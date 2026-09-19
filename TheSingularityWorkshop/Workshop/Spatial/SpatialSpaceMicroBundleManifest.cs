namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Space-domain MicroBundle. It reuses SRPS physics foundations while declaring
/// space-specific propulsion, orbital, environmental, and weapons systems.
/// </summary>
public sealed record SpatialSpaceMicroBundleManifest(
    string Id,
    string Name,
    IReadOnlyList<SpacePhysicsSystem> PhysicsSystems,
    IReadOnlyList<SpaceWeaponsSystem> WeaponsSystems,
    IReadOnlyList<SpacecraftRole> Roles)
{
    public static SpatialSpaceMicroBundleManifest CreateDefault()
        => new(
            "microbundle.space",
            "SPACE",
            [
                new("orbital", "Orbital Mechanics", "Vacuum trajectories, orbital transfers, rendezvous, and station keeping."),
                new("propulsion", "Propulsion", "Thrust, fuel, acceleration, delta-v, and propulsion failure."),
                new("vacuum", "Vacuum Environment", "Pressure, thermal radiation, exposure, and heat rejection."),
                new("microgravity", "Microgravity", "Mass distribution, inertia, docking, and crew movement."),
                new("radiation", "Radiation", "Radiation exposure and shielding."),
                new("thermal", "Thermal Systems", "Heat generation, transfer, storage, and radiator behavior.")
            ],
            [
                new("kinetic", "Kinetic", "Mass-velocity based weapons resolved through SRPS physics."),
                new("directed-energy", "Directed Energy", "Beam and pulse systems with thermal and energy coupling."),
                new("missile", "Missile", "Guided projectile systems with propulsion and intercept geometry."),
                new("defense", "Defensive Systems", "Armor, shielding, interception, maneuver, and damage control.")
            ],
            [
                new("station", "Station", "Permanent orbital facility."),
                new("freighter", "Freighter", "Cargo transport."),
                new("passenger", "Passenger Ship", "Civilian transport."),
                new("yacht", "Personal Yacht", "Private exploration and travel."),
                new("fighter", "Combat Craft", "Small combat spacecraft."),
                new("capital", "Capital Ship", "Large crewed spacecraft.")
            ]);

    public SpacePhysicsSystem? FindPhysics(string id) => PhysicsSystems.FirstOrDefault(x => x.Id == id);
}

public readonly record struct SpacePhysicsSystem(string Id, string Name, string Description);
public readonly record struct SpaceWeaponsSystem(string Id, string Name, string Description);
public readonly record struct SpacecraftRole(string Id, string Name, string Description);
