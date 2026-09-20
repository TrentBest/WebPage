namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>Diegetic laboratory traffic: employees and guards move through real security boundaries on shifts.</summary>
public sealed class SpatialLaboratoryTrafficModel
{
    public IReadOnlyList<SpatialLaboratoryEmployee> Employees { get; } =
    [
        new("research-13", "Research #13", SpatialSecurityClearance.Research, "RESEARCH", new TimeOnly(6, 0), new TimeOnly(18, 0)),
        new("administrator-02", "Administrator #2", SpatialSecurityClearance.Administrator, "ADMINISTRATION", new TimeOnly(8, 0), new TimeOnly(17, 0)),
        new("research-27", "Research #27", SpatialSecurityClearance.Research, "RESEARCH", new TimeOnly(10, 0), new TimeOnly(22, 0)),
        new("facility-head", "Facility Head", SpatialSecurityClearance.FacilityHead, "FACILITY HEAD", new TimeOnly(7, 0), new TimeOnly(19, 0))
    ];

    public IReadOnlyList<SpatialLaboratoryGuardShift> Guards { get; } =
    [
        new("guard-01", "Guard 01", "ELEVATOR LEFT", new TimeOnly(6, 0), new TimeOnly(14, 0)),
        new("guard-02", "Guard 02", "ELEVATOR RIGHT", new TimeOnly(6, 0), new TimeOnly(14, 0)),
        new("guard-03", "Guard 03", "SECURITY LEFT", new TimeOnly(14, 0), new TimeOnly(22, 0)),
        new("guard-04", "Guard 04", "SECURITY RIGHT", new TimeOnly(14, 0), new TimeOnly(22, 0)),
        new("guard-05", "Guard 05", "DIRECTOR LEFT", new TimeOnly(22, 0), new TimeOnly(6, 0)),
        new("guard-06", "Guard 06", "DIRECTOR RIGHT", new TimeOnly(22, 0), new TimeOnly(6, 0))
    ];

    public IReadOnlyList<SpatialLaboratoryEmployee> OnDutyEmployees(TimeOnly time)
        => Employees.Where(employee => IsWithinShift(time, employee.Start, employee.End)).ToArray();

    public IReadOnlyList<SpatialLaboratoryGuardShift> OnDutyGuards(TimeOnly time)
        => Guards.Where(guard => IsWithinShift(time, guard.Start, guard.End)).ToArray();

    private static bool IsWithinShift(TimeOnly time, TimeOnly start, TimeOnly end)
        => start <= end ? time >= start && time < end : time >= start || time < end;
}

public readonly record struct SpatialLaboratoryEmployee(
    string Id,
    string Name,
    SpatialSecurityClearance Clearance,
    string Destination,
    TimeOnly Start,
    TimeOnly End);

public readonly record struct SpatialLaboratoryGuardShift(
    string Id,
    string Name,
    string Post,
    TimeOnly Start,
    TimeOnly End);
