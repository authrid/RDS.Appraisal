using AppraisalSystem.Application.Common;
using AppraisalSystem.Application.Dtos;
using AppraisalSystem.Application.Features.ChatUI;
using AppraisalSystem.Application.Interfaces;
using AppraisalSystem.Domain.Entities;
using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Application.Services;

public sealed class AppraisalService(IAppraisalRepository repository) : IAppraisalService
{
    public async Task<PagedResult<AppraisalListItemDto>> GetPagedAsync(AppraisalQueryDto query, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetPagedAsync(query, cancellationToken);

        return new PagedResult<AppraisalListItemDto>
        {
            Items = result.Items.Select(MapListItem).ToList(),
            TotalItems = result.TotalItems,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<AppraisalDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var appraisal = await repository.GetByIdAsync(id, cancellationToken);
        return appraisal is null ? null : MapDetail(appraisal);
    }

    public async Task<AppraisalDetailDto?> GetByPublicIdAsync(string publicId, CancellationToken cancellationToken = default)
    {
        var normalizedPublicId = (publicId ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedPublicId))
        {
            return null;
        }

        var appraisal = await repository.GetByPublicIdAsync(normalizedPublicId, cancellationToken);
        return appraisal is null ? null : MapDetail(appraisal);
    }

    public Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetDashboardSummaryAsync(cancellationToken);
    }

    public async Task<int> CreateAsync(AppraisalUpsertDto request, string createdBy, CancellationToken cancellationToken = default)
    {
        var appraisal = new Appraisal
        {
            ApplicationNumber = request.ApplicationNumber,
            Segment = request.Segment,
            MakerId = request.MakerId,
            BranchCode = request.BranchCode,
            ApplicationDateUtc = request.ApplicationDateUtc,
            ApplicantType = request.ApplicantType,
            DeedNumber = request.DeedNumber,
            DateOfBirth = request.DateOfBirth,
            BusinessEstablishedDate = request.BusinessEstablishedDate,
            LegalEntity = request.LegalEntity,
            BirthPlace = request.BirthPlace,
            Gender = request.Gender,
            MaritalStatus = request.MaritalStatus,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            AddressLine3 = request.AddressLine3,
            Rt = request.Rt,
            Rw = request.Rw,
            PostalCode = request.PostalCode,
            Subdistrict = request.Subdistrict,
            District = request.District,
            City = request.City,
            Province = request.Province,
            Country = request.Country,
            PhoneNumber = request.PhoneNumber,
            MobileNumber1 = request.MobileNumber1,
            MobileNumber2 = request.MobileNumber2,
            Email = request.Email,
            DebtorName = request.DebtorName,
            IdentityNumber = request.IdentityNumber,
            CollateralType = request.CollateralType,
            CollateralSubtype = request.CollateralSubtype,
            MarketValue = request.MarketValue,
            LiquidationValue = request.LiquidationValue,
            Notes = request.Notes,
            InternalMemo = request.InternalMemo,
            WorkflowHistoryJson = request.WorkflowHistoryJson,
            SavedSessionJson = request.SavedSessionJson,
            Status = AppraisalStatus.Draft,
            CreatedBy = createdBy,
            CreatedAtUtc = DateTime.UtcNow
        };

        return await repository.CreateAsync(appraisal, cancellationToken);
    }

    public async Task UpdateAsync(AppraisalUpsertDto request, CancellationToken cancellationToken = default)
    {
        if (!request.Id.HasValue)
        {
            throw new ArgumentException("Appraisal id is required for update.", nameof(request));
        }

        var appraisal = await repository.GetByIdAsync(request.Id.Value, cancellationToken)
            ?? throw new InvalidOperationException("Appraisal not found.");
        var previousStatus = appraisal.Status;

        if (previousStatus == AppraisalStatus.Approved)
        {
            throw new InvalidOperationException("Data Approved tidak dapat diedit langsung. Gunakan menu Pengkinian Data.");
        }

        appraisal.ApplicationNumber = request.ApplicationNumber;
        appraisal.Segment = request.Segment;
        appraisal.MakerId = request.MakerId;
        appraisal.BranchCode = request.BranchCode;
        appraisal.ApplicationDateUtc = request.ApplicationDateUtc;
        appraisal.ApplicantType = request.ApplicantType;
        appraisal.DeedNumber = request.DeedNumber;
        appraisal.DateOfBirth = request.DateOfBirth;
        appraisal.BusinessEstablishedDate = request.BusinessEstablishedDate;
        appraisal.LegalEntity = request.LegalEntity;
        appraisal.BirthPlace = request.BirthPlace;
        appraisal.Gender = request.Gender;
        appraisal.MaritalStatus = request.MaritalStatus;
        appraisal.AddressLine1 = request.AddressLine1;
        appraisal.AddressLine2 = request.AddressLine2;
        appraisal.AddressLine3 = request.AddressLine3;
        appraisal.Rt = request.Rt;
        appraisal.Rw = request.Rw;
        appraisal.PostalCode = request.PostalCode;
        appraisal.Subdistrict = request.Subdistrict;
        appraisal.District = request.District;
        appraisal.City = request.City;
        appraisal.Province = request.Province;
        appraisal.Country = request.Country;
        appraisal.PhoneNumber = request.PhoneNumber;
        appraisal.MobileNumber1 = request.MobileNumber1;
        appraisal.MobileNumber2 = request.MobileNumber2;
        appraisal.Email = request.Email;
        appraisal.DebtorName = request.DebtorName;
        appraisal.IdentityNumber = request.IdentityNumber;
        appraisal.CollateralType = request.CollateralType;
        appraisal.CollateralSubtype = request.CollateralSubtype;
        appraisal.MarketValue = request.MarketValue;
        appraisal.LiquidationValue = request.LiquidationValue;
        appraisal.Notes = request.Notes;
        appraisal.InternalMemo = request.InternalMemo;
        appraisal.WorkflowHistoryJson = request.WorkflowHistoryJson;
        appraisal.SavedSessionJson = request.SavedSessionJson;

        if (previousStatus == AppraisalStatus.Rejected)
        {
            // Revisi atas data reject dibuka kembali sebagai draft agar bisa diajukan ulang.
            appraisal.Status = AppraisalStatus.Draft;
            appraisal.SupervisorNote = null;
        }

        appraisal.UpdatedAtUtc = DateTime.UtcNow;

        await repository.UpdateAsync(appraisal, cancellationToken);
    }

    public async Task SubmitAsync(int id, string? actor = null, CancellationToken cancellationToken = default)
    {
        var appraisal = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Appraisal not found.");

        if (appraisal.Status != AppraisalStatus.Draft)
        {
            throw new InvalidOperationException("Hanya data berstatus Draft yang dapat disubmit.");
        }

        await AppendWorkflowEntryAsync(id, actor, "Submitted", "Submitted untuk review.", AppraisalStatus.Submitted, cancellationToken);
        await repository.SetStatusAsync(id, AppraisalStatus.Submitted, null, cancellationToken);
    }

    public async Task ApproveAsync(int id, string? supervisorNote, string? actor = null, CancellationToken cancellationToken = default)
    {
        var note = string.IsNullOrWhiteSpace(supervisorNote) ? "Approved tanpa catatan." : supervisorNote.Trim();
        await AppendWorkflowEntryAsync(id, actor, "Approved", note, AppraisalStatus.Approved, cancellationToken);
        await repository.SetStatusAsync(id, AppraisalStatus.Approved, supervisorNote, cancellationToken);
    }

    public async Task RejectAsync(int id, string? supervisorNote, string? actor = null, CancellationToken cancellationToken = default)
    {
        var note = string.IsNullOrWhiteSpace(supervisorNote) ? "Rejected tanpa catatan." : supervisorNote.Trim();
        await AppendWorkflowEntryAsync(id, actor, "Rejected", note, AppraisalStatus.Rejected, cancellationToken);
        await repository.SetStatusAsync(id, AppraisalStatus.Rejected, supervisorNote, cancellationToken);
    }

    public async Task ReopenForPengkinianAsync(int id, string? actor = null, CancellationToken cancellationToken = default)
    {
        var appraisal = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Appraisal not found.");

        if (appraisal.Status != AppraisalStatus.Approved)
        {
            throw new InvalidOperationException("Hanya data berstatus Approved yang dapat dikinikan.");
        }

        await AppendWorkflowEntryAsync(
            id,
            actor,
            "Pengkinian",
            "Data dikembalikan ke Draft untuk pengkinian.",
            AppraisalStatus.Draft,
            cancellationToken);
        await repository.SetStatusAsync(id, AppraisalStatus.Draft, null, cancellationToken);
    }

    private async Task AppendWorkflowEntryAsync(int id, string? actor, string action, string note, AppraisalStatus stageStatus, CancellationToken cancellationToken)
    {
        var appraisal = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Appraisal not found.");

        var entries = ParseWorkflowHistory(appraisal.WorkflowHistoryJson);
        entries.Add(new WorkflowEntry
        {
            TimestampUtc = DateTime.UtcNow,
            Actor = string.IsNullOrWhiteSpace(actor) ? "System User" : actor.Trim(),
            Action = action,
            Note = note,
            StageCode = AppraisalStageReference.GetCode(stageStatus),
            StageLabel = AppraisalStageReference.GetLabel(stageStatus)
        });

        appraisal.WorkflowHistoryJson = SerializeWorkflowHistory(entries);
        appraisal.UpdatedAtUtc = DateTime.UtcNow;

        await repository.UpdateAsync(appraisal, cancellationToken);
    }

    private static List<WorkflowEntry> ParseWorkflowHistory(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return [];
        }

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<WorkflowEntry>>(raw) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static string SerializeWorkflowHistory(IReadOnlyList<WorkflowEntry> entries)
        => System.Text.Json.JsonSerializer.Serialize(entries);

    private sealed class WorkflowEntry
    {
        public DateTime TimestampUtc { get; set; }
        public string Actor { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string StageCode { get; set; } = string.Empty;
        public string StageLabel { get; set; } = string.Empty;
    }

    private static AppraisalListItemDto MapListItem(Appraisal appraisal)
    {
        return new AppraisalListItemDto
        {
            Id = appraisal.Id,
            PublicId = appraisal.PublicId,
            ApplicationNumber = appraisal.ApplicationNumber,
            Segment = appraisal.Segment.ToString(),
            ApplicantType = appraisal.ApplicantType.ToString(),
            IdentityNumber = appraisal.IdentityNumber,
            DeedNumber = appraisal.DeedNumber,
            DateOfBirth = appraisal.DateOfBirth,
            BusinessEstablishedDate = appraisal.BusinessEstablishedDate,
            DebtorName = appraisal.DebtorName,
            CollateralType = appraisal.CollateralType.ToString(),
            Status = appraisal.Status.ToString(),
            StageCode = AppraisalStageReference.GetCode(appraisal.Status),
            StageLabel = AppraisalStageReference.GetDisplay(appraisal.Status),
            CreatedAtUtc = appraisal.CreatedAtUtc
        };
    }

    private static AppraisalDetailDto MapDetail(Appraisal appraisal)
    {
        return new AppraisalDetailDto
        {
            Id = appraisal.Id,
            PublicId = appraisal.PublicId,
            ApplicationNumber = appraisal.ApplicationNumber,
            Segment = appraisal.Segment,
            MakerId = appraisal.MakerId,
            BranchCode = appraisal.BranchCode,
            ApplicationDateUtc = appraisal.ApplicationDateUtc,
            ApplicantType = appraisal.ApplicantType,
            DeedNumber = appraisal.DeedNumber,
            DateOfBirth = appraisal.DateOfBirth,
            BusinessEstablishedDate = appraisal.BusinessEstablishedDate,
            LegalEntity = appraisal.LegalEntity,
            BirthPlace = appraisal.BirthPlace,
            Gender = appraisal.Gender,
            MaritalStatus = appraisal.MaritalStatus,
            AddressLine1 = appraisal.AddressLine1,
            AddressLine2 = appraisal.AddressLine2,
            AddressLine3 = appraisal.AddressLine3,
            Rt = appraisal.Rt,
            Rw = appraisal.Rw,
            PostalCode = appraisal.PostalCode,
            Subdistrict = appraisal.Subdistrict,
            District = appraisal.District,
            City = appraisal.City,
            Province = appraisal.Province,
            Country = appraisal.Country,
            PhoneNumber = appraisal.PhoneNumber,
            MobileNumber1 = appraisal.MobileNumber1,
            MobileNumber2 = appraisal.MobileNumber2,
            Email = appraisal.Email,
            DebtorName = appraisal.DebtorName,
            IdentityNumber = appraisal.IdentityNumber,
            CollateralType = appraisal.CollateralType,
            CollateralSubtype = appraisal.CollateralSubtype,
            Location = appraisal.Location,
            Specification = appraisal.Specification,
            MarketValue = appraisal.MarketValue,
            LiquidationValue = appraisal.LiquidationValue,
            Notes = appraisal.Notes,
            InternalMemo = appraisal.InternalMemo,
            WorkflowHistoryJson = appraisal.WorkflowHistoryJson,
            SavedSessionJson = appraisal.SavedSessionJson,
            Status = appraisal.Status,
            StageCode = AppraisalStageReference.GetCode(appraisal.Status),
            StageLabel = AppraisalStageReference.GetDisplay(appraisal.Status),
            CreatedBy = appraisal.CreatedBy,
            CreatedAtUtc = appraisal.CreatedAtUtc,
            UpdatedAtUtc = appraisal.UpdatedAtUtc,
            SupervisorNote = appraisal.SupervisorNote
        };
    }

    public async Task SaveListingsAsync(int appraisalId, List<PropertyListing> listings, CancellationToken cancellationToken = default)
    {
        var entities = listings.Select(p => new SavedPropertyListing
        {
            Url = p.Url ?? string.Empty,
            ImageUrl = p.ImageUrl,
            Price = p.Price,
            Date = p.Date,
            Type = p.Type,
            Title = p.Title,
            Description = p.Description,
            Lt = p.Lt,
            Lb = p.Lb,
            Kt = p.Kt,
            Km = p.Km,
            DetailDescription = p.DetailDescription,
            Certificate = p.Certificate,
            Floor = p.Floor,
            Electricity = p.Electricity,
            Furnished = p.Furnished,
            Facing = p.Facing,
            LocationText = p.LocationText,
            Transaction = p.Transaction,
            PropertyType = p.PropertyType,
            AddressDetail = p.AddressDetail,
            LocationDetail = p.LocationDetail,
            GroupDetail = p.GroupDetail,
            Garage = p.Garage,
            ListedDate = p.ListedDate,
            IdListing = p.IdListing,
            ApprovalStatus = p.ApprovalStatus,
        }).ToList();

        await repository.ReplaceListingsAsync(appraisalId, entities, cancellationToken);
    }

    public Task UpdateListingApprovalAsync(int listingId, ListingApprovalStatus status, CancellationToken cancellationToken = default)
    {
        return repository.UpdateListingApprovalAsync(listingId, status, cancellationToken);
    }

    public async Task SaveOcrResultAsync(int appraisalId, AddressInfo? address, CertificatePayload? certificate, CancellationToken cancellationToken = default)
    {
        var ocr = new OcrResult
        {
            Province = address?.Province,
            City = address?.City,
            District = address?.District,
            SubDistrict = address?.SubDistrict,
            JenisSertifikat = certificate?.JenisSertifikat,
            NomorSertifikat = certificate?.NomorSertifikat,
            NamaPemegang = certificate?.NamaPemegang,
            Nib = certificate?.Nib,
            LuasTanah = certificate?.LuasTanah,
            LuasBangunan = certificate?.LuasBangunan,
        };
        await repository.SaveOcrResultAsync(appraisalId, ocr, cancellationToken);
    }

    public async Task<(AddressInfo? Address, CertificatePayload? Certificate)> GetOcrResultAsync(int appraisalId, CancellationToken cancellationToken = default)
    {
        var ocr = await repository.GetOcrResultAsync(appraisalId, cancellationToken);
        if (ocr is null) return (null, null);

        var address = new AddressInfo
        {
            Province = ocr.Province ?? string.Empty,
            City = ocr.City ?? string.Empty,
            District = ocr.District ?? string.Empty,
            SubDistrict = ocr.SubDistrict ?? string.Empty,
        };
        var cert = new CertificatePayload
        {
            JenisSertifikat = ocr.JenisSertifikat,
            NomorSertifikat = ocr.NomorSertifikat,
            NamaPemegang = ocr.NamaPemegang,
            Nib = ocr.Nib,
            LuasTanah = ocr.LuasTanah,
            LuasBangunan = ocr.LuasBangunan,
        };
        return (address, cert);
    }

    public async Task<List<PropertyListing>> GetListingsAsync(int appraisalId, CancellationToken cancellationToken = default)
    {
        var entities = await repository.GetListingsAsync(appraisalId, cancellationToken);
        return entities.Select(e => new PropertyListing
        {
            Url = e.Url,
            ImageUrl = e.ImageUrl ?? string.Empty,
            Price = e.Price ?? string.Empty,
            Date = e.Date ?? string.Empty,
            Type = e.Type ?? string.Empty,
            Title = e.Title ?? string.Empty,
            Description = e.Description ?? string.Empty,
            Lt = e.Lt ?? string.Empty,
            Lb = e.Lb ?? string.Empty,
            Kt = e.Kt ?? string.Empty,
            Km = e.Km ?? string.Empty,
            DetailDescription = e.DetailDescription,
            Certificate = e.Certificate,
            Floor = e.Floor,
            Electricity = e.Electricity,
            Furnished = e.Furnished,
            Facing = e.Facing,
            LocationText = e.LocationText,
            Transaction = e.Transaction,
            PropertyType = e.PropertyType,
            AddressDetail = e.AddressDetail,
            LocationDetail = e.LocationDetail,
            GroupDetail = e.GroupDetail,
            Garage = e.Garage,
            ListedDate = e.ListedDate,
            IdListing = e.IdListing,
        }).ToList();
    }
}