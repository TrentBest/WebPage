using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.City;

/// <summary>
/// A generalized organization in the simulated world. An architectural firm is one
/// industry profile, not a special-case simulation entity.
/// </summary>
/// <remarks>
/// This is world substrate only: membership, work, and produced artifacts are modeled
/// without prescribing compensation, ownership law, fees, or a marketplace policy.
/// Stable Digiten IDs connect people to the compact population model.
/// </remarks>
public sealed class SingularityOrganization
{
    private readonly Dictionary<ulong, OrganizationMember> _members = new();
    private readonly Dictionary<string, OrganizationProject> _projects = new(StringComparer.Ordinal);

    public SingularityOrganization(string id, string name, string industry, string? buildingId = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("An organization ID is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("An organization name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(industry)) throw new ArgumentException("An industry profile is required.", nameof(industry));

        Id = id;
        Name = name;
        Industry = industry;
        BuildingId = buildingId;
    }

    public string Id { get; }
    public string Name { get; }
    public string Industry { get; }
    public string? BuildingId { get; private set; }
    public IReadOnlyCollection<OrganizationMember> Members => _members.Values;
    public IReadOnlyCollection<OrganizationProject> Projects => _projects.Values;

    public void LocateAt(string? buildingId)
    {
        if (buildingId is not null && string.IsNullOrWhiteSpace(buildingId))
            throw new ArgumentException("A building ID must be non-empty or null.", nameof(buildingId));
        BuildingId = buildingId;
    }

    public void AddMember(OrganizationMember member)
    {
        ArgumentNullException.ThrowIfNull(member);
        if (member.DigitenId == 0) throw new ArgumentOutOfRangeException(nameof(member), "A member requires a stable Digiten ID.");
        if (string.IsNullOrWhiteSpace(member.Role)) throw new ArgumentException("A member role is required.", nameof(member));
        _members.Add(member.DigitenId, member);
    }

    public bool RemoveMember(ulong digitenId) => _members.Remove(digitenId);

    public void AddProject(OrganizationProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        if (string.IsNullOrWhiteSpace(project.Id)) throw new ArgumentException("A project ID is required.", nameof(project));
        if (string.IsNullOrWhiteSpace(project.Name)) throw new ArgumentException("A project name is required.", nameof(project));
        if (string.IsNullOrWhiteSpace(project.ArtifactKind)) throw new ArgumentException("An artifact kind is required.", nameof(project));
        _projects.Add(project.Id, project);
    }

    public bool TryGetMember(ulong digitenId, out OrganizationMember? member) =>
        _members.TryGetValue(digitenId, out member);

    public bool TryGetProject(string id, out OrganizationProject? project) =>
        _projects.TryGetValue(id, out project);
}

/// <summary>A Digiten's role in an organization; the role name remains domain-extensible.</summary>
public sealed record OrganizationMember(ulong DigitenId, string Role, string Status = "Active");

/// <summary>
/// Work that may produce a reusable artifact. ArtifactKind and ArtifactId allow the
/// same organization model to support buildings, tools, services, research, or other content.
/// </summary>
public sealed record OrganizationProject(
    string Id,
    string Name,
    string ArtifactKind,
    string? ArtifactId = null,
    string Status = "Proposed");
