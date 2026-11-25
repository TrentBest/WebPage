#if WORKSHOP_USE_REPOSITORY_SOURCES
namespace TheSingularityWorkshop.Gui;

using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Projects the Laboratory-owned campus overview into WebPage's platform-neutral GUI tree.
/// WebPage supplies presentation only; the source campus remains authoritative for all
/// laboratory, facility, room, apparatus, instrument and measurement-channel identities.
/// </summary>
public static class LaboratoryCampusOverviewGuiBuilder
{
    /// <summary>Builds a semantic view of a campus without inventing host geometry or access policy.</summary>
    public static GuiNode Build(LaboratoryCampusOverview overview)
    {
        ArgumentNullException.ThrowIfNull(overview);

        var root = GuiBuilder.Create("panel", "laboratory-campus-overview")
            .Text(overview.Name)
            .Property("data-campus-id", overview.Id)
            .Property("data-campus-version", overview.Version);

        foreach (var workspace in overview.Workspaces)
        {
            root.Child("panel", workspace.Id, workspaceNode =>
            {
                workspaceNode
                    .Text(workspace.LaboratoryName)
                    .Property("data-workspace-id", workspace.Id)
                    .Property("data-laboratory-id", workspace.LaboratoryId)
                    .Property("data-facility-id", workspace.FacilityId)
                    .Property("data-room-id", workspace.RoomId);

                workspaceNode.Child("text", workspace.Id + "-description",
                    text => text.Text(workspace.LaboratoryDescription));
                workspaceNode.Child("text", workspace.Id + "-facility",
                    text => text.Text(workspace.FacilityName));
                workspaceNode.Child("text", workspace.Id + "-room",
                    text => text.Text(workspace.RoomName + " — " + workspace.RoomPurpose));

                foreach (var apparatus in workspace.Apparatus)
                {
                    workspaceNode.Child("panel", apparatus.Id, apparatusNode =>
                    {
                        apparatusNode
                            .Text(apparatus.Name)
                            .Property("data-apparatus-id", apparatus.Id)
                            .Property("data-apparatus-type", apparatus.ApparatusType);
                        apparatusNode.Child("text", apparatus.Id + "-description",
                            text => text.Text(apparatus.Description));
                    });
                }

                foreach (var instrument in workspace.Instruments)
                {
                    workspaceNode.Child("panel", instrument.Id, instrumentNode =>
                    {
                        instrumentNode
                            .Text(instrument.Name)
                            .Property("data-instrument-id", instrument.Id)
                            .Property("data-instrument-type", instrument.InstrumentType)
                            .Property("data-instrument-version", instrument.InstrumentVersion)
                            .Property("data-calibration-reference", instrument.CalibrationReference);

                        foreach (var channel in instrument.MeasurementChannels)
                        {
                            instrumentNode.Child("text", channel.Id, channelNode =>
                            {
                                channelNode
                                    .Text(channel.Name + " — " + channel.Quantity + " (" + channel.UnitSymbol + ")")
                                    .Property("data-channel-id", channel.Id)
                                    .Property("data-unit", channel.UnitSymbol)
                                    .Property("data-sampling-mode", channel.SamplingMode.ToString());
                                if (channel.SamplesPerSecond is not null)
                                    channelNode.Property("data-samples-per-second",
                                        channel.SamplesPerSecond.Value.ToString(
                                            System.Globalization.CultureInfo.InvariantCulture));
                            });
                        }
                    });
                }
            });
        }

        return root.Build();
    }
}
#endif
