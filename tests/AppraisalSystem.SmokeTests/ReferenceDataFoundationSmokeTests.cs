using AppraisalSystem.Application.Common;
using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.SmokeTests;

public sealed class ReferenceDataFoundationSmokeTests
{
    [Fact]
    public void AppRoles_ExposeStableConstants()
    {
        Assert.Equal("Admin", AppRoles.Admin);
        Assert.Equal("Appraiser", AppRoles.Appraiser);
        Assert.Equal("Supervisor", AppRoles.Supervisor);
        Assert.Equal("Checker", AppRoles.Checker);
        Assert.Equal("Appraiser,Admin", AppRoles.AppraiserOrAdmin);
        Assert.Equal("Supervisor,Checker", AppRoles.SupervisorOrChecker);
    }

    [Theory]
    [InlineData(ApplicantType.Individual, "Perorangan")]
    [InlineData(ApplicantType.Business, "Badan Usaha")]
    [InlineData(ApplicantType.Unknown, "-")]
    public void ApplicantType_LabelIsConsistent(ApplicantType value, string expected)
    {
        Assert.Equal(expected, value.GetLabel());
    }

    [Theory]
    [InlineData(CollateralType.Property, "Properti")]
    [InlineData(CollateralType.Vehicle, "Kendaraan")]
    [InlineData(CollateralType.Machine, "Mesin")]
    [InlineData(CollateralType.Inventory, "Inventori")]
    [InlineData(CollateralType.Other, "Lain-lain")]
    public void CollateralType_LabelIsConsistent(CollateralType value, string expected)
    {
        Assert.Equal(expected, value.GetLabel());
    }

    [Fact]
    public void AppraisalStatus_LabelsCoverAllValues()
    {
        foreach (var status in Enum.GetValues<AppraisalStatus>())
        {
            var label = status.GetLabel();
            Assert.False(string.IsNullOrWhiteSpace(label));
        }
    }
}
