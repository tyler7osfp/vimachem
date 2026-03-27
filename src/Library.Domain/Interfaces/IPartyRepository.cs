using Library.Domain.Entities;

namespace Library.Domain.Interfaces
{
    public interface IPartyRepository : IRepository<Party>
    {
        void Delete(Party entity);
    }
}
