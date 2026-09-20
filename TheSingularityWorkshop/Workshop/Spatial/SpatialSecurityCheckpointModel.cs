namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Reusable spatial security-checkpoint substrate.
///
/// The checkpoint is deliberately modeled as a physical space rather than a menu:
/// a queue has occupied/vacant positions, a tray moves onto the X-ray rollers,
/// belongings move into the tray, the visitor walks through the scanner, and
/// the tray is later collected. The same substrate can be recomposed for
/// laboratories, airports, starships, offices, secure facilities, or simulations.
/// </summary>
public sealed class SpatialSecurityCheckpointModel
{
    public const int QueueCapacity = 5;

    private readonly List<SpatialSecurityQueueAgent> _agents =
    [
        new("security-employee-01", "Research #13", SpatialSecurityClearance.Research, 0),
        new("security-employee-02", "Administrator #2", SpatialSecurityClearance.Administrator, 1),
        new("security-visitor-01", "Visitor", SpatialSecurityClearance.Visitor, 2)
    ];

    public SpatialSecurityCheckpointModel() => Reset();

    public SpatialSecurityCheckpointStage Stage { get; private set; }
    public SpatialSecurityClearance BadgeClearance { get; private set; } = SpatialSecurityClearance.Visitor;
    public SpatialSecurityClearance CheckpointClearance { get; set; } = SpatialSecurityClearance.Visitor;
    public string BadgeId { get; private set; } = "VISITOR";
    public int PlayerQueueSlot { get; private set; } = QueueCapacity - 1;
    public bool TrayOnRollers { get; private set; }
    public bool BelongingsOnTray { get; private set; }
    public bool ScannerCleared { get; private set; }
    public bool Agitated { get; private set; }
    public int CutAttempts { get; private set; }
    public int QueueProgresses { get; private set; }
    public string LastMessage { get; private set; } = "Proceed directly to the security checkpoint.";
    public IReadOnlyList<SpatialSecurityQueueAgent> Agents => _agents;

    public bool HasClearedCheckpoint => Stage == SpatialSecurityCheckpointStage.Cleared;
    public bool CanFreeRoam => HasClearedCheckpoint;

    public IReadOnlyList<int> VacantQueueSlots
        => Enumerable.Range(0, QueueCapacity)
            .Where(slot => slot != PlayerQueueSlot && _agents.All(agent => agent.QueueSlot != slot))
            .ToArray();

    public void Reset()
    {
        Stage = SpatialSecurityCheckpointStage.Approach;
        BadgeClearance = SpatialSecurityClearance.Visitor;
        BadgeId = "VISITOR";
        PlayerQueueSlot = QueueCapacity - 1;
        TrayOnRollers = false;
        BelongingsOnTray = false;
        ScannerCleared = false;
        Agitated = false;
        CutAttempts = 0;
        QueueProgresses = 0;
        LastMessage = "Proceed directly to the security checkpoint.";

        _agents[0] = _agents[0] with { QueueSlot = 0 };
        _agents[1] = _agents[1] with { QueueSlot = 1 };
        _agents[2] = _agents[2] with { QueueSlot = 2 };
    }

    public void EnterQueue()
    {
        if (Stage != SpatialSecurityCheckpointStage.Approach) return;
        Stage = SpatialSecurityCheckpointStage.Queue;
        LastMessage = "SECURITY QUEUE // Click a vacated space to move forward.";
    }

    public bool TryMoveToQueueSlot(int slot)
    {
        if (Stage != SpatialSecurityCheckpointStage.Queue ||
            slot < 0 || slot >= QueueCapacity ||
            slot == PlayerQueueSlot)
            return false;

        if (_agents.Any(agent => agent.QueueSlot == slot))
        {
            LastMessage = "That position is occupied. Please wait for the line to move.";
            return false;
        }

        var distanceForward = PlayerQueueSlot - slot;
        if (distanceForward > 1)
        {
            CutAttempts++;
            Agitated = true;
            LastMessage = "HEY! NO CUTTING! The line moves one space at a time.";
        }
        else
        {
            LastMessage = slot == 0
                ? "You're next. Click the tray section on the X-ray rollers."
                : "Good. The line advanced. Click the next vacated space when it opens.";
        }

        PlayerQueueSlot = slot;
        QueueProgresses++;

        if (slot == 0)
            Stage = SpatialSecurityCheckpointStage.TrayReady;

        return true;
    }

    public bool PlaceTray()
    {
        if (Stage != SpatialSecurityCheckpointStage.TrayReady) return false;
        TrayOnRollers = true;
        Stage = SpatialSecurityCheckpointStage.TrayLoaded;
        LastMessage = "TRAY ON ROLLERS. Click the tray to place your belongings on it.";
        return true;
    }

    public bool LoadBelongings()
    {
        if (Stage != SpatialSecurityCheckpointStage.TrayLoaded || !TrayOnRollers) return false;
        BelongingsOnTray = true;
        Stage = SpatialSecurityCheckpointStage.Scanner;
        LastMessage = "BELONGINGS LOADED. Walk through the scanner and wait for the agent.";
        return true;
    }

    public bool WalkThroughScanner()
    {
        if (Stage != SpatialSecurityCheckpointStage.Scanner || !BelongingsOnTray) return false;
        ScannerCleared = true;
        Stage = SpatialSecurityCheckpointStage.Collection;
        LastMessage = "WAIT... OK, GOOD! Proceed to the collection point.";
        return true;
    }

    public bool CollectTray()
    {
        if (Stage != SpatialSecurityCheckpointStage.Collection || !ScannerCleared) return false;
        Stage = SpatialSecurityCheckpointStage.Cleared;
        LastMessage = $"SECURITY CLEAR. Badge {BadgeId} // {BadgeClearance}. You may roam within your clearance.";
        return true;
    }

    public bool TryUpgradeBadge(string badgeId, SpatialSecurityClearance clearance)
    {
        if (!HasClearedCheckpoint || clearance < BadgeClearance || string.IsNullOrWhiteSpace(badgeId))
            return false;

        BadgeId = badgeId.Trim().ToUpperInvariant();
        BadgeClearance = clearance;
        LastMessage = $"BADGE TIER UPDATED // {BadgeId} // {BadgeClearance}.";
        return true;
    }

    public void AdvanceQueueTraffic()
    {
        if (Stage != SpatialSecurityCheckpointStage.Queue) return;

        // The checkpoint service point consumes the person at the front,
        // then everyone behind them advances into the vacated spaces.
        if (_agents.Any(agent => agent.QueueSlot == 0))
        {
            _agents.RemoveAll(agent => agent.QueueSlot == 0);
            for (var index = 0; index < _agents.Count; index++)
            {
                var agent = _agents[index];
                if (agent.QueueSlot > 0)
                    _agents[index] = agent with { QueueSlot = agent.QueueSlot - 1 };
            }

            LastMessage = "SECURITY CALLED THE NEXT PERSON. A SPACE JUST OPENED IN THE LINE.";
            return;
        }

        for (var index = 0; index < _agents.Count; index++)
        {
            var agent = _agents[index];
            if (agent.QueueSlot <= 0) continue;

            var target = agent.QueueSlot - 1;
            if (target == PlayerQueueSlot || _agents.Any(other => other.Id != agent.Id && other.QueueSlot == target))
                continue;

            _agents[index] = agent with { QueueSlot = target };
            LastMessage = $"{agent.Name} moved forward. Click the vacated space to advance.";
            return;
        }
    }
}

public enum SpatialSecurityCheckpointStage
{
    Approach,
    Queue,
    TrayReady,
    TrayLoaded,
    Scanner,
    Collection,
    Cleared
}

public readonly record struct SpatialSecurityQueueAgent(
    string Id,
    string Name,
    SpatialSecurityClearance Clearance,
    int QueueSlot);
