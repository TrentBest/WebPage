using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class TransitTrainMicroBundleTests
{
    [Fact(DisplayName = "Incremental Unit Test 73 — train MicroBundle carries HO sandbox dimensions and consist data")]
    public void TrainBundleCarriesModelData()
    {
        var specification = TransitTrainSpecification.WorkshopHoEmu();

        Assert.Equal("workshop-ho-emu", specification.Id);
        Assert.Equal(8, specification.CarCount);
        Assert.Equal(8, specification.Cars.Count);
        Assert.Equal(87.1, TransitTrainSpecification.HoScaleRatio);
        Assert.Equal(16.54, TransitTrainSpecification.HoTrackGaugeMillimeters);
        Assert.True(specification.ModelCarLengthMillimeters > 0);
        Assert.Contains("camera-view", specification.Capabilities);
        Assert.Contains("museum-display", specification.Capabilities);
    }

    [Fact(DisplayName = "Incremental Unit Test 74 — train MicroBundle exposes operational states")]
    public void TrainBundleExposesOperationalStates()
    {
        using var train = new TransitTrainMicroBundle(7301, TransitTrainSpecification.WorkshopHoEmu());

        Assert.Equal(TransitTrainOperatingState.Stabled, train.State);
        train.SetState(TransitTrainOperatingState.Boarding);
        train.SetRouteProgress(.5);
        Assert.Equal(TransitTrainOperatingState.Boarding, train.State);
        Assert.Equal(.5, train.RouteProgress);
    }
}
