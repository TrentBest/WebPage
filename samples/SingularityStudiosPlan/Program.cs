using System.Globalization;
using System.Numerics;
using System.Text.Json;
using TheSingularityWorkshop.Visualize;
using TheSingularityWorkshop.Visualize.GuiSvg;
using TheSingularityWorkshop.Renderer;
using TheSingularityWorkshop.Renderer.VisualizeAdapter;

var root = FindWorkspaceRoot();
var sourcePath = Path.Combine(root, "TheSingularityWorkshop", "wwwroot", "Workshop", "Forge", "StreamingAssets", "Experiences", "singularity-studios-stage.json");
var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(root, "TheSingularityWorkshop", "wwwroot", "Workshop", "Forge", "StreamingAssets", "Experiences", "singularity-studios-stage-plan.svg");

using var document = JsonDocument.Parse(File.ReadAllText(sourcePath));
var data = document.RootElement;
var coordinateFrame = data.GetProperty("coordinateFrame");
var axes = coordinateFrame.GetProperty("axes");
var planProjection = coordinateFrame.GetProperty("planProjection");
if (axes.GetProperty("x").GetString() != "stage-width" ||
    axes.GetProperty("y").GetString() != "elevation" ||
    axes.GetProperty("z").GetString() != "stage-depth" ||
    planProjection.GetProperty("horizontalAxis").GetString() != "x" ||
    planProjection.GetProperty("verticalAxis").GetString() != "z" ||
    planProjection.GetProperty("omittedAxis").GetString() != "y")
{
    throw new InvalidDataException("The studio plan renderer requires the declared X-width, Y-elevation, Z-depth coordinate convention.");
}

var stage = data.GetProperty("stage");
var dimensions = stage.GetProperty("dimensions");
var stageWidth = dimensions.GetProperty("width").GetSingle();
var stageDepth = dimensions.GetProperty("depth").GetSingle();
if (stageWidth <= 0 || stageDepth <= 0)
    throw new InvalidDataException("Stage plan dimensions must be positive.");

var scale = 0.9f / stageWidth;
var catalog = new VisualizerResourceCatalog();
var quadId = new VisualizerResourceId(1);
var cameraId = new VisualizerResourceId(2);
var quadRenderId = new RenderResourceId(1);
var cameraRenderId = new RenderResourceId(2);
catalog.SetGeometry(quadId, Quad());
catalog.SetGeometry(cameraId, Triangle());

var stageMaterial = Material(10, new Vector4(0.025f, 0.055f, 0.09f, 1));
var zoneMainMaterial = Material(11, new Vector4(0.035f, 0.20f, 0.24f, 1));
var zoneRailMaterial = Material(12, new Vector4(0.16f, 0.08f, 0.22f, 1));
var zoneServiceMaterial = Material(13, new Vector4(0.20f, 0.14f, 0.045f, 1));
var setMaterial = Material(14, new Vector4(0.02f, 0.68f, 0.76f, 1));
var propMaterial = Material(15, new Vector4(1f, 0.72f, 0.28f, 1));
var cameraMaterial = Material(16, new Vector4(1f, 0.20f, 0.80f, 1));
var lightMaterial = Material(17, new Vector4(0.75f, 0.86f, 1f, 1));

var commands = new List<RenderDrawCommand>();
var observer = new Vector3(0, 0, 0.1f);
var representationPolicy = new ObserverRepresentationPolicy(
[
    new RepresentationHorizonRule(new EventHorizon("Interactive", 0.15, 16), RepresentationTarget.Interactive),
    new RepresentationHorizonRule(new EventHorizon("Near", 0.3, 8), RepresentationTarget.Near),
    new RepresentationHorizonRule(new EventHorizon("Distant", 0.5, 4), RepresentationTarget.Distant),
    new RepresentationHorizonRule(new EventHorizon("Aggregate", 0.8, 2), RepresentationTarget.Aggregate),
    new RepresentationHorizonRule(new EventHorizon("Semantic", 10, 1), RepresentationTarget.Semantic)
]);
ulong nextEntityId = 1;
AddRect("stage-floor", stageWidth, stageDepth, stageWidth / 2, stageDepth / 2, stageMaterial);
foreach (var zone in stage.GetProperty("zones").EnumerateArray())
{
    var bounds = zone.GetProperty("bounds");
    var material = zone.GetProperty("id").GetString() switch
    {
        "zone.main-set" => zoneMainMaterial,
        "zone.camera-rail" => zoneRailMaterial,
        _ => zoneServiceMaterial
    };
    AddZoneOutline(zone.GetProperty("id").GetString()!, bounds, material);
}

var definitions = data.GetProperty("assetDefinitions").EnumerateArray()
    .ToDictionary(item => item.GetProperty("id").GetString()!, item => item);
// Draw the stage boundary after zone outlines so it remains visible at the perimeter.
AddRect("stage-boundary-north", stageWidth, 2, stageWidth / 2, 1, lightMaterial);
AddRect("stage-boundary-south", stageWidth, 2, stageWidth / 2, stageDepth - 1, lightMaterial);
AddRect("stage-boundary-west", 2, stageDepth, 1, stageDepth / 2, lightMaterial);
AddRect("stage-boundary-east", 2, stageDepth, stageWidth - 1, stageDepth / 2, lightMaterial);

foreach (var instance in data.GetProperty("scene").GetProperty("instances").EnumerateArray())
{
    var assetId = instance.GetProperty("assetId").GetString()!;
    var definition = definitions[assetId];
    var footprint = definition.GetProperty("planFootprint");
    var position = instance.GetProperty("transform").GetProperty("position");
    var width = footprint.GetProperty("width").GetSingle();
    var depth = footprint.GetProperty("depth").GetSingle();
    var material = definition.GetProperty("kind").GetString() == "set" ? setMaterial : propMaterial;
    AddRect(instance.GetProperty("id").GetString()!, width, depth,
        position.GetProperty("x").GetSingle(), position.GetProperty("z").GetSingle(), material);
}

foreach (var camera in data.GetProperty("cameras").EnumerateArray())
{
    var transform = camera.GetProperty("transform");
    var position = transform.GetProperty("position");
    var yaw = transform.GetProperty("rotationDegrees").GetProperty("y").GetSingle();
    AddTriangle(camera.GetProperty("id").GetString()!, 12,
        position.GetProperty("x").GetSingle(), position.GetProperty("z").GetSingle(), yaw, cameraMaterial);
}

foreach (var light in data.GetProperty("lightingRigPlacements").EnumerateArray())
{
    var position = light.GetProperty("position");
    AddRect(light.GetProperty("id").GetString()!, 10, 10,
        position.GetProperty("x").GetSingle(), position.GetProperty("z").GetSingle(), lightMaterial);
}

var frame = new RenderFrame(
    new RenderView(observer, Matrix4x4.Identity, Matrix4x4.Identity),
    commands);
var representationSummary = string.Join(", ", commands.GroupBy(command => command.Representation)
    .OrderBy(group => group.Key)
    .Select(group => $"{group.Key}={group.Count()}"));
await using var visualizer = new Visualizer(
    new GuiSvgVisualizerBackend(
        outputPath,
        backgroundColor: "#030a14",
        gridColor: "#16404e",
        sceneLabel: $"Singularity Studios production plan · Renderer policy → Visualize → GUI.SVG · {representationSummary} · {stageWidth.ToString(CultureInfo.InvariantCulture)}m × {stageDepth.ToString(CultureInfo.InvariantCulture)}m · authored data",
        drawGrid: true),
    catalog);
var result = await visualizer.RenderAsync(frame, new RendererFrameAdapter());
Console.WriteLine($"Singularity Studios plan: {result.SubmittedDrawCount} draw commands; {representationSummary} -> {outputPath}");

void AddZoneOutline(string label, JsonElement bounds, RenderResourceId material)
{
    var x = bounds.GetProperty("x").GetSingle();
    var z = bounds.GetProperty("z").GetSingle();
    var width = bounds.GetProperty("width").GetSingle();
    var depth = bounds.GetProperty("depth").GetSingle();
    const float thickness = 3;
    AddRect(label + "-north", width, thickness, x + width / 2, z + thickness / 2, material);
    AddRect(label + "-south", width, thickness, x + width / 2, z + depth - thickness / 2, material);
    AddRect(label + "-west", thickness, depth, x + thickness / 2, z + depth / 2, material);
    AddRect(label + "-east", thickness, depth, x + width - thickness / 2, z + depth / 2, material);
}

void AddRect(string label, float width, float depth, float centerX, float centerZ, RenderResourceId material)
{
    var x = (centerX - stageWidth / 2) * scale;
    var y = (stageDepth / 2 - centerZ) * scale;
    var transform = Matrix4x4.CreateScale(width * scale, depth * scale, 1) *
        Matrix4x4.CreateTranslation(x, y, 0);
    var worldCenter = new Vector3(x, y, 0);
    var representation = ToRendererRepresentation(representationPolicy.Select(Vector3.Distance(observer, worldCenter)).Representation);
    commands.Add(new RenderDrawCommand(quadRenderId, material, transform, 6, 0, 0, representation, entity: new RenderEntityId(nextEntityId++)));
}

void AddTriangle(string label, float size, float centerX, float centerZ, float yawDegrees, RenderResourceId material)
{
    var x = (centerX - stageWidth / 2) * scale;
    var y = (stageDepth / 2 - centerZ) * scale;
    var transform = Matrix4x4.CreateScale(size * scale, size * scale, 1) *
        Matrix4x4.CreateRotationZ(yawDegrees * MathF.PI / 180f) *
        Matrix4x4.CreateTranslation(x, y, 0);
    var worldCenter = new Vector3(x, y, 0);
    var representation = ToRendererRepresentation(representationPolicy.Select(Vector3.Distance(observer, worldCenter)).Representation);
    commands.Add(new RenderDrawCommand(cameraRenderId, material, transform, 3, 0, 0, representation, entity: new RenderEntityId(nextEntityId++)));
}

RenderResourceId Material(ulong id, Vector4 color)
{
    catalog.SetMaterial(new VisualizerResourceId(id), new VisualizerMaterial(1, color));
    return new RenderResourceId(id);
}

static VisualizerGeometry Quad() => new(1,
[
    Vertex(-0.5f, -0.5f), Vertex(-0.5f, 0.5f),
    Vertex(0.5f, 0.5f), Vertex(0.5f, -0.5f)
], [0, 1, 2, 0, 2, 3]);

static VisualizerGeometry Triangle() => new(1,
[
    Vertex(0, 0.65f), Vertex(-0.48f, -0.4f), Vertex(0.48f, -0.4f)
], [0, 1, 2]);

static VisualizerVertex Vertex(float x, float y) =>
    new(new Vector3(x, y, 0), Vector3.UnitZ, Vector2.Zero, Vector4.One);

static RendererRepresentation ToRendererRepresentation(RepresentationTarget target) => target switch
{
    RepresentationTarget.Semantic => RendererRepresentation.Semantic,
    RepresentationTarget.Aggregate => RendererRepresentation.Aggregate,
    RepresentationTarget.Distant => RendererRepresentation.Distant,
    RepresentationTarget.Near => RendererRepresentation.Near,
    RepresentationTarget.Interactive => RendererRepresentation.Interactive,
    _ => throw new ArgumentOutOfRangeException(nameof(target), target, "Unknown Renderer representation.")
};

static string FindWorkspaceRoot()
{
    var current = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (current is not null)
    {
        if (File.Exists(Path.Combine(current.FullName, "TheSingularityWorkshop", "TheSingularityWorkshop.csproj")))
            return current.FullName;
        current = current.Parent;
    }

    throw new DirectoryNotFoundException("Could not locate the WebPage repository root.");
}
