using System;

namespace TheSingularityWorkshop.Workshop.Laboratory;

public enum LogicGate
{
    And,
    Or,
    Xor,
    Nand,
    Nor,
    Not
}

public readonly record struct FullAdderResult(bool Sum, bool Carry);

/// <summary>
/// Small deterministic digital-logic substrate for gates and arithmetic primitives.
/// Higher-level CPU parts can compose these primitives without depending on a GUI
/// or a particular hardware simulator.
/// </summary>
public static class DigitalLogicSimulator
{
    public static bool Evaluate(LogicGate gate, bool a, bool b = false) => gate switch
    {
        LogicGate.And => a && b,
        LogicGate.Or => a || b,
        LogicGate.Xor => a ^ b,
        LogicGate.Nand => !(a && b),
        LogicGate.Nor => !(a || b),
        LogicGate.Not => !a,
        _ => throw new ArgumentOutOfRangeException(nameof(gate))
    };

    public static FullAdderResult FullAdder(bool a, bool b, bool carryIn)
    {
        var sum = a ^ b ^ carryIn;
        var carry = (a && b) || (carryIn && (a ^ b));
        return new FullAdderResult(sum, carry);
    }

    public static uint Add(uint left, uint right, bool carryIn = false)
    {
        var result = 0u;
        var carry = carryIn;

        for (var bit = 0; bit < 32; bit++)
        {
            var a = (left & (1u << bit)) != 0;
            var b = (right & (1u << bit)) != 0;
            var full = FullAdder(a, b, carry);

            if (full.Sum)
                result |= 1u << bit;

            carry = full.Carry;
        }

        return result;
    }
}
