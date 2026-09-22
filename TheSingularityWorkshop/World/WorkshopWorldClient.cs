using System.Net.Http.Json;

namespace TheSingularityWorkshop.World;

public sealed class WorkshopWorldClient : IAsyncDisposable
{
    private readonly HttpClient _http;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(3));
    private CancellationTokenSource? _cts;
    private long _observedEpoch = -1;

    public WorkshopWorldClient(HttpClient http) => _http = http;

    public WorkshopWorldState State { get; private set; } = WorkshopWorldState.CreateInitial();
    public event Action<WorkshopWorldState>? WorldChanged;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_cts is not null) return;
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await RefreshAsync(_cts.Token);
        while (await _timer.WaitForNextTickAsync(_cts.Token))
            await RefreshAsync(_cts.Token);
    }

    public async Task<WorkshopWorldState> ActivateAsync(
        HiddenWorldSwitch worldSwitch,
        string discovererId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(worldSwitch);

        using var response = await _http.PostAsJsonAsync(
            "api/world/switch",
            new { SwitchId = worldSwitch.Id, LocationId = worldSwitch.LocationId, DiscovererId = discovererId },
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return await RefreshAsync(cancellationToken);
    }

    public async Task<WorkshopWorldState> RefreshAsync(CancellationToken cancellationToken = default)
    {
        var dto = await _http.GetFromJsonAsync<WorldStateDto>("api/world/state", cancellationToken)
            ?? throw new InvalidOperationException("The Workshop world endpoint returned no state.");

        if (dto.Epoch <= _observedEpoch)
            return State;

        var eventData = dto.LastEvent is null
            ? null
            : new WorkshopWorldEvent(
                dto.LastEvent,
                dto.LocationId ?? string.Empty,
                dto.DiscovererId ?? "anonymous",
                ParseMode(dto.PresentationMode),
                dto.EventTimestampUtc ?? DateTimeOffset.UtcNow);

        State = new WorkshopWorldState(ParseMode(dto.PresentationMode), dto.Epoch, eventData);
        _observedEpoch = dto.Epoch;
        WorldChanged?.Invoke(State);
        return State;
    }

    public ValueTask DisposeAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _timer.Dispose();
        return ValueTask.CompletedTask;
    }

    private static WorkshopPresentationMode ParseMode(string mode)
        => string.Equals(mode, "ThreeDimensional", StringComparison.OrdinalIgnoreCase)
            ? WorkshopPresentationMode.ThreeDimensional
            : WorkshopPresentationMode.TwoDimensional;

    private sealed record WorldStateDto(
        string PresentationMode,
        long Epoch,
        string? LastEvent,
        string? SwitchId,
        string? LocationId,
        string? DiscovererId,
        DateTimeOffset? EventTimestampUtc);
}
