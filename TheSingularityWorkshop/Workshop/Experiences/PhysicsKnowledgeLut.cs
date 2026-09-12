namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>Authoring depth for knowledge carried by a MicroBundle.</summary>
public enum KnowledgeDetailLevel
{
    Essential = 1,
    Intermediate = 2,
    Advanced = 3,
    Expert = 4
}

/// <summary>A single immutable knowledge entry addressed by an integer ontology bundle id.</summary>
public sealed record PhysicsKnowledgeEntry(
    ulong MicroBundleId,
    KnowledgeDetailLevel Level,
    string Summary,
    IReadOnlyList<string> Facts);

/// <summary>
/// Compact lookup tables for physics knowledge. The tables are authored as rich text today;
/// they are intentionally isolated so a later compiler can replace prose equations with an
/// integer-addressed expression/units representation without changing MicroBundle identity.
/// </summary>
public static class PhysicsKnowledgeLut
{
    public static IReadOnlyList<PhysicsKnowledgeEntry> Entries { get; } =
    [
        Entry(0x31000001, KnowledgeDetailLevel.Essential, "Motion describes how position changes with time.",
            "Velocity is the time derivative of position.", "Acceleration is the time derivative of velocity."),
        Entry(0x31000001, KnowledgeDetailLevel.Intermediate, "Kinematics relates position, velocity, acceleration, and time.",
            "For constant acceleration, v = v0 + a t.", "For constant acceleration, x = x0 + v0 t + 1/2 a t^2.", "For constant acceleration, v^2 = v0^2 + 2 a (x - x0)."),
        Entry(0x31000001, KnowledgeDetailLevel.Advanced, "Vector kinematics separates independent spatial components.",
            "Acceleration is a vector and may change speed, direction, or both.", "Projectile motion is commonly decomposed into horizontal and vertical components when air resistance is neglected."),
        Entry(0x31000001, KnowledgeDetailLevel.Expert, "Kinematics can be generalized with coordinate transformations and constrained motion.",
            "Generalized coordinates permit configuration spaces that are not Cartesian.", "Constraints can be represented explicitly or eliminated through coordinate choice."),

        Entry(0x31000002, KnowledgeDetailLevel.Essential, "Forces change momentum.",
            "Newton's second law relates net force to the time rate of change of momentum.", "For constant mass, F = m a."),
        Entry(0x31000002, KnowledgeDetailLevel.Intermediate, "Free-body analysis identifies forces acting on a body.",
            "Weight near Earth's surface is approximately m g.", "Normal, friction, tension, drag, and applied forces are modeled separately before summation."),
        Entry(0x31000002, KnowledgeDetailLevel.Advanced, "Force models define differential equations of motion.",
            "Linear drag produces a velocity-dependent force.", "Nonlinear force laws can produce dynamics that require numerical integration."),
        Entry(0x31000002, KnowledgeDetailLevel.Expert, "Force descriptions can be replaced by variational or field formulations.",
            "Lagrangian mechanics derives equations from stationary action.", "Hamiltonian mechanics expresses dynamics in phase space."),

        Entry(0x31000005, KnowledgeDetailLevel.Essential, "Gravity is an attractive interaction between mass-energy.",
            "Near Earth's surface, gravitational acceleration is approximately 9.81 m/s^2.", "Weight depends on local gravitational acceleration."),
        Entry(0x31000005, KnowledgeDetailLevel.Intermediate, "Newtonian gravitation models attraction between masses.",
            "F = G m1 m2 / r^2 for point masses.", "Gravitational potential energy for two point masses is U = -G m1 m2 / r."),
        Entry(0x31000005, KnowledgeDetailLevel.Advanced, "Orbital dynamics follows from gravitational acceleration.",
            "For a circular orbit, orbital speed satisfies v = sqrt(G M / r).", "Escape speed from radius r is sqrt(2 G M / r) in the ideal two-body model."),
        Entry(0x31000005, KnowledgeDetailLevel.Expert, "General relativity replaces Newtonian gravity with spacetime geometry.",
            "Free-fall trajectories are geodesics of spacetime.", "Newtonian gravity is recovered as an appropriate weak-field, low-speed limit."),

        Entry(0x32000001, KnowledgeDetailLevel.Essential, "Electric charge is a conserved physical quantity.",
            "Like charges repel and unlike charges attract.", "Charge is measured in coulombs."),
        Entry(0x32000001, KnowledgeDetailLevel.Intermediate, "Electric current is the rate of charge flow.",
            "I = dQ/dt.", "For a steady current, charge transported over time satisfies Q = I t."),
        Entry(0x32000001, KnowledgeDetailLevel.Advanced, "Charge distributions generate electric fields and potentials.",
            "Gauss's law relates electric flux through a closed surface to enclosed charge.", "Electric potential is related to electric field by E = -grad(V)."),
        Entry(0x32000001, KnowledgeDetailLevel.Expert, "Electromagnetic systems are described by coupled fields.",
            "Maxwell's equations connect charge, current, electric fields, and magnetic fields.", "Electromagnetic waves propagate in vacuum at c in the classical theory."),

        Entry(0x32000003, KnowledgeDetailLevel.Essential, "Circuits provide controlled paths for electric current.",
            "Ohm's law for an ideal resistor is V = I R.", "Electrical power is P = V I."),
        Entry(0x32000003, KnowledgeDetailLevel.Intermediate, "Kirchhoff's laws express conservation constraints in lumped circuits.",
            "The algebraic sum of currents at an ideal node is zero.", "The algebraic sum of voltages around an ideal closed loop is zero."),
        Entry(0x32000003, KnowledgeDetailLevel.Advanced, "Reactive components make circuit behavior frequency dependent.",
            "An ideal capacitor obeys I = C dV/dt.", "An ideal inductor obeys V = L dI/dt."),
        Entry(0x32000003, KnowledgeDetailLevel.Expert, "Distributed electromagnetic effects eventually invalidate simple lumped-element assumptions.",
            "Transmission lines require spatially distributed voltage and current models.", "Characteristic impedance and propagation delay become first-class circuit properties."),

        Entry(0x33000001, KnowledgeDetailLevel.Essential, "Temperature characterizes the thermal state of matter.",
            "Temperature is related to the average energy distribution of microscopic degrees of freedom.", "Temperature is measured on absolute and relative scales."),
        Entry(0x33000001, KnowledgeDetailLevel.Intermediate, "Temperature is not simply the kinetic energy of every molecule.",
            "Thermal energy is distributed statistically across accessible microscopic states.", "Equipartition provides useful classical limits but is not universally valid."),
        Entry(0x33000001, KnowledgeDetailLevel.Advanced, "Statistical mechanics connects macroscopic temperature to microscopic state populations.",
            "Boltzmann factors weight states according to energy and temperature.", "Quantum statistics become important when classical assumptions fail."),
        Entry(0x33000001, KnowledgeDetailLevel.Expert, "Thermal descriptions depend on ensembles, equations of state, and microscopic constraints.",
            "Temperature can be defined through entropy derivatives in equilibrium thermodynamics.", "Non-equilibrium systems require transport and kinetic descriptions beyond equilibrium state variables."),

        Entry(0x33000002, KnowledgeDetailLevel.Essential, "Heat is energy transferred because of a temperature difference.",
            "Heat is not a substance stored independently of a system.", "Energy transfer can occur by conduction, convection, or radiation."),
        Entry(0x33000002, KnowledgeDetailLevel.Intermediate, "The first law tracks energy conservation.",
            "For a closed system, delta U = Q - W when W is work done by the system.", "Specific heat relates temperature change to energy input over a specified regime."),
        Entry(0x33000002, KnowledgeDetailLevel.Advanced, "Heat transfer rates depend on transport mechanisms and boundary conditions.",
            "Fourier's law relates conductive heat flux to temperature gradient.", "Newton's law of cooling is a useful convective approximation in suitable regimes."),
        Entry(0x33000002, KnowledgeDetailLevel.Expert, "Thermal transport can become strongly coupled to phase, flow, and radiation.",
            "Boiling regimes can transition from nucleate boiling toward film boiling as surface conditions change.", "Critical heat flux is a regime boundary rather than a universal material constant."),

        Entry(0x33000003, KnowledgeDetailLevel.Essential, "Entropy measures a thermodynamic property associated with state and energy distribution.",
            "For a reversible process, dS = dQ_rev/T.", "The second law constrains the direction of spontaneous processes."),
        Entry(0x33000003, KnowledgeDetailLevel.Intermediate, "Entropy increases for an isolated system in irreversible evolution.",
            "Entropy generation is associated with irreversibility.", "Mixing, friction, heat transfer across finite temperature differences, and diffusion can generate entropy."),
        Entry(0x33000003, KnowledgeDetailLevel.Advanced, "Statistical mechanics connects entropy to microscopic multiplicity.",
            "Boltzmann entropy is S = k ln(Omega) for a suitable microstate count Omega.", "Thermodynamic entropy is an emergent state function compatible with statistical descriptions."),
        Entry(0x33000003, KnowledgeDetailLevel.Expert, "Entropy provides a bridge between thermodynamics, information, and statistical inference.",
            "The operational interpretation depends on the physical ensemble and coarse-graining used.", "Information-theoretic analogies must preserve the distinction between mathematical entropy and thermodynamic entropy."),

        Entry(0x34000001, KnowledgeDetailLevel.Essential, "Pressure describes normal force per unit area in a continuum.",
            "Pressure is a scalar field in a fluid at local thermodynamic equilibrium.", "Pressure is measured in pascals."),
        Entry(0x34000001, KnowledgeDetailLevel.Intermediate, "Static fluids exhibit pressure variation under body forces.",
            "For a stationary incompressible fluid in a uniform gravitational field, pressure changes approximately linearly with depth.", "Absolute and gauge pressure must be distinguished."),
        Entry(0x34000001, KnowledgeDetailLevel.Advanced, "Pressure participates in the momentum equations of fluid mechanics.",
            "Pressure gradients can accelerate fluid motion.", "In viscous flow, pressure gradients interact with viscous stresses and boundary conditions."),
        Entry(0x34000001, KnowledgeDetailLevel.Expert, "Pressure may be coupled to compressibility, temperature, and phase.",
            "An equation of state can relate pressure to density and temperature.", "At high pressures or near phase transitions, ideal-gas assumptions may become inadequate."),

        Entry(0x34000005, KnowledgeDetailLevel.Essential, "Buoyancy is the net force produced by pressure variation around an immersed body.",
            "For a displaced-fluid volume, ideal hydrostatic buoyancy equals the weight of displaced fluid.", "Buoyancy can oppose or augment other forces depending on density and configuration."),
        Entry(0x34000005, KnowledgeDetailLevel.Intermediate, "Archimedes' principle follows from integrating hydrostatic pressure over a surface.",
            "A body can float when its displaced-fluid weight balances its effective weight.", "The center of buoyancy is associated with the centroid of displaced volume in simple hydrostatic cases."),
        Entry(0x34000005, KnowledgeDetailLevel.Advanced, "Buoyancy interacts with stratification and fluid motion.",
            "Density gradients can drive natural convection.", "Stable and unstable stratification depend on how density changes with height."),
        Entry(0x34000005, KnowledgeDetailLevel.Expert, "Buoyant flow couples mass, momentum, and thermal or compositional transport.",
            "Boussinesq approximations retain buoyancy effects while simplifying density variation in selected regimes.", "Dimensionless groups such as Rayleigh and Prandtl numbers help classify convective regimes."),

        Entry(0x35000005, KnowledgeDetailLevel.Essential, "Magnetohydrodynamics couples electrically conducting fluid motion to magnetic fields.",
            "Plasma and some liquid metals can be treated as conducting continua under appropriate assumptions.", "Fluid velocity and magnetic field interact through electromagnetic induction."),
        Entry(0x35000005, KnowledgeDetailLevel.Intermediate, "MHD adds magnetic forces and induction to fluid dynamics.",
            "The Lorentz force couples current density and magnetic field.", "Magnetic diffusion competes with field advection in conducting fluids."),
        Entry(0x35000005, KnowledgeDetailLevel.Advanced, "Dimensionless magnetic Reynolds number compares advection and diffusion of magnetic fields.",
            "High magnetic Reynolds number regimes can exhibit strong field-line advection.", "Resistivity, conductivity, geometry, and boundary conditions determine MHD behavior."),
        Entry(0x35000005, KnowledgeDetailLevel.Expert, "MHD is a reduced model of a larger kinetic electromagnetic system.",
            "Its validity depends on scale separation, collisionality, quasi-neutrality, and other closure assumptions.", "More detailed plasma descriptions may require multi-fluid or kinetic equations."),

        Entry(0x35000004, KnowledgeDetailLevel.Essential, "Debye shielding describes electrostatic screening in a plasma.",
            "A charged perturbation can be screened by rearrangement of mobile charged particles.", "The Debye length sets a characteristic screening scale."),
        Entry(0x35000004, KnowledgeDetailLevel.Intermediate, "Debye shielding depends on particle density and temperature.",
            "Higher temperature generally increases the Debye length in the classical plasma approximation.", "Higher density generally decreases the Debye length in that approximation."),
        Entry(0x35000004, KnowledgeDetailLevel.Advanced, "Debye shielding is a collective effect rather than a single-particle collision.",
            "The concept assumes a sufficiently populated Debye sphere for fluid or statistical plasma descriptions.", "Plasma parameter and coupling strength determine whether weakly coupled assumptions are appropriate."),
        Entry(0x35000004, KnowledgeDetailLevel.Expert, "Kinetic plasma theory replaces simple shielding formulas when distribution functions and wave-particle interactions matter.",
            "Linearized kinetic equations can predict dynamic screening and dielectric response.", "Static Debye shielding is therefore a limiting description of a broader collective response."),

        Entry(0x36000001, KnowledgeDetailLevel.Essential, "Nuclear binding arises from interactions within atomic nuclei.",
            "Nuclear systems contain protons and neutrons bound by the residual strong interaction.", "Binding energy is the energy required to separate a nucleus into its constituent nucleons."),
        Entry(0x36000001, KnowledgeDetailLevel.Intermediate, "Nuclear stability depends on neutron-proton composition and nuclear structure.",
            "Binding energy per nucleon varies across nuclides.", "Radioactive decay changes a nucleus toward configurations with different energies and stability."),
        Entry(0x36000001, KnowledgeDetailLevel.Advanced, "Nuclear reactions conserve charge, nucleon number in appropriate reaction descriptions, energy, and momentum.",
            "Mass differences can appear as reaction Q-values.", "Cross sections describe reaction probabilities under specified incident and target conditions."),
        Entry(0x36000001, KnowledgeDetailLevel.Expert, "Nuclear behavior requires models spanning quantum many-body structure and reaction dynamics.",
            "Mean-field, shell, collective, and reaction models describe different regimes and observables.", "A single simplified nuclear model should not be treated as universally valid."),

        Entry(0x36000002, KnowledgeDetailLevel.Essential, "Neutron interactions can sustain a chain reaction in fissile material.",
            "Fission can release energy and additional neutrons.", "The neutron population depends on production, absorption, leakage, and geometry."),
        Entry(0x36000002, KnowledgeDetailLevel.Intermediate, "Reactor criticality describes whether neutron population is steady, increasing, or decreasing.",
            "The effective multiplication factor k-effective is the ratio of successive neutron generations.", "k-effective near one corresponds to a steady idealized chain-reaction population."),
        Entry(0x36000002, KnowledgeDetailLevel.Advanced, "Control materials alter neutron economy through absorption and spectral effects.",
            "Boron isotopes can strongly absorb neutrons and are used in several reactor control strategies.", "Geometry, moderator conditions, fuel composition, temperature, and poisons all affect reactivity."),
        Entry(0x36000002, KnowledgeDetailLevel.Expert, "Reactor physics couples neutron transport, material composition, thermal feedback, and time-dependent kinetics.",
            "Point-kinetics models compress spatial neutron behavior into lumped parameters.", "Detailed transport and depletion calculations retain spatial, energy, and nuclide dependencies."),

        Entry(0x36000003, KnowledgeDetailLevel.Essential, "Nuclear decay transforms unstable nuclei and can emit radiation.",
            "Alpha, beta, and gamma emissions represent different physical processes.", "Decay rates are commonly modeled statistically."),
        Entry(0x36000003, KnowledgeDetailLevel.Intermediate, "Radioactive decay follows exponential population behavior for a single decay constant.",
            "N(t) = N0 exp(-lambda t).", "Half-life satisfies T1/2 = ln(2)/lambda."),
        Entry(0x36000003, KnowledgeDetailLevel.Advanced, "Decay chains couple multiple nuclides through production and loss terms.",
            "Bateman equations provide analytic solutions for idealized sequential chains.", "Branching ratios distribute parent decays among possible daughter pathways."),
        Entry(0x36000003, KnowledgeDetailLevel.Expert, "Radiation transport depends on particle type, energy, material composition, and geometry.",
            "Attenuation models are regime dependent and should not be reduced to a universal exponential law.", "Energy deposition and biological or material effects require separate response models."),

        Entry(0x36000004, KnowledgeDetailLevel.Essential, "Nuclear thermal systems convert deposited energy into heat and ultimately useful work.",
            "Fission energy is deposited through multiple channels including fragment kinetic energy and subsequent radiation interactions.", "Thermal-hydraulic behavior determines how heat is removed from fuel and structures."),
        Entry(0x36000004, KnowledgeDetailLevel.Intermediate, "Boiling heat transfer has distinct regimes.",
            "Nucleate boiling can provide high heat-transfer coefficients while vapor remains in discrete bubbles.", "Departure from nucleate boiling marks a transition associated with deterioration of heat transfer under relevant conditions."),
        Entry(0x36000004, KnowledgeDetailLevel.Advanced, "Critical heat flux depends on pressure, mass flux, quality, geometry, surface condition, and fluid properties.",
            "The precise transition mechanism differs between pool and forced-flow boiling systems.", "Engineering correlations are generally regime- and geometry-specific."),
        Entry(0x36000004, KnowledgeDetailLevel.Expert, "Reactor thermal limits emerge from coupled neutronic, thermal, hydraulic, material, and control behavior.",
            "Fuel temperature feedback, coolant density feedback, two-phase flow, heat-transfer limits, and structural margins can interact dynamically.", "High-fidelity analysis therefore couples multiple conservation laws and validated constitutive models."),

        Entry(0x36000005, KnowledgeDetailLevel.Essential, "Radiation shielding reduces exposure by interaction of radiation with matter.",
            "Shielding effectiveness depends on radiation type and energy.", "Distance, time, and shielding are fundamental exposure-control concepts."),
        Entry(0x36000005, KnowledgeDetailLevel.Intermediate, "Photon shielding commonly involves attenuation, scattering, and energy deposition.",
            "Lead is effective for many photon energies because of its high density and atomic number.", "Neutron shielding often relies on hydrogen-rich materials followed by capture or other secondary-radiation management."),
        Entry(0x36000005, KnowledgeDetailLevel.Advanced, "A shielding design must account for secondary radiation and geometry.",
            "High-energy photons can produce secondary particles in shielding materials.", "Neutron interactions can generate capture gammas that must be considered separately."),
        Entry(0x36000005, KnowledgeDetailLevel.Expert, "Radiation transport is fundamentally a particle-transport problem.",
            "Monte Carlo, deterministic transport, and specialized attenuation models serve different accuracy and computational regimes.", "Material cross sections, spectra, geometry, buildup, and detector response all influence calculated dose or flux."),

        Entry(0x37000001, KnowledgeDetailLevel.Essential, "Energy is conserved while changing form or location.",
            "Kinetic, potential, thermal, chemical, electrical, and nuclear energies are useful accounting categories.", "Energy is measured in joules in the SI system."),
        Entry(0x37000001, KnowledgeDetailLevel.Intermediate, "Work transfers energy through force acting over displacement.",
            "For a constant force parallel to displacement, W = F d.", "Power is the rate of energy transfer, P = dE/dt."),
        Entry(0x37000001, KnowledgeDetailLevel.Advanced, "Conservation laws can arise from symmetries.",
            "Time-translation symmetry is associated with energy conservation in suitable physical systems.", "Continuum energy equations track local transport, work, and source terms."),
        Entry(0x37000001, KnowledgeDetailLevel.Expert, "Energy bookkeeping becomes a system-design language when coupled to control volumes and constitutive laws.",
            "The first law can be written as integral or differential conservation equations.", "Choosing the control volume determines which transport and boundary terms must be represented explicitly."),

        Entry(0x37000002, KnowledgeDetailLevel.Essential, "Momentum quantifies motion and responds to net force.",
            "Linear momentum is p = m v for a nonrelativistic particle of constant mass.", "Momentum is conserved in isolated systems."),
        Entry(0x37000002, KnowledgeDetailLevel.Intermediate, "Momentum conservation is central to collisions and fluid flow.",
            "Impulse equals the change in momentum.", "Control-volume momentum balances account for momentum flux and external forces."),
        Entry(0x37000002, KnowledgeDetailLevel.Advanced, "Continuum mechanics represents momentum through stress tensors and body forces.",
            "Pressure contributes an isotropic normal stress in simple fluid models.", "Viscous stresses depend on deformation rate through constitutive assumptions."),
        Entry(0x37000002, KnowledgeDetailLevel.Expert, "Momentum equations unify particle, rigid-body, and continuum descriptions through scale-appropriate models.",
            "Navier-Stokes equations are momentum balances supplemented by constitutive and conservation relations.", "Turbulence modeling introduces closure approximations because direct resolution of every scale is usually impractical."),

        Entry(0x38000001, KnowledgeDetailLevel.Essential, "Waves transfer energy and information through oscillatory or propagating disturbances.",
            "Amplitude, wavelength, frequency, phase, and propagation speed characterize many wave systems.", "For a simple periodic wave, v = f lambda."),
        Entry(0x38000001, KnowledgeDetailLevel.Intermediate, "Wave behavior includes reflection, refraction, interference, and diffraction.",
            "Superposition applies to linear wave systems.", "Boundary conditions determine allowed modes in many resonant systems."),
        Entry(0x38000001, KnowledgeDetailLevel.Advanced, "Dispersion occurs when propagation speed depends on frequency or wavelength.",
            "Phase and group velocities need not be equal in dispersive media.", "Fourier representations decompose suitable signals into spectral components."),
        Entry(0x38000001, KnowledgeDetailLevel.Expert, "Wave equations emerge from local conservation laws and constitutive relations.",
            "Electromagnetic, acoustic, elastic, and plasma waves share mathematical structures while retaining different physical closures.", "Linearization around an equilibrium state is a common route to wave-mode analysis."),

        Entry(0x39000001, KnowledgeDetailLevel.Essential, "Dimensionless numbers compare competing physical effects.",
            "They help identify dominant mechanisms before detailed calculation.", "Examples include Reynolds, Prandtl, Mach, and Peclet numbers."),
        Entry(0x39000001, KnowledgeDetailLevel.Intermediate, "Scaling laws reduce the number of independent parameters in a physical problem.",
            "Dimensional analysis can expose missing variables or inconsistent equations.", "Buckingham Pi theory constructs dimensionless parameter groups."),
        Entry(0x39000001, KnowledgeDetailLevel.Advanced, "Similarity permits experiments or simulations to represent systems at different scales.",
            "Dynamic similarity requires matching the dimensionless groups that control the regime of interest.", "Not every dimensionless group must be matched if asymptotic analysis establishes a negligible contribution."),
        Entry(0x39000001, KnowledgeDetailLevel.Expert, "Dimensionless structure is a bridge between equations, experiment design, simulation, and regime classification.",
            "Asymptotic limits can reduce governing equations systematically.", "The resulting reduced models should retain explicit assumptions about neglected terms."),

        Entry(0x39000002, KnowledgeDetailLevel.Essential, "Units make numerical quantities physically interpretable.",
            "The SI system defines coherent base units for physical measurement.", "Dimensional consistency is a basic validation rule for equations."),
        Entry(0x39000002, KnowledgeDetailLevel.Intermediate, "Unit conversion changes representation without changing the physical quantity.",
            "Meters, kilograms, seconds, amperes, kelvin, moles, and candela form the SI base-unit foundation.", "Derived units such as joule, watt, pascal, and volt are combinations of base units."),
        Entry(0x39000002, KnowledgeDetailLevel.Advanced, "Unit-aware mathematics can prevent classes of numerical and modeling errors.",
            "Dimensional analysis can be performed symbolically before evaluating numerical values.", "Dimensionless normalization can improve numerical conditioning in some simulations."),
        Entry(0x39000002, KnowledgeDetailLevel.Expert, "A future compiled physics representation should separate value, unit dimension, uncertainty, and provenance.",
            "Unit strings are authoring vocabulary, not an ideal runtime identity mechanism.", "Integer unit dimensions and canonical conversion factors are suitable candidates for LUT-backed runtime representation.")
    ];

    private static PhysicsKnowledgeEntry Entry(ulong id, KnowledgeDetailLevel level, string summary, params string[] facts)
        => new(id, level, summary, facts);
}
