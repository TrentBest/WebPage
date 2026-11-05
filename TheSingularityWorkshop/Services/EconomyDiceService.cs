using System;
using System.Collections.Generic;
using System.Linq;
#if WORKSHOP_USE_REPOSITORY_SOURCES
using TheSingularityWorkshop.Economy;
#endif

namespace TheSingularityWorkshop.Services;

/// <summary>Presentation-safe result projected from Economy's authoritative dice domain.</summary>
public sealed record EconomyDiceResult(Guid RollId, DateTimeOffset RolledAt, int FaceCount,
    IReadOnlyList<int> Faces, int Total, int Highest, bool? Doubles, bool? AtLeastTen);

/// <summary>Thin WebPage adapter; it never generates outcomes outside Economy.</summary>
public sealed class EconomyDiceService
{
#if WORKSHOP_USE_REPOSITORY_SOURCES
    private readonly DiceRoller _roller = new(new SystemDiceRandomSource());
    public bool IsAvailable => true;

    public EconomyDiceResult Roll(int faceCount, int quantity)
    {
        var roll = _roller.Roll(new DiceDefinition(faceCount), quantity);
        var sum = DiceRuleEvaluator.Evaluate(roll, DiceRuleDefinition.Sum);
        var highest = DiceRuleEvaluator.Evaluate(roll, DiceRuleDefinition.Highest);
        bool? doubles = quantity == 2
            ? DiceRuleEvaluator.Evaluate(roll, DiceRuleDefinition.Doubles).IsSatisfied
            : null;
        var threshold = DiceRuleEvaluator.Evaluate(roll, DiceRuleDefinition.AtLeastTotal(10));
        return new EconomyDiceResult(roll.RollId, roll.RolledAt, roll.FaceCount,
            roll.Faces.ToArray(), sum.Value, highest.Value, doubles, threshold.IsSatisfied);
    }
#else
    public bool IsAvailable => false;

    public EconomyDiceResult Roll(int faceCount, int quantity) =>
        throw new InvalidOperationException("Dice rolls require Workshop repository-source integration.");
#endif
}
