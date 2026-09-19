namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Declarative language-to-voice pipeline. Text is decomposed into reusable linguistic units,
/// then composed into speech. The first implementation may run as a desktop bake tool before
/// the browser receives a compact runtime representation.
/// </summary>
public sealed record LanguageVocalizationManifest(
    IReadOnlyList<LanguageVocalizationProfile> Languages,
    IReadOnlyList<VocalUnitKind> UnitKinds)
{
    public static LanguageVocalizationManifest CreateDefault()
        => new(
        [
            new("en", "English", "phoneme", true),
            new("de", "German", "phoneme", true),
            new("es", "Spanish", "phoneme", true),
            new("fr", "French", "phoneme", true),
            new("ja", "Japanese", "mora", true),
            new("ko", "Korean", "syllable", true),
            new("tlh", "Klingon", "phoneme", false)
        ],
        [VocalUnitKind.Phoneme, VocalUnitKind.Morpheme, VocalUnitKind.Word, VocalUnitKind.Phrase]);

    public LanguageVocalizationProfile? FindLanguage(string id)
        => Languages.FirstOrDefault(x => x.Id == id);
}

public readonly record struct LanguageVocalizationProfile(
    string Id,
    string Name,
    string AtomicUnit,
    bool NaturalLanguageReference);

public enum VocalUnitKind
{
    Phoneme,
    Morpheme,
    Word,
    Phrase
}
