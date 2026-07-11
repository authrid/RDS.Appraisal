namespace AppraisalSystem.Application.Common;

public sealed class PagedResult<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }
    public required int TotalItems { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }

    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}