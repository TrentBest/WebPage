using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Chemistry;

/// <summary>
/// Small canonical catalog used by the Chemistry Lab foundation. The data
/// boundary is intentionally replaceable by a warehouse-backed provider later.
/// </summary>
public static class ElementalCatalog
{
    public static IReadOnlyDictionary<string, Atom> Core { get; } = new Dictionary<string, Atom>
    {
        ["H"] = new AtomBuilder("Hydrogen", 1, "H").WithAtomicWeight(1.008).WithElectronConfiguration(["1s1"]).WithNeutrons(0).WithOxidationStates("-1,+1").WithStateAtStp("Gas").WithCategory("Nonmetal").Build(),
        ["C"] = new AtomBuilder("Carbon", 6, "C").WithAtomicWeight(12.011).WithElectronConfiguration(["1s2","2s2","2p2"]).WithNeutrons(6).WithOxidationStates("-4,+2,+4").WithStateAtStp("Solid").WithCategory("Nonmetal").Build(),
        ["O"] = new AtomBuilder("Oxygen", 8, "O").WithAtomicWeight(15.999).WithElectronConfiguration(["1s2","2s2","2p4"]).WithNeutrons(8).WithOxidationStates("-2,-1").WithStateAtStp("Gas").WithCategory("Nonmetal").Build(),
        ["Fe"] = new AtomBuilder("Iron", 26, "Fe").WithAtomicWeight(55.845).WithElectronConfiguration(["[Ar]","3d6","4s2"]).WithNeutrons(30).WithOxidationStates("+2,+3").WithStateAtStp("Solid").WithCategory("Transition Metal").Build(),
        ["Si"] = new AtomBuilder("Silicon", 14, "Si").WithAtomicWeight(28.085).WithElectronConfiguration(["[Ne]","3s2","3p2"]).WithNeutrons(14).WithOxidationStates("-4,+2,+4").WithStateAtStp("Solid").WithCategory("Metalloid").Build(),
        ["Al"] = new AtomBuilder("Aluminium", 13, "Al").WithAtomicWeight(26.982).WithElectronConfiguration(["[Ne]","3s2","3p1"]).WithNeutrons(14).WithOxidationStates("+3").WithStateAtStp("Solid").WithCategory("Post-transition Metal").Build(),
        ["Cu"] = new AtomBuilder("Copper", 29, "Cu").WithAtomicWeight(63.546).WithElectronConfiguration(["[Ar]","3d10","4s1"]).WithNeutrons(35).WithOxidationStates("+1,+2").WithStateAtStp("Solid").WithCategory("Transition Metal").Build()
    };
}
