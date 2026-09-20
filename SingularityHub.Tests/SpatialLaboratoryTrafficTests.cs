using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialLaboratoryTrafficTests
{
    [Fact]
    public void LaboratoryTrafficContainsResearchAndAdministrativeEmployees()
    {
        var traffic = new SpatialLaboratoryTrafficModel();

        Assert.Contains(traffic.Employees, employee => employee.Name == "Research #13");
        Assert.Contains(traffic.Employees, employee => employee.Name == "Administrator #2");
        Assert.Contains(traffic.Guards, guard => guard.Post == "ELEVATOR LEFT");
        Assert.Contains(traffic.Guards, guard => guard.Post == "DIRECTOR LEFT");
    }

    [Fact]
    public void OvernightGuardShiftCrossesMidnight()
    {
        var traffic = new SpatialLaboratoryTrafficModel();

        Assert.Contains(traffic.OnDutyGuards(new TimeOnly(23, 30)), guard => guard.Name == "Guard 05");
        Assert.Contains(traffic.OnDutyGuards(new TimeOnly(5, 30)), guard => guard.Name == "Guard 05");
        Assert.DoesNotContain(traffic.OnDutyGuards(new TimeOnly(13, 0)), guard => guard.Name == "Guard 05");
    }
}
