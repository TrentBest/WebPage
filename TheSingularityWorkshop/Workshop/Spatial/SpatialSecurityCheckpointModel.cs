namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Physical, plan-view security line. Digitens occupy the line positions and
/// provide the social cues that teach the visitor how to move through the space.
/// </summary>
public sealed class SpatialSecurityCheckpointModel
{
    public const int QueueCapacity = 8;

    private readonly List<SpatialSecurityQueueAgent> _agents = [];
    private readonly Dictionary<string, SpatialSecurityAgentStage> _serviceStages = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _agentSpeech = new(StringComparer.Ordinal);

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
    public int ServicePulse { get; private set; }
    public int QueueWaitTicks { get; private set; }
    public string LastMessage { get; private set; } = "Join the line.";
    public IReadOnlyList<SpatialSecurityQueueAgent> Agents => _agents;
    public IReadOnlyDictionary<string, string> AgentSpeech => _agentSpeech;

    /// <summary>
    /// Returns queue positions that are physically open for the visitor to move into.
    /// The visitor's current position is not considered vacant.
    /// </summary>
    public IReadOnlyList<int> VacantQueueSlots
        => Enumerable.Range(0, QueueCapacity)
            .Where(slot => slot != PlayerQueueSlot && _agents.All(agent => agent.QueueSlot != slot))
            .ToArray();

    public bool HasClearedCheckpoint => Stage == SpatialSecurityCheckpointStage.Cleared;
    public bool CanFreeRoam => HasClearedCheckpoint;

    public SpatialSecurityAgentStage GetAgentStage(string id)
        => _serviceStages.TryGetValue(id, out var stage) ? stage : SpatialSecurityAgentStage.Waiting;

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
        ServicePulse = 0;
        QueueWaitTicks = 0;
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
        LastMessage = "WATCH THE LINE. MOVE WHEN THE SPACE IN FRONT OF YOU OPENS.";
        RefreshSpeech();
    }

    public bool TryMoveToQueueSlot(int slot)
    {
        if (Stage != SpatialSecurityCheckpointStage.Queue || slot < 0 || slot >= QueueCapacity || slot == PlayerQueueSlot)
            return false;

        var distance = PlayerQueueSlot - slot;

        if (distance != 1)
        {
            CutAttempts++;
            Agitated = true;
            LastMessage = "The line teaches you to move one position at a time.";
            if (distance > 1)
                SpeakToAgents(_agents.Where(agent => agent.QueueSlot < PlayerQueueSlot), "HEY! WAIT YOUR TURN!");
            return false;
        }

        if (_agents.Any(agent => agent.QueueSlot == slot))
        {
            LastMessage = "Someone is still there. Wait for the opening.";
            SpeakToAgents(_agents.Where(agent => agent.QueueSlot > PlayerQueueSlot), "HOLD UP! THEY'RE NOT THROUGH YET!");
            return false;
        }

        PlayerQueueSlot = slot;
        QueueProgresses++;
        QueueWaitTicks = 0;
        Agitated = false;
        LastMessage = slot == 0
            ? "YOU ARE NEXT. THE SECURITY EQUIPMENT IS AHEAD."
            : "GOOD. FOLLOW THE SPACE THE LINE JUST GAVE YOU.";
        RefreshSpeech();

        if (slot == 0)
            Stage = SpatialSecurityCheckpointStage.TrayReady;

        return true;
    }

    /// <summary>Processes the Digitens ahead of the visitor one physical screening step at a time.</summary>
    public void AdvanceQueueTraffic()
    {
        if (Stage != SpatialSecurityCheckpointStage.Queue)
            return;

        QueueWaitTicks++;
        Agitated |= QueueWaitTicks >= 2;

        var front = _agents.FirstOrDefault(agent => agent.QueueSlot == 0);

        if (front.Id is not null)
        {
            // One traffic advance represents one Digitens completing the physical
            // screening sequence: tray, X-ray, person scanner, clearance, then exit.
            _serviceStages[front.Id] = SpatialSecurityAgentStage.Tray;
            _serviceStages[front.Id] = SpatialSecurityAgentStage.XRay;
            _serviceStages[front.Id] = SpatialSecurityAgentStage.Scanner;
            _serviceStages[front.Id] = SpatialSecurityAgentStage.Cleared;

            _agents.Remove(front);
            _serviceStages.Remove(front.Id);
            ServicePulse++;

            // Close the physical line only for people ahead of the visitor.
            // The resulting empty position teaches the visitor where to move.
            for (var i = 0; i < _agents.Count; i++)
            {
                var agent = _agents[i];

                if (agent.QueueSlot > 0 && agent.QueueSlot < PlayerQueueSlot)
                    _agents[i] = agent with { QueueSlot = agent.QueueSlot - 1 };
            }

            LastMessage = $"{front.Name} cleared security. THE FRONT OF THE LINE OPENED.";
            RefreshSpeech();
            return;
        }

        // Once the visitor has moved into the newly opened space, bring the next
        // Digitens forward behind them when there is room to do so.
        var next = _agents
            .Where(agent => agent.QueueSlot > PlayerQueueSlot + 1)
            .OrderBy(agent => agent.QueueSlot)
            .FirstOrDefault();

        if (next.Id is not null)
        {
            _agents[_agents.FindIndex(agent => agent.Id == next.Id)] =
                next with { QueueSlot = PlayerQueueSlot + 1 };

            LastMessage = $"{next.Name} moved closer.";
        }

        RefreshSpeech();
    }

    public bool PlaceTray()
    {
        if (Stage != SpatialSecurityCheckpointStage.TrayReady) return false;
        TrayOnRollers = true;
        Stage = SpatialSecurityCheckpointStage.TrayLoaded;
        LastMessage = "THE TRAY IS ON THE ROLLERS. PUT YOUR BELONGINGS ON IT.";
        return true;
    }

    public bool LoadBelongings()
    {
        if (Stage != SpatialSecurityCheckpointStage.TrayLoaded || !TrayOnRollers) return false;
        BelongingsOnTray = true;
        Stage = SpatialSecurityCheckpointStage.Scanner;
        LastMessage = "BELONGINGS ARE MOVING THROUGH X-RAY. NOW WALK THROUGH.";
        return true;
    }

    public bool WalkThroughScanner()
    {
        if (Stage != SpatialSecurityCheckpointStage.Scanner || !BelongingsOnTray) return false;
        ScannerCleared = true;
        Stage = SpatialSecurityCheckpointStage.Collection;
        LastMessage = "SCANNER CLEARED. GO TO THE OTHER SIDE AND COLLECT YOUR TRAY.";
        return true;
    }

    public bool CollectTray()
    {
        if (Stage != SpatialSecurityCheckpointStage.Collection || !ScannerCleared) return false;
        Stage = SpatialSecurityCheckpointStage.Cleared;
        LastMessage = $"SECURITY CLEAR. BADGE {BadgeId} // {BadgeClearance}.";
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

    private void RefreshSpeech()
    {
        _agentSpeech.Clear();

        foreach (var agent in _agents)
        {
            if (agent.QueueSlot == PlayerQueueSlot - 1)
                _agentSpeech[agent.Id] = "MOVE UP.";
            else if (agent.QueueSlot == PlayerQueueSlot + 1)
                _agentSpeech[agent.Id] = Agitated ? "MOVE. COME ON." : "COME ON. MOVE.";
            else if (agent.QueueSlot < PlayerQueueSlot &&
                     GetAgentStage(agent.Id) is SpatialSecurityAgentStage.Scanner or SpatialSecurityAgentStage.Cleared)
                _agentSpeech[agent.Id] = "KEEP THE LINE MOVING.";
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
