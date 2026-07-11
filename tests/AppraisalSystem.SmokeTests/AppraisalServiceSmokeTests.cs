using AppraisalSystem.Application.Common;
using AppraisalSystem.Application.Dtos;
using AppraisalSystem.Application.Interfaces;
using AppraisalSystem.Application.Services;
using AppraisalSystem.Domain.Entities;
using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.SmokeTests;

public sealed class AppraisalServiceSmokeTests
{
    [Fact]
    public async Task GetPagedAsync_MapsResultAndKeepsPaginationMetadata()
    {
        var seeded = new Appraisal
        {
            Id = 7,
            ApplicationNumber = "APP-20260710-0007",
            Segment = CustomerSegment.Corporate,
            ApplicantType = ApplicantType.Business,
            DebtorName = "PT Maju Properti",
            IdentityNumber = "ID-0001",
            DeedNumber = "AKTA-01",
            DateOfBirth = null,
            BusinessEstablishedDate = new DateTime(2018, 5, 12),
            CollateralType = CollateralType.Property,
            Status = AppraisalStatus.Submitted,
            CreatedAtUtc = new DateTime(2026, 7, 10, 7, 0, 0, DateTimeKind.Utc)
        };

        var repository = new FakeRepository
        {
            PagedResult = new PagedResult<Appraisal>
            {
                Items = new[] { seeded },
                TotalItems = 31,
                PageNumber = 2,
                PageSize = 10
            }
        };

        var service = new AppraisalService(repository);

        var result = await service.GetPagedAsync(new AppraisalQueryDto { PageNumber = 2, PageSize = 10 });

        Assert.Single(result.Items);
        var item = Assert.Single(result.Items);
        Assert.Equal(7, item.Id);
        Assert.Equal("APP-20260710-0007", item.ApplicationNumber);
        Assert.Equal("Corporate", item.Segment);
        Assert.Equal("Business", item.ApplicantType);
        Assert.Equal("Submitted", item.Status);
        Assert.Equal(31, result.TotalItems);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task GetPagedAsync_PassesQueryToRepository()
    {
        var repository = new FakeRepository();
        var service = new AppraisalService(repository);

        var query = new AppraisalQueryDto
        {
            Search = "ahmad",
            Status = AppraisalStatus.Draft,
            ApplicantType = ApplicantType.Business,
            PageNumber = 3,
            PageSize = 25
        };

        await service.GetPagedAsync(query);

        Assert.NotNull(repository.LastQuery);
        Assert.Equal("ahmad", repository.LastQuery!.Search);
        Assert.Equal(AppraisalStatus.Draft, repository.LastQuery.Status);
        Assert.Equal(ApplicantType.Business, repository.LastQuery.ApplicantType);
        Assert.Equal(3, repository.LastQuery.PageNumber);
        Assert.Equal(25, repository.LastQuery.PageSize);
    }

    [Fact]
    public async Task UpdateAsync_WhenExistingStatusRejected_ResetsStatusToDraft()
    {
        var existing = new Appraisal
        {
            Id = 15,
            ApplicationNumber = "APP-20260711-0015",
            Segment = CustomerSegment.Consumer,
            ApplicantType = ApplicantType.Individual,
            Status = AppraisalStatus.Rejected,
            SupervisorNote = "Data belum lengkap",
            CreatedBy = "appraiser"
        };

        var repository = new FakeRepository();
        repository.Entities[existing.Id] = existing;
        var service = new AppraisalService(repository);

        await service.UpdateAsync(new AppraisalUpsertDto
        {
            Id = existing.Id,
            ApplicationNumber = "APP-20260711-0015",
            Segment = CustomerSegment.Consumer,
            ApplicantType = ApplicantType.Individual,
            MakerId = "MKR01",
            BranchCode = "001",
            DebtorName = "Budi",
            IdentityNumber = "ID-15",
            CollateralType = CollateralType.Property,
            CollateralSubtype = "Rumah",
            Location = "Jakarta",
            Specification = "Luas 100m2",
            MarketValue = 1000000000,
            LiquidationValue = 800000000,
            Notes = "Revisi data setelah reject"
        });

        Assert.Equal(AppraisalStatus.Draft, existing.Status);
        Assert.Null(existing.SupervisorNote);
    }

    [Fact]
    public async Task SubmitAsync_WhenStatusNotDraft_ThrowsInvalidOperationException()
    {
        var existing = new Appraisal
        {
            Id = 22,
            ApplicationNumber = "APP-20260711-0022",
            Segment = CustomerSegment.Consumer,
            ApplicantType = ApplicantType.Individual,
            Status = AppraisalStatus.Rejected,
            CreatedBy = "appraiser"
        };

        var repository = new FakeRepository();
        repository.Entities[existing.Id] = existing;
        var service = new AppraisalService(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SubmitAsync(existing.Id));
    }

    private sealed class FakeRepository : IAppraisalRepository
    {
        public AppraisalQueryDto? LastQuery { get; private set; }
        public Dictionary<int, Appraisal> Entities { get; } = new();

        public PagedResult<Appraisal> PagedResult { get; set; } = new()
        {
            Items = Array.Empty<Appraisal>(),
            TotalItems = 0,
            PageNumber = 1,
            PageSize = 10
        };

        public Task<PagedResult<Appraisal>> GetPagedAsync(AppraisalQueryDto query, CancellationToken cancellationToken = default)
        {
            LastQuery = query;
            return Task.FromResult(PagedResult);
        }

        public Task<Appraisal?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(Entities.TryGetValue(id, out var appraisal) ? appraisal : null);

        public Task<int> CreateAsync(Appraisal appraisal, CancellationToken cancellationToken = default)
            => Task.FromResult(1);

        public Task UpdateAsync(Appraisal appraisal, CancellationToken cancellationToken = default)
        {
            Entities[appraisal.Id] = appraisal;
            return Task.CompletedTask;
        }

        public Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new DashboardSummaryDto());

        public Task SetStatusAsync(int id, AppraisalStatus status, string? supervisorNote, CancellationToken cancellationToken = default)
        {
            if (Entities.TryGetValue(id, out var appraisal))
            {
                appraisal.Status = status;
                appraisal.SupervisorNote = supervisorNote;
            }

            return Task.CompletedTask;
        }
    }
}
