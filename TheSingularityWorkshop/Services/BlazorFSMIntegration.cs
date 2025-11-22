using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using TheSingularityWorkshop.FSM_API;


public class BlazorFSMIntegration : IDisposable
{
    private const int UpdateRateMs = 33;
    public List<string> UpdateGroups { get; private set; } = new() { "Update" };

    // >>> THIS IS THE MISSING PIECE <<<
    // This event allows the UI to redraw whenever the FSM ticks
    public event Action OnStateChanged;

    private PeriodicTimer _gameLoopTimer;
    private CancellationTokenSource _cts;
    private Task _loopTask;

    public BlazorFSMIntegration()
    {
        // Create groups if they don't exist
        foreach (var group in UpdateGroups) FSM_API.Create.CreateProcessingGroup(group);

        //StartLoop();
    }

    private void StartLoop()
    {
        _cts = new CancellationTokenSource();
        _gameLoopTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(UpdateRateMs));
        _loopTask = RunGameLoopAsync(_cts.Token);
    }

    private async Task RunGameLoopAsync(CancellationToken token)
    {
        try
        {
            while (await _gameLoopTimer.WaitForNextTickAsync(token))
            {
                foreach (var group in UpdateGroups)
                {
                    FSM_API.Interaction.Update(group);
                }
                // Invoke the event to notify subscribers (App.razor)
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
