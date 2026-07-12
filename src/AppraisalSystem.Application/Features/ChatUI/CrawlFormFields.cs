using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class CrawlFormFields
{
    public string Address { get; set; } = "";
    public string CertificateType { get; set; } = "SHM";
    public string Keyword { get; set; } = "";
    public string Kt { get; set; } = "";
    public string Km { get; set; } = "";
    public string LtMin { get; set; } = "";
    public string LtMax { get; set; } = "";
    public string LbMin { get; set; } = "";
    public string LbMax { get; set; } = "";
    public string Hadap { get; set; } = "";
    public string City { get; set; } = "";
    public string District { get; set; } = "";
    public string TransactionType { get; set; } = "dijual";
}