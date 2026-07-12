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

    public async Task ReplaceListingsAsync(int appraisalId, List<SavedPropertyListing> listings, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.SavedPropertyListings
            .Where(x => x.AppraisalId == appraisalId)
            .ToListAsync(cancellationToken);

        var existingByUrl = existing
            .Where(x => !string.IsNullOrEmpty(x.Url))
            .ToDictionary(x => x.Url, StringComparer.OrdinalIgnoreCase);

        var incomingUrls = listings
            .Where(x => !string.IsNullOrEmpty(x.Url))
            .Select(x => x.Url)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var now = DateTime.UtcNow;

        foreach (var listing in listings)
        {
            listing.AppraisalId = appraisalId;

            if (!string.IsNullOrEmpty(listing.Url) && existingByUrl.TryGetValue(listing.Url, out var existingRecord))
            {
                // Update field yang sudah ada — pertahankan Id & CreatedAtUtc
                existingRecord.ImageUrl = listing.ImageUrl;
                existingRecord.Price = listing.Price;
                existingRecord.Date = listing.Date;
                existingRecord.Type = listing.Type;
                existingRecord.Title = listing.Title;
                existingRecord.Description = listing.Description;
                existingRecord.Lt = listing.Lt;
                existingRecord.Lb = listing.Lb;
                existingRecord.Kt = listing.Kt;
                existingRecord.Km = listing.Km;
                existingRecord.DetailDescription = listing.DetailDescription;
                existingRecord.Certificate = listing.Certificate;
                existingRecord.Floor = listing.Floor;
                existingRecord.Electricity = listing.Electricity;
                existingRecord.Furnished = listing.Furnished;
                existingRecord.Facing = listing.Facing;
                existingRecord.LocationText = listing.LocationText;
                existingRecord.Transaction = listing.Transaction;
                existingRecord.PropertyType = listing.PropertyType;
                existingRecord.AddressDetail = listing.AddressDetail;
                existingRecord.LocationDetail = listing.LocationDetail;
                existingRecord.GroupDetail = listing.GroupDetail;
                existingRecord.Garage = listing.Garage;
                existingRecord.ListedDate = listing.ListedDate;
                existingRecord.IdListing = listing.IdListing;
                existingRecord.ApprovalStatus = listing.ApprovalStatus;
            }
            else
            {
                // Insert baru
                listing.CreatedAtUtc = now;
                await dbContext.SavedPropertyListings.AddAsync(listing, cancellationToken);
            }
        }

        // Hapus record yang ada di DB tapi tidak ada di incoming
        var toDelete = existing.Where(x => !incomingUrls.Contains(x.Url)).ToList();
        if (toDelete.Count > 0)
            dbContext.SavedPropertyListings.RemoveRange(toDelete);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<SavedPropertyListing>> GetListingsAsync(int appraisalId, CancellationToken cancellationToken = default)
    {
        return dbContext.SavedPropertyListings
            .Where(x => x.AppraisalId == appraisalId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateListingApprovalAsync(int listingId, ListingApprovalStatus status, CancellationToken cancellationToken = default)
    {
        var listing = await dbContext.SavedPropertyListings
            .FirstOrDefaultAsync(x => x.Id == listingId, cancellationToken);
        if (listing is null) return;

        listing.ApprovalStatus = status;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveOcrResultAsync(int appraisalId, OcrResult ocr, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.OcrResults
            .FirstOrDefaultAsync(x => x.AppraisalId == appraisalId, cancellationToken);

        if (existing is not null)
        {
            existing.Province = ocr.Province;
            existing.City = ocr.City;
            existing.District = ocr.District;
            existing.SubDistrict = ocr.SubDistrict;
            existing.JenisSertifikat = ocr.JenisSertifikat;
            existing.NomorSertifikat = ocr.NomorSertifikat;
            existing.NamaPemegang = ocr.NamaPemegang;
            existing.Nib = ocr.Nib;
            existing.LuasTanah = ocr.LuasTanah;
            existing.LuasBangunan = ocr.LuasBangunan;
        }
        else
        {
            ocr.AppraisalId = appraisalId;
            await dbContext.OcrResults.AddAsync(ocr, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<OcrResult?> GetOcrResultAsync(int appraisalId, CancellationToken cancellationToken = default)
    {
        return dbContext.OcrResults
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AppraisalId == appraisalId, cancellationToken);
    }
}