using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Domain.Entities;

public class SavedPropertyListing
{
    public int Id { get; set; }
    public int AppraisalId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Price { get; set; }
    public string? Date { get; set; }
    public string? Type { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Lt { get; set; }
    public string? Lb { get; set; }
    public string? Kt { get; set; }
    public string? Km { get; set; }
    public string? DetailDescription { get; set; }
    public string? Certificate { get; set; }
    public string? Floor { get; set; }
    public string? Electricity { get; set; }
    public string? Furnished { get; set; }
    public string? Facing { get; set; }
    public string? LocationText { get; set; }
    public string? Transaction { get; set; }
    public string? PropertyType { get; set; }
    public string? AddressDetail { get; set; }
    public string? LocationDetail { get; set; }
    public string? GroupDetail { get; set; }
    public string? Garage { get; set; }
    public string? ListedDate { get; set; }
    public string? IdListing { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ListingApprovalStatus ApprovalStatus { get; set; } = ListingApprovalStatus.Pending;

    public Appraisal Appraisal { get; set; } = null!;
}
