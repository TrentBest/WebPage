using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using _FSM_API = TheSingularityWorkshop.FSM_API.FSM_API;

public class BlazorFSMIntegration : IDisposable
{
    private const int UpdateRateMs = 33;
    public List<string> UpdateGroups { get; private set; } = new() { "Update" };

    public event Action? OnStateChanged;

    private PeriodicTimer? _gameLoopTimer;
    private CancellationTokenSource? _cts;
    private Task? _loopTask;

    // Removed unused '_isRunning' field to fix warning

    public BlazorFSMIntegration()
    {
        foreach (var group in UpdateGroups) _FSM_API.Create.CreateProcessingGroup(group);
        StartLoop();
    }

    public void StartLoop()
    {
        _cts = new CancellationTokenSource();
        _gameLoopTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(UpdateRateMs));
        _loopTask = RunGameLoopAsync(_cts.Token);
    }

    public void StopLoop()
    {
        _cts?.Cancel();
    }

    private async Task RunGameLoopAsync(CancellationToken token)
    {
        try
        {
            if (_gameLoopTimer == null) return;

            while (await _gameLoopTimer.WaitForNextTickAsync(token))
            {
                foreach (var group in UpdateGroups)
                {
                    _FSM_API.Interaction.Update(group);
                }
                OnStateChanged?.Invoke();
            }
        }
        catch (OperationCanceledException) { /* Graceful shutdown */ }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _gameLoopTimer?.Dispose();
    }
}
