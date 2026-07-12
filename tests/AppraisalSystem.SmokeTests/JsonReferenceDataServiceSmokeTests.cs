using AppraisalSystem.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;

namespace AppraisalSystem.SmokeTests;

public sealed class JsonReferenceDataServiceSmokeTests
{
    private static JsonReferenceDataService CreateSut()
    {
        var wwwroot = LocateWebRoot();
        var cache = new MemoryCache(new MemoryCacheOptions());
        return new JsonReferenceDataService(new StubEnvironment(wwwroot), cache, NullLogger<JsonReferenceDataService>.Instance);
    }

    [Fact]
    public async Task GetGendersAsync_ReturnsBahasaIndonesiaLabels()
    {
        var sut = CreateSut();

        var result = await sut.GetGendersAsync();

        Assert.Contains(result, x => x.Code == "Laki-laki" && x.Label == "Laki-laki");
        Assert.Contains(result, x => x.Code == "Perempuan" && x.Label == "Perempuan");
    }

    [Fact]
    public async Task GetMaritalStatusesAsync_ExposesFourValues()
    {
        var sut = CreateSut();

        var result = await sut.GetMaritalStatusesAsync();

        Assert.Equal(4, result.Count);
        Assert.Contains(result, x => x.Label == "Belum Menikah");
        Assert.Contains(result, x => x.Label == "Menikah");
        Assert.Contains(result, x => x.Label == "Cerai Hidup");
        Assert.Contains(result, x => x.Label == "Cerai Mati");
    }

    [Fact]
    public async Task GetBranchesAsync_UsesCanonicalCodesAndPreservesDisplay()
    {
        var sut = CreateSut();

        var result = await sut.GetBranchesAsync();

        Assert.NotEmpty(result);
        var head = result.First(x => x.Code == "001");
        Assert.Equal("Kantor Pusat Jakarta", head.Label);
        Assert.Equal("001 - Kantor Pusat Jakarta", head.Display);
    }

    [Fact]
    public async Task GetProvincesAsync_ContainsCommonSeedValues()
    {
        var sut = CreateSut();

        var result = await sut.GetProvincesAsync();

        Assert.Contains(result, x => x.Label == "DKI Jakarta");
        Assert.Contains(result, x => x.Label == "Jawa Barat");
        Assert.Contains(result, x => x.Label == "Jawa Timur");
    }

    [Fact]
    public async Task GetCollateralSubtypesAsync_ContainsSeedValues()
    {
        var sut = CreateSut();

        var result = await sut.GetCollateralSubtypesAsync();

        Assert.Contains(result, x => x.Label == "Ruko");
        Assert.Contains(result, x => x.Label == "Gudang");
        Assert.Contains(result, x => x.Label == "Lain-lain");
    }

    [Fact]
    public async Task GetCitiesAsync_FiltersByProvinceParent()
    {
        var sut = CreateSut();

        var jakarta = await sut.GetCitiesAsync("DKI Jakarta");
        var all = await sut.GetCitiesAsync();

        Assert.NotEmpty(jakarta);
        Assert.All(jakarta, x => Assert.Equal("DKI Jakarta", x.ParentCode));
        Assert.Contains(jakarta, x => x.Label == "Jakarta Selatan");
        Assert.True(all.Count >= jakarta.Count);
    }

    [Fact]
    public async Task GetDistrictsAsync_FiltersByCityParent()
    {
        var sut = CreateSut();

        var districts = await sut.GetDistrictsAsync("Jakarta Selatan");

        Assert.NotEmpty(districts);
        Assert.All(districts, x => Assert.Equal("Jakarta Selatan", x.ParentCode));
        Assert.Contains(districts, x => x.Label == "Tebet");
    }

    [Fact]
    public async Task GetSubdistrictsAsync_FiltersByDistrictParent()
    {
        var sut = CreateSut();

        var kelurahan = await sut.GetSubdistrictsAsync("Tebet");

        Assert.NotEmpty(kelurahan);
        Assert.All(kelurahan, x => Assert.Equal("Tebet", x.ParentCode));
        Assert.Contains(kelurahan, x => x.Label == "Tebet Barat");
    }

    private static string LocateWebRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "src", "AppraisalSystem.Web", "wwwroot");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Cannot locate AppraisalSystem.Web/wwwroot from test base directory.");
    }

    private sealed class StubEnvironment : IWebHostEnvironment
    {
        public StubEnvironment(string webRootPath)
        {
            WebRootPath = webRootPath;
            WebRootFileProvider = new PhysicalFileProvider(webRootPath);
            ContentRootPath = webRootPath;
            ContentRootFileProvider = new PhysicalFileProvider(webRootPath);
        }

        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ApplicationName { get; set; } = "AppraisalSystem.Web";
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; } = "Development";
    }
}
