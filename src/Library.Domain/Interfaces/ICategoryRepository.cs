using Library.Domain.Entities;

namespace Library.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Delete(Category entity);
    }
}
