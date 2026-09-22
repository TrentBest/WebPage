using System;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Live editing session for the Workshop model presented by the AEC Remote Office.
/// The session deliberately separates model movement from authored map geometry.
/// </summary>
public sealed class SpatialAECModelSession
{
    public SpatialAECModelSession(SpatialWorkshopModel model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        BuildingTransform = new SpatialPoint(0, 0);
        VisitorTransform = new SpatialPoint(0, 0);
    }

    public SpatialWorkshopModel Model { get; }

    /// <summary>
    /// Translation applied to the entire Workshop model in the AEC presentation.
    /// </summary>
    public SpatialPoint BuildingTransform { get; private set; }

    /// <summary>
    /// Translation applied to the visitor when the visitor is inhabiting the
    /// structure being moved. This preserves the visitor's relationship to the
    /// building instead of leaving the avatar behind.
    /// </summary>
    public SpatialPoint VisitorTransform { get; private set; }

    public SpatialPoint MoveWorkshop(double deltaX, double deltaY, bool visitorIsInside)
    {
        BuildingTransform = new SpatialPoint(
            BuildingTransform.X + deltaX,
            BuildingTransform.Y + deltaY);

        if (visitorIsInside)
        {
            VisitorTransform = new SpatialPoint(
                VisitorTransform.X + deltaX,
                VisitorTransform.Y + deltaY);
        }

        return BuildingTransform;
    }

    public SpatialPoint WorldPosition(SpatialPoint modelPosition)
        => new(
            modelPosition.X + BuildingTransform.X,
            modelPosition.Y + BuildingTransform.Y);

    public SpatialPoint VisitorWorldPosition(SpatialPoint localPosition)
        => new(
            localPosition.X + VisitorTransform.X,
            localPosition.Y + VisitorTransform.Y);

    public void Reset()
    {
        BuildingTransform = new SpatialPoint(0, 0);
        VisitorTransform = new SpatialPoint(0, 0);
    }
}
