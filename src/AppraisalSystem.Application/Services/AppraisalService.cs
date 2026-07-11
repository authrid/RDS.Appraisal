using AppraisalSystem.Application.Common;
using AppraisalSystem.Application.Dtos;
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
            Location = request.Location,
            Specification = request.Specification,
            MarketValue = request.MarketValue,
            LiquidationValue = request.LiquidationValue,
            Notes = request.Notes,
            InternalMemo = request.InternalMemo,
            WorkflowHistoryJson = request.WorkflowHistoryJson,
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
        appraisal.Location = request.Location;
        appraisal.Specification = request.Specification;
        appraisal.MarketValue = request.MarketValue;
        appraisal.LiquidationValue = request.LiquidationValue;
        appraisal.Notes = request.Notes;
        appraisal.InternalMemo = request.InternalMemo;
        appraisal.WorkflowHistoryJson = request.WorkflowHistoryJson;

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

        await AppendWorkflowEntryAsync(id, actor, "Submitted", "Submitted untuk review.", cancellationToken);
        await repository.SetStatusAsync(id, AppraisalStatus.Submitted, null, cancellationToken);
    }

    public async Task ApproveAsync(int id, string? supervisorNote, string? actor = null, CancellationToken cancellationToken = default)
    {
        var note = string.IsNullOrWhiteSpace(supervisorNote) ? "Approved tanpa catatan." : supervisorNote.Trim();
        await AppendWorkflowEntryAsync(id, actor, "Approved", note, cancellationToken);
        await repository.SetStatusAsync(id, AppraisalStatus.Approved, supervisorNote, cancellationToken);
    }

    public async Task RejectAsync(int id, string? supervisorNote, string? actor = null, CancellationToken cancellationToken = default)
    {
        var note = string.IsNullOrWhiteSpace(supervisorNote) ? "Rejected tanpa catatan." : supervisorNote.Trim();
        await AppendWorkflowEntryAsync(id, actor, "Rejected", note, cancellationToken);
        await repository.SetStatusAsync(id, AppraisalStatus.Rejected, supervisorNote, cancellationToken);
    }

    private async Task AppendWorkflowEntryAsync(int id, string? actor, string action, string note, CancellationToken cancellationToken)
    {
        var appraisal = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Appraisal not found.");

        var entries = ParseWorkflowHistory(appraisal.WorkflowHistoryJson);
        entries.Add(new WorkflowEntry
        {
            TimestampUtc = DateTime.UtcNow,
            Actor = string.IsNullOrWhiteSpace(actor) ? "System User" : actor.Trim(),
            Action = action,
            Note = note
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
    }

    private static AppraisalListItemDto MapListItem(Appraisal appraisal)
    {
        return new AppraisalListItemDto
        {
            Id = appraisal.Id,
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
            CreatedAtUtc = appraisal.CreatedAtUtc
        };
    }

    private static AppraisalDetailDto MapDetail(Appraisal appraisal)
    {
        return new AppraisalDetailDto
        {
            Id = appraisal.Id,
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
            Status = appraisal.Status,
            CreatedBy = appraisal.CreatedBy,
            CreatedAtUtc = appraisal.CreatedAtUtc,
            UpdatedAtUtc = appraisal.UpdatedAtUtc,
            SupervisorNote = appraisal.SupervisorNote
        };
    }
}