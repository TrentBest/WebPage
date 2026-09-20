using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialAECBuildingTypeFactoryTests
{
    [Fact(DisplayName = "AEC ontology mall is derived from canonical building specifications")]
    public void Factory_DerivesBuildingTypesFromCanonicalSpecifications()
    {
        var specifications = SpatialBuildingSpecificationCatalog.All;

        var profiles = specifications.Select(specification =>
            SpatialAECBuildingTypeFactory.StandardProfile(
                specification,
                specification.Purpose switch
                {
                    SpatialBuildingPurpose.CivicGovernment => "CIVIC",
                    SpatialBuildingPurpose.Education => "BUILT ENVIRONMENT",
                    SpatialBuildingPurpose.Industrial => "INDUSTRIAL",
                    SpatialBuildingPurpose.Medical => "CIVIC",
                    SpatialBuildingPurpose.Office => "BUILT ENVIRONMENT",
                    SpatialBuildingPurpose.Retail => "BUILT ENVIRONMENT",
                    SpatialBuildingPurpose.ResidentialMultifamily => "RESIDENTIAL",
                    SpatialBuildingPurpose.ResidentialLuxury => "RESIDENTIAL",
                    _ => "BUILT ENVIRONMENT"
                },
                specification.Purpose switch
                {
                    SpatialBuildingPurpose.CivicGovernment => "GOVERNMENT",
                    SpatialBuildingPurpose.Education => "EDUCATION",
                    SpatialBuildingPurpose.Industrial => "FACTORY",
                    SpatialBuildingPurpose.Medical => "HOSPITAL",
                    SpatialBuildingPurpose.Office => "OFFICE",
                    SpatialBuildingPurpose.Retail => "RETAIL",
                    SpatialBuildingPurpose.ResidentialMultifamily => "RESIDENTIAL",
                    SpatialBuildingPurpose.ResidentialLuxury => "RESIDENTIAL",
                    _ => "FACILITY"
                },
                specification.Purpose switch
                {
                    SpatialBuildingPurpose.CivicGovernment => "CIVIC",
                    SpatialBuildingPurpose.Education => "SCIENCE",
                    SpatialBuildingPurpose.Industrial => "ENGINEERING",
                    SpatialBuildingPurpose.Medical => "MEDICAL",
                    SpatialBuildingPurpose.Office => "ENGINEERING",
                    SpatialBuildingPurpose.Retail => "COMMERCE",
                    SpatialBuildingPurpose.ResidentialMultifamily => "RESIDENTIAL",
                    SpatialBuildingPurpose.ResidentialLuxury => "RESIDENTIAL",
                    _ => "ENGINEERING"
                }))
            .ToArray();

        var types = SpatialAECBuildingTypeFactory.Create(specifications, profiles);

        Assert.Equal(specifications.Count, types.Count);
        Assert.Contains(types, x => x.Id == "office.corporate" && x.Name == "Corporate Office");
        Assert.Contains(types, x => x.Id == "medical.hospital" && x.Phylum == "HOSPITAL");
        Assert.All(types, type => Assert.Equal(9, type.Layers.Count));
    }

    [Fact(DisplayName = "AEC ontology mall shopping only exposes concrete inventory")]
    public void Catalog_OptionsAreBackedByConcreteInventory()
    {
        var answers = new string?[9];
        var options = SpatialAECOntologyCatalog.OptionsForLayer(1, answers);

        Assert.NotEmpty(options);
        Assert.All(options, option =>
            Assert.Contains(
                SpatialAECOntologyCatalog.ListBuildingTypes(),
                type => type.Domain.Equals(option, System.StringComparison.OrdinalIgnoreCase)));
    }
}
