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

    private readonly List<SpatialSecurityQueueAgent> _agents = [];
    private readonly Dictionary<string, SpatialSecurityAgentStage> _serviceStages =
        new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _agentSpeech =
        new(StringComparer.Ordinal);

    public SpatialSecurityCheckpointModel() => Reset();

    public SpatialSecurityCheckpointStage Stage { get; private set; }
    public SpatialSecurityClearance BadgeClearance { get; private set; } = SpatialSecurityClearance.Visitor;
    public SpatialSecurityClearance CheckpointClearance { get; set; } = SpatialSecurityClearance.Visitor;
    public string BadgeId { get; private set; } = "VISITOR";
    public int PlayerQueueSlot { get; private set; } = 4;
    public bool TrayOnRollers { get; private set; }
    public bool BelongingsOnTray { get; private set; }
    public bool ScannerCleared { get; private set; }
    public bool Agitated { get; private set; }
    public int CutAttempts { get; private set; }
    public int QueueProgresses { get; private set; }
    public string LastMessage { get; private set; } = "Join the line.";
    public IReadOnlyList<SpatialSecurityQueueAgent> Agents => _agents;
    public IReadOnlyDictionary<string, string> AgentSpeech => _agentSpeech;

    public bool HasClearedCheckpoint => Stage == SpatialSecurityCheckpointStage.Cleared;
    public bool CanFreeRoam => HasClearedCheckpoint;

    public IReadOnlyList<int> VacantQueueSlots
        => Enumerable.Range(0, QueueCapacity)
            .Where(slot => slot != PlayerQueueSlot && _agents.All(agent => agent.QueueSlot != slot))
            .ToArray();

    public SpatialSecurityAgentStage GetAgentStage(string id)
        => _serviceStages.TryGetValue(id, out var stage)
            ? stage
            : SpatialSecurityAgentStage.Waiting;

    public string? GetAgentSpeech(string id)
        => _agentSpeech.TryGetValue(id, out var speech) ? speech : null;

    public void Reset()
    {
        Stage = SpatialSecurityCheckpointStage.Approach;
        BadgeClearance = SpatialSecurityClearance.Visitor;
        BadgeId = "VISITOR";
        PlayerQueueSlot = 4;
        TrayOnRollers = false;
        BelongingsOnTray = false;
        ScannerCleared = false;
        Agitated = false;
        CutAttempts = 0;
        QueueProgresses = 0;
        LastMessage = "Join the line.";
        _serviceStages.Clear();
        _agentSpeech.Clear();

        _agents.Clear();
        _agents.AddRange(
        [
            new("security-research-01", "Research #13", SpatialSecurityClearance.Research, 0),
            new("security-research-02", "Research #21", SpatialSecurityClearance.Research, 1),
            new("security-admin-01", "Administrator #2", SpatialSecurityClearance.Administrator, 2),
            new("security-visitor-01", "Visitor", SpatialSecurityClearance.Visitor, 3),
            new("security-research-03", "Research #34", SpatialSecurityClearance.Research, 5),
            new("security-admin-02", "Administrator #7", SpatialSecurityClearance.Administrator, 6),
            new("security-visitor-02", "Visitor", SpatialSecurityClearance.Visitor, 7)
        ]);

        foreach (var agent in _agents)
            _serviceStages[agent.Id] = SpatialSecurityAgentStage.Waiting;

        RefreshSpeech();
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
            slot < 0 ||
            slot >= QueueCapacity ||
            slot == PlayerQueueSlot)
            return false;

        var distance = PlayerQueueSlot - slot;

        if (distance > 1)
        {
            CutAttempts++;
            Agitated = true;
            LastMessage = "The line stops you. Move one position at a time.";
            SpeakToAgents(_agents.Where(agent => agent.QueueSlot < PlayerQueueSlot), "HEY! WAIT YOUR TURN!");
            return false;
        }

        if (distance == 1 && _agents.Any(agent => agent.QueueSlot == slot))
        {
            LastMessage = "Someone is still there. Wait for the opening.";
            SpeakToAgents(_agents.Where(agent => agent.QueueSlot > PlayerQueueSlot), "HOLD UP! THEY'RE NOT THROUGH YET!");
            return false;
        }

        PlayerQueueSlot = slot;
        QueueProgresses++;
        Agitated = false;
        LastMessage = slot == 0
            ? "YOU ARE NEXT. THE SECURITY EQUIPMENT IS AHEAD."
            : "GOOD. FOLLOW THE SPACE THE LINE JUST GAVE YOU.";
        RefreshSpeech();

        if (slot == 0)
            Stage = SpatialSecurityCheckpointStage.TrayReady;

        return true;
    }

    /// <summary>
    /// Advances one Digitens' physical security processing. People ahead of the
    /// visitor are processed one at a time; people behind wait for the visitor.
    /// </summary>
    public void AdvanceQueueTraffic()
    {
        if (Stage != SpatialSecurityCheckpointStage.Queue) return;

        var front = _agents.FirstOrDefault(agent => agent.QueueSlot == 0);
        if (front.Id is not null)
        {
            var stage = GetAgentStage(front.Id);
            switch (stage)
            {
                case SpatialSecurityAgentStage.Waiting:
                    _serviceStages[front.Id] = SpatialSecurityAgentStage.Tray;
                    LastMessage = $"{front.Name} reached the screening equipment.";
                    break;
                case SpatialSecurityAgentStage.Tray:
                    _serviceStages[front.Id] = SpatialSecurityAgentStage.XRay;
                    LastMessage = $"{front.Name} placed belongings on the tray.";
                    break;
                case SpatialSecurityAgentStage.XRay:
                    _serviceStages[front.Id] = SpatialSecurityAgentStage.Scanner;
                    LastMessage = $"{front.Name} is walking through the scanner.";
                    break;
                case SpatialSecurityAgentStage.Scanner:
                    _serviceStages[front.Id] = SpatialSecurityAgentStage.Cleared;
                    LastMessage = $"{front.Name} cleared security.";
                    break;
                case SpatialSecurityAgentStage.Cleared:
                    _agents.Remove(front);
                    _serviceStages.Remove(front.Id);
                    for (var index = 0; index < _agents.Count; index++)
                    {
                        var agent = _agents[index];
                        if (agent.QueueSlot > 0 && agent.QueueSlot < PlayerQueueSlot)
                            _agents[index] = agent with { QueueSlot = agent.QueueSlot - 1 };
                    }
                    LastMessage = "THE FRONT OF THE LINE OPENED. MOVE INTO THE SPACE.";
                    break;
            }

            RefreshSpeech();
            return;
        }

        if (PlayerQueueSlot == 0)
        {
            Stage = SpatialSecurityCheckpointStage.TrayReady;
            LastMessage = "YOU ARE NEXT. TAKE THE TRAY.";
            RefreshSpeech();
            return;
        }

        var next = _agents
            .Where(agent => agent.QueueSlot > PlayerQueueSlot + 1)
            .OrderBy(agent => agent.QueueSlot)
            .FirstOrDefault();

        if (next.Id is not null)
        {
            _agents[_agents.FindIndex(agent => agent.Id == next.Id)] =
                next with { QueueSlot = PlayerQueueSlot + 1 };
            LastMessage = $"{next.Name} moved into the open space behind you.";
        }

        RefreshSpeech();
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

    private void RefreshSpeech()
    {
        _agentSpeech.Clear();

        foreach (var agent in _agents)
        {
            if (agent.QueueSlot == PlayerQueueSlot - 1)
                _agentSpeech[agent.Id] = "MOVE UP.";
            else if (agent.QueueSlot == PlayerQueueSlot + 1)
                _agentSpeech[agent.Id] = "COME ON. MOVE.";
            else if (agent.QueueSlot < PlayerQueueSlot)
                _agentSpeech[agent.Id] = GetAgentStage(agent.Id) is SpatialSecurityAgentStage.Scanner or SpatialSecurityAgentStage.Cleared
                    ? "KEEP THE LINE MOVING."
                    : string.Empty;
            else
                _agentSpeech[agent.Id] = string.Empty;
        }
    }

    private void SpeakToAgents(IEnumerable<SpatialSecurityQueueAgent> agents, string message)
    {
        foreach (var agent in agents)
            _agentSpeech[agent.Id] = message;
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

public enum SpatialSecurityAgentStage
{
    Waiting,
    Tray,
    XRay,
    Scanner,
    Cleared
}

public readonly record struct SpatialSecurityQueueAgent(
    string Id,
    string Name,
    SpatialSecurityClearance Clearance,
    int QueueSlot);
