namespace Infrastructure.Repository.Interface
{
    public interface INonRelationalUoW
    {
        T GetRepository<T>() where T : INonRelationalRepository;
    }

    public interface INonRelationalRepository
    {

    }
}
