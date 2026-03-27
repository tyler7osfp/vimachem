namespace Library.Application.Services;

public interface IPartyRoleCommandService
{
    Task AddRoleAsync(Guid id, string role, CancellationToken ct = default);
    Task<bool> RemoveRoleAsync(Guid id, string role, CancellationToken ct = default);
}
