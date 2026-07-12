namespace AppraisalSystem.Application.Dtos;

// DTO umum untuk semua master/reference data (JSON hari ini, DB nanti).
public sealed class ReferenceItemDto
{
    public string Code { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public bool IsActive { get; init; } = true;

    // Opsional: parent code untuk hirarki (kota→provinsi, kecamatan→kota, kelurahan→kecamatan).
    public string? ParentCode { get; init; }

    // Bentuk tampilan default untuk field yang berbeda code vs label (mis. cabang "001 - Kantor Pusat").
    public string Display => string.IsNullOrWhiteSpace(Code) || string.Equals(Code, Label, StringComparison.Ordinal)
        ? Label
        : $"{Code} - {Label}";
}
