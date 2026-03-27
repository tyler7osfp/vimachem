using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repos
{
    public class CategoryRepository(LibraryDbContext _ctx) : ICategoryRepository
    {
        public async Task AddAsync(Category entity, CancellationToken ct = default)
        {
            await _ctx.AddAsync(entity, ct);
        }

        public void Delete(Category entity)
        {
            _ctx.Categories.Update(entity);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default)
        {
            return await _ctx.Categories.ToListAsync(ct);
        }

        public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return _ctx.Categories.FirstOrDefaultAsync(z => z.Id == id, ct);
        }

        public void Update(Category entity)
        {
            _ctx.Categories.Update(entity);
        }
    }
}
