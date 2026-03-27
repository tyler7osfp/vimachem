using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repos
{
    public class PartyRepository(LibraryDbContext _ctx) : IPartyRepository
    {
        public async Task AddAsync(Party entity, CancellationToken ct = default)
            => await _ctx.Parties.AddAsync(entity, ct);

        public void Delete(Party entity)
        {
            _ctx.Parties.Update(entity);
        }

        public async Task<IEnumerable<Party>> GetAllAsync(CancellationToken ct = default)
            => await _ctx.Parties.Include(z => z.Roles).ToListAsync(ct);

        public Task<Party?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _ctx.Parties.Include(z => z.Roles).FirstOrDefaultAsync(x => x.Id == id, ct);

        public void Update(Party entity)
            => _ctx.Parties.Update(entity);
    }
}
