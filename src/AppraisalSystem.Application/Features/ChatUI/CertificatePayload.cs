using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class CertificatePayload
{
    [JsonPropertyName("jenis_sertifikat")]
    public string? JenisSertifikat { get; set; }

    [JsonPropertyName("nomor_sertifikat")]
    public string? NomorSertifikat { get; set; }

    [JsonPropertyName("nama_pemegang")]
    public string? NamaPemegang { get; set; }

    [JsonPropertyName("nib")]
    public string? Nib { get; set; }

    [JsonPropertyName("luas_tanah")]
    public string? LuasTanah { get; set; }

    [JsonPropertyName("luas_bangunan")]
    public string? LuasBangunan { get; set; }
}