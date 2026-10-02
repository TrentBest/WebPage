using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Chemistry;

/// <summary>
/// Canonical elemental identity catalog.
/// The catalog is a data boundary: it contains all 118 currently named
/// chemical elements. Richer physical/material properties can be supplied by
/// a warehouse-backed provider later without changing chemistry contracts or
/// presentation code.
/// </summary>
public static class ElementalCatalog
{
    private static readonly IReadOnlyDictionary<string, Atom> _core =
        new Dictionary<string, Atom>(System.StringComparer.Ordinal)
        {
            ["H"] = new AtomBuilder("Hydrogen", 1, "H").WithAtomicWeight(1.008).Build(),
            ["He"] = new AtomBuilder("Helium", 2, "He").WithAtomicWeight(4.0026).Build(),
            ["Li"] = new AtomBuilder("Lithium", 3, "Li").WithAtomicWeight(6.94).Build(),
            ["Be"] = new AtomBuilder("Beryllium", 4, "Be").WithAtomicWeight(9.0122).Build(),
            ["B"] = new AtomBuilder("Boron", 5, "B").WithAtomicWeight(10.81).Build(),
            ["C"] = new AtomBuilder("Carbon", 6, "C").WithAtomicWeight(12.011).WithElectronConfiguration(["1s2", "2s2", "2p2"]).WithNeutrons(6).Build(),
            ["N"] = new AtomBuilder("Nitrogen", 7, "N").WithAtomicWeight(14.007).Build(),
            ["O"] = new AtomBuilder("Oxygen", 8, "O").WithAtomicWeight(15.999).Build(),
            ["F"] = new AtomBuilder("Fluorine", 9, "F").WithAtomicWeight(18.998).Build(),
            ["Ne"] = new AtomBuilder("Neon", 10, "Ne").WithAtomicWeight(20.180).Build(),
            ["Na"] = new AtomBuilder("Sodium", 11, "Na").WithAtomicWeight(22.990).Build(),
            ["Mg"] = new AtomBuilder("Magnesium", 12, "Mg").WithAtomicWeight(24.305).Build(),
            ["Al"] = new AtomBuilder("Aluminum", 13, "Al").WithAtomicWeight(26.982).Build(),
            ["Si"] = new AtomBuilder("Silicon", 14, "Si").WithAtomicWeight(28.085).Build(),
            ["P"] = new AtomBuilder("Phosphorus", 15, "P").WithAtomicWeight(30.974).Build(),
            ["S"] = new AtomBuilder("Sulfur", 16, "S").WithAtomicWeight(32.06).Build(),
            ["Cl"] = new AtomBuilder("Chlorine", 17, "Cl").WithAtomicWeight(35.45).Build(),
            ["Ar"] = new AtomBuilder("Argon", 18, "Ar").WithAtomicWeight(39.948).Build(),
            ["K"] = new AtomBuilder("Potassium", 19, "K").WithAtomicWeight(39.098).Build(),
            ["Ca"] = new AtomBuilder("Calcium", 20, "Ca").WithAtomicWeight(40.078).Build(),
            ["Sc"] = new AtomBuilder("Scandium", 21, "Sc").WithAtomicWeight(44.956).Build(),
            ["Ti"] = new AtomBuilder("Titanium", 22, "Ti").WithAtomicWeight(47.867).Build(),
            ["V"] = new AtomBuilder("Vanadium", 23, "V").WithAtomicWeight(50.942).Build(),
            ["Cr"] = new AtomBuilder("Chromium", 24, "Cr").WithAtomicWeight(51.996).Build(),
            ["Mn"] = new AtomBuilder("Manganese", 25, "Mn").WithAtomicWeight(54.938).Build(),
            ["Fe"] = new AtomBuilder("Iron", 26, "Fe").WithAtomicWeight(55.845).Build(),
            ["Co"] = new AtomBuilder("Cobalt", 27, "Co").WithAtomicWeight(58.933).Build(),
            ["Ni"] = new AtomBuilder("Nickel", 28, "Ni").WithAtomicWeight(58.693).Build(),
            ["Cu"] = new AtomBuilder("Copper", 29, "Cu").WithAtomicWeight(63.546).Build(),
            ["Zn"] = new AtomBuilder("Zinc", 30, "Zn").WithAtomicWeight(65.38).Build(),
            ["Ga"] = new AtomBuilder("Gallium", 31, "Ga").WithAtomicWeight(69.723).Build(),
            ["Ge"] = new AtomBuilder("Germanium", 32, "Ge").WithAtomicWeight(72.63).Build(),
            ["As"] = new AtomBuilder("Arsenic", 33, "As").WithAtomicWeight(74.922).Build(),
            ["Se"] = new AtomBuilder("Selenium", 34, "Se").WithAtomicWeight(78.971).Build(),
            ["Br"] = new AtomBuilder("Bromine", 35, "Br").WithAtomicWeight(79.904).Build(),
            ["Kr"] = new AtomBuilder("Krypton", 36, "Kr").WithAtomicWeight(83.798).Build(),
            ["Rb"] = new AtomBuilder("Rubidium", 37, "Rb").WithAtomicWeight(85.468).Build(),
            ["Sr"] = new AtomBuilder("Strontium", 38, "Sr").WithAtomicWeight(87.62).Build(),
            ["Y"] = new AtomBuilder("Yttrium", 39, "Y").WithAtomicWeight(88.906).Build(),
            ["Zr"] = new AtomBuilder("Zirconium", 40, "Zr").WithAtomicWeight(91.224).Build(),
            ["Nb"] = new AtomBuilder("Niobium", 41, "Nb").WithAtomicWeight(92.906).Build(),
            ["Mo"] = new AtomBuilder("Molybdenum", 42, "Mo").WithAtomicWeight(95.95).Build(),
            ["Tc"] = new AtomBuilder("Technetium", 43, "Tc").WithAtomicWeight(98).Build(),
            ["Ru"] = new AtomBuilder("Ruthenium", 44, "Ru").WithAtomicWeight(101.07).Build(),
            ["Rh"] = new AtomBuilder("Rhodium", 45, "Rh").WithAtomicWeight(102.91).Build(),
            ["Pd"] = new AtomBuilder("Palladium", 46, "Pd").WithAtomicWeight(106.42).Build(),
            ["Ag"] = new AtomBuilder("Silver", 47, "Ag").WithAtomicWeight(107.87).Build(),
            ["Cd"] = new AtomBuilder("Cadmium", 48, "Cd").WithAtomicWeight(112.41).Build(),
            ["In"] = new AtomBuilder("Indium", 49, "In").WithAtomicWeight(114.82).Build(),
            ["Sn"] = new AtomBuilder("Tin", 50, "Sn").WithAtomicWeight(118.71).Build(),
            ["Sb"] = new AtomBuilder("Antimony", 51, "Sb").WithAtomicWeight(121.76).Build(),
            ["Te"] = new AtomBuilder("Tellurium", 52, "Te").WithAtomicWeight(127.6).Build(),
            ["I"] = new AtomBuilder("Iodine", 53, "I").WithAtomicWeight(126.9).Build(),
            ["Xe"] = new AtomBuilder("Xenon", 54, "Xe").WithAtomicWeight(131.29).Build(),
            ["Cs"] = new AtomBuilder("Cesium", 55, "Cs").WithAtomicWeight(132.91).Build(),
            ["Ba"] = new AtomBuilder("Barium", 56, "Ba").WithAtomicWeight(137.33).Build(),
            ["La"] = new AtomBuilder("Lanthanum", 57, "La").WithAtomicWeight(138.91).Build(),
            ["Ce"] = new AtomBuilder("Cerium", 58, "Ce").WithAtomicWeight(140.12).Build(),
            ["Pr"] = new AtomBuilder("Praseodymium", 59, "Pr").WithAtomicWeight(140.91).Build(),
            ["Nd"] = new AtomBuilder("Neodymium", 60, "Nd").WithAtomicWeight(144.24).Build(),
            ["Pm"] = new AtomBuilder("Promethium", 61, "Pm").WithAtomicWeight(145).Build(),
            ["Sm"] = new AtomBuilder("Samarium", 62, "Sm").WithAtomicWeight(150.36).Build(),
            ["Eu"] = new AtomBuilder("Europium", 63, "Eu").WithAtomicWeight(151.96).Build(),
            ["Gd"] = new AtomBuilder("Gadolinium", 64, "Gd").WithAtomicWeight(157.25).Build(),
            ["Tb"] = new AtomBuilder("Terbium", 65, "Tb").WithAtomicWeight(158.93).Build(),
            ["Dy"] = new AtomBuilder("Dysprosium", 66, "Dy").WithAtomicWeight(162.5).Build(),
            ["Ho"] = new AtomBuilder("Holmium", 67, "Ho").WithAtomicWeight(164.93).Build(),
            ["Er"] = new AtomBuilder("Erbium", 68, "Er").WithAtomicWeight(167.26).Build(),
            ["Tm"] = new AtomBuilder("Thulium", 69, "Tm").WithAtomicWeight(168.93).Build(),
            ["Yb"] = new AtomBuilder("Ytterbium", 70, "Yb").WithAtomicWeight(173.05).Build(),
            ["Lu"] = new AtomBuilder("Lutetium", 71, "Lu").WithAtomicWeight(174.97).Build(),
            ["Hf"] = new AtomBuilder("Hafnium", 72, "Hf").WithAtomicWeight(178.49).Build(),
            ["Ta"] = new AtomBuilder("Tantalum", 73, "Ta").WithAtomicWeight(180.95).Build(),
            ["W"] = new AtomBuilder("Tungsten", 74, "W").WithAtomicWeight(183.84).Build(),
            ["Re"] = new AtomBuilder("Rhenium", 75, "Re").WithAtomicWeight(186.21).Build(),
            ["Os"] = new AtomBuilder("Osmium", 76, "Os").WithAtomicWeight(190.23).Build(),
            ["Ir"] = new AtomBuilder("Iridium", 77, "Ir").WithAtomicWeight(192.22).Build(),
            ["Pt"] = new AtomBuilder("Platinum", 78, "Pt").WithAtomicWeight(195.08).Build(),
            ["Au"] = new AtomBuilder("Gold", 79, "Au").WithAtomicWeight(196.97).Build(),
            ["Hg"] = new AtomBuilder("Mercury", 80, "Hg").WithAtomicWeight(200.59).Build(),
            ["Tl"] = new AtomBuilder("Thallium", 81, "Tl").WithAtomicWeight(204.38).Build(),
            ["Pb"] = new AtomBuilder("Lead", 82, "Pb").WithAtomicWeight(207.2).Build(),
            ["Bi"] = new AtomBuilder("Bismuth", 83, "Bi").WithAtomicWeight(208.98).Build(),
            ["Po"] = new AtomBuilder("Polonium", 84, "Po").WithAtomicWeight(209).Build(),
            ["At"] = new AtomBuilder("Astatine", 85, "At").WithAtomicWeight(210).Build(),
            ["Rn"] = new AtomBuilder("Radon", 86, "Rn").WithAtomicWeight(222).Build(),
            ["Fr"] = new AtomBuilder("Francium", 87, "Fr").WithAtomicWeight(223).Build(),
            ["Ra"] = new AtomBuilder("Radium", 88, "Ra").WithAtomicWeight(226).Build(),
            ["Ac"] = new AtomBuilder("Actinium", 89, "Ac").WithAtomicWeight(227).Build(),
            ["Th"] = new AtomBuilder("Thorium", 90, "Th").WithAtomicWeight(232.04).Build(),
            ["Pa"] = new AtomBuilder("Protactinium", 91, "Pa").WithAtomicWeight(231.04).Build(),
            ["U"] = new AtomBuilder("Uranium", 92, "U").WithAtomicWeight(238.03).Build(),
            ["Np"] = new AtomBuilder("Neptunium", 93, "Np").WithAtomicWeight(237).Build(),
            ["Pu"] = new AtomBuilder("Plutonium", 94, "Pu").WithAtomicWeight(244).Build(),
            ["Am"] = new AtomBuilder("Americium", 95, "Am").WithAtomicWeight(243).Build(),
            ["Cm"] = new AtomBuilder("Curium", 96, "Cm").WithAtomicWeight(247).Build(),
            ["Bk"] = new AtomBuilder("Berkelium", 97, "Bk").WithAtomicWeight(247).Build(),
            ["Cf"] = new AtomBuilder("Californium", 98, "Cf").WithAtomicWeight(251).Build(),
            ["Es"] = new AtomBuilder("Einsteinium", 99, "Es").WithAtomicWeight(252).Build(),
            ["Fm"] = new AtomBuilder("Fermium", 100, "Fm").WithAtomicWeight(257).Build(),
            ["Md"] = new AtomBuilder("Mendelevium", 101, "Md").WithAtomicWeight(258).Build(),
            ["No"] = new AtomBuilder("Nobelium", 102, "No").WithAtomicWeight(259).Build(),
            ["Lr"] = new AtomBuilder("Lawrencium", 103, "Lr").WithAtomicWeight(262).Build(),
            ["Rf"] = new AtomBuilder("Rutherfordium", 104, "Rf").WithAtomicWeight(267).Build(),
            ["Db"] = new AtomBuilder("Dubnium", 105, "Db").WithAtomicWeight(270).Build(),
            ["Sg"] = new AtomBuilder("Seaborgium", 106, "Sg").WithAtomicWeight(271).Build(),
            ["Bh"] = new AtomBuilder("Bohrium", 107, "Bh").WithAtomicWeight(270).Build(),
            ["Hs"] = new AtomBuilder("Hassium", 108, "Hs").WithAtomicWeight(277).Build(),
            ["Mt"] = new AtomBuilder("Meitnerium", 109, "Mt").WithAtomicWeight(278).Build(),
            ["Ds"] = new AtomBuilder("Darmstadtium", 110, "Ds").WithAtomicWeight(281).Build(),
            ["Rg"] = new AtomBuilder("Roentgenium", 111, "Rg").WithAtomicWeight(282).Build(),
            ["Cn"] = new AtomBuilder("Copernicium", 112, "Cn").WithAtomicWeight(285).Build(),
            ["Nh"] = new AtomBuilder("Nihonium", 113, "Nh").WithAtomicWeight(286).Build(),
            ["Fl"] = new AtomBuilder("Flerovium", 114, "Fl").WithAtomicWeight(289).Build(),
            ["Mc"] = new AtomBuilder("Moscovium", 115, "Mc").WithAtomicWeight(290).Build(),
            ["Lv"] = new AtomBuilder("Livermorium", 116, "Lv").WithAtomicWeight(293).Build(),
            ["Ts"] = new AtomBuilder("Tennessine", 117, "Ts").WithAtomicWeight(294).Build(),
            ["Og"] = new AtomBuilder("Oganesson", 118, "Og").WithAtomicWeight(294).Build(),
        };

    private static readonly IReadOnlyDictionary<string, Atom> _fictional =
        new Dictionary<string, Atom>(System.StringComparer.Ordinal)
        {
            // Hypothetical identity only; fictional properties are supplied separately.
            ["Ub"] = new AtomBuilder("Unobtanium", 120, "Ub")
                .WithAtomicWeight(120)
                .WithOrigin(ElementOrigin.Fictional)
                .WithCategory("Fictional")
                .Build(),
        };

    private static readonly IReadOnlyDictionary<int, Atom> _byAtomicNumber =
        _core.Values.ToDictionary(x => x.AtomicNumber);

    public static IReadOnlyDictionary<string, Atom> Core => _core;
    public static IReadOnlyDictionary<int, Atom> ByAtomicNumber => _byAtomicNumber;
    public static IReadOnlyDictionary<string, Atom> Fictional => _fictional;
    public static int FictionalCount => _fictional.Count;

    public static int Count => _core.Count;
    public static Atom GetBySymbol(string symbol) => _core[symbol];
    public static Atom GetByAtomicNumber(int atomicNumber) => _byAtomicNumber[atomicNumber];
}
