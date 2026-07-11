using AppraisalSystem.Application.Common;
using AppraisalSystem.Application.Dtos;
using AppraisalSystem.Application.Interfaces;
using AppraisalSystem.Domain.Entities;
using AppraisalSystem.Domain.Enums;
using AppraisalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppraisalSystem.Infrastructure.Repositories;

public sealed class AppraisalRepository(AppraisalDbContext dbContext) : IAppraisalRepository
{
    public async Task<PagedResult<Appraisal>> GetPagedAsync(AppraisalQueryDto query, CancellationToken cancellationToken = default)
    {
        var normalizedPageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
        var normalizedPageSize = query.PageSize <= 0 ? 10 : query.PageSize;

        IQueryable<Appraisal> baseQuery = dbContext.Appraisals.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            baseQuery = baseQuery.Where(x =>
                x.ApplicationNumber.ToLower().Contains(search) ||
                x.DebtorName.ToLower().Contains(search) ||
                x.IdentityNumber.ToLower().Contains(search) ||
                x.DeedNumber.ToLower().Contains(search) ||
                x.Location.ToLower().Contains(search));
        }

        if (query.Status.HasValue)
        {
            baseQuery = baseQuery.Where(x => x.Status == query.Status.Value);
        }

        if (query.ApplicantType.HasValue)
        {
            baseQuery = baseQuery.Where(x => x.ApplicantType == query.ApplicantType.Value);
        }

        var totalItems = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((normalizedPageNumber - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Appraisal>
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = normalizedPageNumber,
            PageSize = normalizedPageSize
        };
    }

    public Task<Appraisal?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Appraisals.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Appraisal?> GetByPublicIdAsync(string publicId, CancellationToken cancellationToken = default)
    {
        return dbContext.Appraisals.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<int> CreateAsync(Appraisal appraisal, CancellationToken cancellationToken = default)
    {
        dbContext.Appraisals.Add(appraisal);
        await dbContext.SaveChangesAsync(cancellationToken);
        return appraisal.Id;
    }

    public async Task UpdateAsync(Appraisal appraisal, CancellationToken cancellationToken = default)
    {
        dbContext.Appraisals.Update(appraisal);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var total = await dbContext.Appraisals.CountAsync(cancellationToken);
        var pending = await dbContext.Appraisals.CountAsync(x => x.Status == AppraisalStatus.Submitted, cancellationToken);
        var approved = await dbContext.Appraisals.CountAsync(x => x.Status == AppraisalStatus.Approved, cancellationToken);
        var rejected = await dbContext.Appraisals.CountAsync(x => x.Status == AppraisalStatus.Rejected, cancellationToken);

        return new DashboardSummaryDto
        {
            Total = total,
            Pending = pending,
            Approved = approved,
            Rejected = rejected
        };
    }

    public async Task SetStatusAsync(int id, AppraisalStatus status, string? supervisorNote, CancellationToken cancellationToken = default)
    {
        var appraisal = await dbContext.Appraisals.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Appraisal not found.");

        appraisal.Status = status;
        appraisal.SupervisorNote = supervisorNote;
        appraisal.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}