using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Domain.Entities;

public class OcrResult
{
    public int Id { get; set; }
    public int AppraisalId { get; set; }

    // Address fields
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? SubDistrict { get; set; }

    // Certificate fields
    public string? JenisSertifikat { get; set; }
    public string? NomorSertifikat { get; set; }
    public string? NamaPemegang { get; set; }
    public string? Nib { get; set; }
    public string? LuasTanah { get; set; }
    public string? LuasBangunan { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Appraisal Appraisal { get; set; } = null!;
}
