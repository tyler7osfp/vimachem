namespace EventService.Models;

public sealed record PagedResponse<T>(
    long Total,
    int Page,
    int PageSize,
    IReadOnlyCollection<T> Items);

