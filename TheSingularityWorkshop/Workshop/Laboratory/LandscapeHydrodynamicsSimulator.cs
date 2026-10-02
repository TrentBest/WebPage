using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Laboratory;

/// <summary>
/// A deterministic, renderer-neutral landscape hydrodynamics substrate.
///
/// This is intentionally a reduced shallow-water model rather than a claim to be
/// production CFD. Water is represented on an arbitrary terrain grid; rainfall,
/// wind, waves, tectonic displacement, erosion/deposition, and floating craft are
/// explicit effectors. The same state can therefore drive a diegetic laboratory
/// bench, a toy-boat experience, or a future high-resolution numerical solver.
/// </summary>
public sealed class LandscapeHydrodynamicsSimulator
{
    private readonly LandscapeCell[] _cells;
    private readonly int _width;
    private readonly int _height;
    private long _step;

    public LandscapeHydrodynamicsSimulator(
        int width,
        int height,
        IReadOnlyList<double> terrainElevationM,
        double cellSizeM = 1)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        if (terrainElevationM.Count != width * height)
            throw new ArgumentException("Terrain elevation count must equal width × height.", nameof(terrainElevationM));
        if (cellSizeM <= 0) throw new ArgumentOutOfRangeException(nameof(cellSizeM));

        _width = width;
        _height = height;
        CellSizeM = cellSizeM;
        _cells = new LandscapeCell[terrainElevationM.Count];

        for (var i = 0; i < _cells.Length; i++)
            _cells[i] = new LandscapeCell(terrainElevationM[i], 0, 0, 0);
    }

    public double CellSizeM { get; }
    public double GravityMPerS2 { get; init; } = 9.81;
    public double ErosionCoefficient { get; init; } = 0.01;
    public double DepositionCoefficient { get; init; } = 0.005;
    public IReadOnlyList<LandscapeCell> Cells => _cells;
    public IReadOnlyList<BoatState> Boats => _boats;
    private readonly List<BoatState> _boats = [];

    public LandscapeCell GetCell(int x, int y)
    {
        ValidateCoordinates(x, y);
        return _cells[Index(x, y)];
    }

    public void ApplyWeather(WeatherForcing forcing)
    {
        if (forcing.RainfallMPerS < 0) throw new ArgumentOutOfRangeException(nameof(forcing));
        if (forcing.WindMPerS < 0) throw new ArgumentOutOfRangeException(nameof(forcing));

        for (var i = 0; i < _cells.Length; i++)
        {
            var water = _cells[i].WaterDepthM + forcing.RainfallMPerS * forcing.DurationS;
            _cells[i] = _cells[i] with { WaterDepthM = water };
        }

        WindX = forcing.WindXMPerS;
        WindY = forcing.WindYMPerS;
    }

    public void ApplyTectonics(TectonicForcing forcing)
    {
        if (forcing.DisplacementM.Count != _cells.Length)
            throw new ArgumentException("Tectonic displacement count must equal the landscape cell count.", nameof(forcing));

        for (var i = 0; i < _cells.Length; i++)
            _cells[i] = _cells[i] with { TerrainElevationM = _cells[i].TerrainElevationM + forcing.DisplacementM[i] };
    }

    public void ApplyWave(WaveForcing forcing)
    {
        ValidateCoordinates(forcing.X, forcing.Y);
        if (forcing.WaterDepthM < 0) throw new ArgumentOutOfRangeException(nameof(forcing));

        var cell = _cells[Index(forcing.X, forcing.Y)];
        _cells[Index(forcing.X, forcing.Y)] = cell with
        {
            WaterDepthM = Math.Max(cell.WaterDepthM, forcing.WaterDepthM),
            VelocityXMPerS = cell.VelocityXMPerS + forcing.VelocityXMPerS,
            VelocityYMPerS = cell.VelocityYMPerS + forcing.VelocityYMPerS
        };
    }

    public BoatState LaunchBoat(BoatState boat)
    {
        ValidateCoordinates(boat.X, boat.Y);
        if (boat.MassKg <= 0) throw new ArgumentOutOfRangeException(nameof(boat));

        _boats.Add(boat);
        return boat;
    }

    public LandscapeHydrodynamicStep Step(double timeStepS)
    {
        if (timeStepS <= 0) throw new ArgumentOutOfRangeException(nameof(timeStepS));

        var before = (LandscapeCell[])_cells.Clone();
        var next = (LandscapeCell[])_cells.Clone();
        var maxWaveHeight = 0d;
        var eroded = 0d;

        for (var y = 0; y < _height; y++)
        for (var x = 0; x < _width; x++)
        {
            var index = Index(x, y);
            var cell = before[index];
            var water = cell.WaterDepthM;
            if (water <= 0)
            {
                next[index] = cell with
                {
                    VelocityXMPerS = cell.VelocityXMPerS * 0.5,
                    VelocityYMPerS = cell.VelocityYMPerS * 0.5
                };
                continue;
            }

            var velocityX = cell.VelocityXMPerS + WindX * 0.001 * timeStepS;
            var velocityY = cell.VelocityYMPerS + WindY * 0.001 * timeStepS;

            // Hydrostatic head drives water toward neighboring lower free-surface cells.
            // This is a deterministic finite-volume-style approximation, not full CFD.
            foreach (var (nx, ny) in Neighbors(x, y))
            {
                var neighbor = before[Index(nx, ny)];
                var head = (cell.TerrainElevationM + water) -
                           (neighbor.TerrainElevationM + neighbor.WaterDepthM);
                var acceleration = head / CellSizeM * GravityMPerS2;
                velocityX += (nx - x) * acceleration * timeStepS * 0.25;
                velocityY += (ny - y) * acceleration * timeStepS * 0.25;
            }

            var speed = Math.Sqrt(velocityX * velocityX + velocityY * velocityY);
            var transport = Math.Min(water, speed * timeStepS / CellSizeM * water * 0.25);
            var newWater = Math.Max(0, water - transport);
            var sedimentCapacity = speed * speed * water * ErosionCoefficient;
            var terrainChange = sedimentCapacity > cell.SedimentKgPerM2
                ? Math.Min(sedimentCapacity - cell.SedimentKgPerM2, cell.TerrainElevationM + 1000) * ErosionCoefficient * timeStepS
                : -Math.Min(cell.SedimentKgPerM2 - sedimentCapacity, 1) * DepositionCoefficient * timeStepS;

            next[index] = cell with
            {
                WaterDepthM = newWater,
                VelocityXMPerS = velocityX * 0.995,
                VelocityYMPerS = velocityY * 0.995,
                TerrainElevationM = cell.TerrainElevationM - terrainChange,
                SedimentKgPerM2 = Math.Max(0, cell.SedimentKgPerM2 + terrainChange)
            };

            maxWaveHeight = Math.Max(maxWaveHeight, water);
            eroded += Math.Max(0, terrainChange);
        }

        Array.Copy(next, _cells, _cells.Length);
        var boatStates = AdvanceBoats(timeStepS);
        return new LandscapeHydrodynamicStep(++_step, maxWaveHeight, eroded, boatStates);
    }

    public double WindXMPerS { get; private set; }
    public double WindYMPerS { get; private set; }

    private IReadOnlyList<BoatState> AdvanceBoats(double timeStepS)
    {
        for (var i = 0; i < _boats.Count; i++)
        {
            var boat = _boats[i];
            var cell = GetCell((int)Math.Round(boat.X), (int)Math.Round(boat.Y));
            var buoyancy = Math.Max(0, cell.WaterDepthM) * boat.DisplacementAreaM2 * 1000 * GravityMPerS2;
            var verticalError = buoyancy - boat.MassKg * GravityMPerS2;
            var acceleration = Math.Clamp(verticalError / Math.Max(boat.MassKg, 0.001), -10, 10);

            _boats[i] = boat with
            {
                X = Math.Clamp(boat.X + (cell.VelocityXMPerS + WindXMPerS * 0.05) * timeStepS, 0, _width - 1),
                Y = Math.Clamp(boat.Y + (cell.VelocityYMPerS + WindYMPerS * 0.05) * timeStepS, 0, _height - 1),
                VerticalAccelerationMPerS2 = acceleration
            };
        }

        return _boats.ToArray();
    }

    private IEnumerable<(int X, int Y)> Neighbors(int x, int y)
    {
        if (x > 0) yield return (x - 1, y);
        if (x + 1 < _width) yield return (x + 1, y);
        if (y > 0) yield return (x, y - 1);
        if (y + 1 < _height) yield return (x, y + 1);
    }

    private int Index(int x, int y) => y * _width + x;

    private void ValidateCoordinates(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
            throw new ArgumentOutOfRangeException($"({x},{y}) is outside the landscape.");
    }
}

public readonly record struct LandscapeCell(
    double TerrainElevationM,
    double WaterDepthM,
    double VelocityXMPerS,
    double VelocityYMPerS,
    double SedimentKgPerM2 = 0);

public readonly record struct WeatherForcing(
    double RainfallMPerS,
    double WindXMPerS,
    double WindYMPerS,
    double DurationS);

public readonly record struct TectonicForcing(IReadOnlyList<double> DisplacementM);

public readonly record struct WaveForcing(
    int X,
    int Y,
    double WaterDepthM,
    double VelocityXMPerS,
    double VelocityYMPerS);

public readonly record struct BoatState(
    string Id,
    double X,
    double Y,
    double MassKg,
    double DisplacementAreaM2,
    double VerticalAccelerationMPerS2 = 0);

public readonly record struct LandscapeHydrodynamicStep(
    long Step,
    double MaximumWaterDepthM,
    double ErodedTerrainM,
    IReadOnlyList<BoatState> Boats);
