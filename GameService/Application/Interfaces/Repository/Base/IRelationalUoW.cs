namespace Application.Interfaces.Repository.Base
{
    public interface IRelationalUoW
    {
        T GetRepository<T>() where T : IRelationalRepository;
        Task BeginTransactionAsync();
        Task<int> CommitAsync();
        Task<int> SaveChangesAsync();
    }

    public interface IRelationalRepository
    {

    } 
}
