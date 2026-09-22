using System;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Presentation-neutral description of the "god hand" interaction planned for VR.
/// Rendering belongs to the client; this model supplies the spatial relationship
/// between the controller hit, the hand, and the visitor avatar.
/// </summary>
public sealed record SpatialVrGodInteraction(
    string TargetId,
    SpatialPoint TargetPoint,
    SpatialPoint HandPoint,
    SpatialPoint AvatarPoint,
    bool AvatarAboveTarget)
{
    public static SpatialVrGodInteraction Create(
        string targetId,
        SpatialPoint targetPoint,
        SpatialPoint handPoint,
        SpatialPoint avatarPoint)
    {
        if (string.IsNullOrWhiteSpace(targetId))
            throw new ArgumentException("A target id is required.", nameof(targetId));

        return new SpatialVrGodInteraction(
            targetId,
            targetPoint,
            handPoint,
            avatarPoint,
            avatarPoint.Y < targetPoint.Y);
    }
}
