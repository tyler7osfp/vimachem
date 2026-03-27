namespace Library.Domain.Interfaces
{
    public interface IUnitOfWork
    { 
        Task SaveChangesAsync(CancellationToken ct= default);
    }
}
