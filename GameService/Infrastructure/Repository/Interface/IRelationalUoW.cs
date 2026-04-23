namespace Infrastructure.Repository.Interface
{
    public interface IRelationalUoW
    {
        T GetRepository<T>() where T : IRelationalRepository;

        Task BeginTransactionAsync();

        Task<int> CommitAsync();

        Task SaveChangesAsync();
    }

    public interface IRelationalRepository
    {

    } 
}
