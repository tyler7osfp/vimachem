using Library.Application.Dtos;

namespace Library.Application.Services;

public interface IPartyCommandService
{
    Task<PartyListItemDto> CreateAsync(string name, string email, string initialRole, CancellationToken ct = default);
    Task<PartyListItemDto?> UpdateAsync(Guid id, string name, string email, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
