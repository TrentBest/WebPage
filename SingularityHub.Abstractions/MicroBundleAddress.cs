namespace TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Globally addressable slot for a MicroBundle.
/// The nine-layer ontology identifies the kind of thing; VariantId identifies
/// one of the finite integer-addressable manifestations of that ontology.
/// </summary>
public readonly record struct MicroBundleAddress(OntologySignature Ontology, int VariantId)
{
    /// <summary>
    /// Exactly <see cref="int.MaxValue"/> non-negative variant slots are reserved
    /// for each ontology coordinate: 0 through int.MaxValue - 1.
    /// </summary>
    public const long VariantCapacity = int.MaxValue;

    /// <summary>Gets whether the variant occupies a valid non-negative address slot.</summary>
    public bool IsValid => VariantId >= 0 && VariantId < int.MaxValue;

    /// <summary>
    /// Stable structural coordinate used by catalogs and persistence layers.
    /// The variant is deliberately kept separate from OntologySignature so the
    /// ontology remains reusable while each concrete slot remains unique.
    /// </summary>
    public ulong StructuralId
    {
        get
        {
            unchecked
            {
                var hash = Ontology.StructuralId;
                hash ^= (uint)VariantId;
                hash *= 1099511628211UL;
                return hash;
            }
        }
    }

    /// <summary>Creates and validates a MicroBundle address.</summary>
    public static MicroBundleAddress Create(OntologySignature ontology, int variantId)
    {
        var address = new MicroBundleAddress(ontology, variantId);
        if (!address.IsValid)
            throw new ArgumentOutOfRangeException(nameof(variantId), "VariantId must be between 0 and int.MaxValue - 1.");

        return address;
    }
}
