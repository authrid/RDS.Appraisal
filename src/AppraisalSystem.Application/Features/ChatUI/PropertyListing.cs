using System.Text.Json.Serialization;
using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Application.Features.ChatUI;

/// <summary>Structured property listing from Brighton.</summary>
public class PropertyListing
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("lt")]
    public string Lt { get; set; } = string.Empty;

    [JsonPropertyName("lb")]
    public string Lb { get; set; } = string.Empty;

    [JsonPropertyName("kt")]
    public string Kt { get; set; } = string.Empty;

    [JsonPropertyName("km")]
    public string Km { get; set; } = string.Empty;

    [JsonPropertyName("detail_description")]
    public string? DetailDescription { get; set; }

    [JsonPropertyName("certificate")]
    public string? Certificate { get; set; }

    [JsonPropertyName("floor")]
    public string? Floor { get; set; }

    [JsonPropertyName("electricity")]
    public string? Electricity { get; set; }

    [JsonPropertyName("furnished")]
    public string? Furnished { get; set; }

    [JsonPropertyName("facing")]
    public string? Facing { get; set; }

    [JsonPropertyName("location_text")]
    public string? LocationText { get; set; }

    [JsonPropertyName("additional_images")]
    public List<string>? AdditionalImages { get; set; }

    [JsonPropertyName("transaction")]
    public string? Transaction { get; set; }

    [JsonPropertyName("property_type")]
    public string? PropertyType { get; set; }

    [JsonPropertyName("address_detail")]
    public string? AddressDetail { get; set; }

    [JsonPropertyName("location_detail")]
    public string? LocationDetail { get; set; }

    [JsonPropertyName("group_detail")]
    public string? GroupDetail { get; set; }

    [JsonPropertyName("garage")]
    public string? Garage { get; set; }

    [JsonPropertyName("listed_date")]
    public string? ListedDate { get; set; }

    [JsonPropertyName("id_listing")]
    public string? IdListing { get; set; }

    [JsonPropertyName("approval_status")]
    public ListingApprovalStatus ApprovalStatus { get; set; } = ListingApprovalStatus.Pending;
}