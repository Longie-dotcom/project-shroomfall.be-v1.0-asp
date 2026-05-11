namespace Application.Interfaces.Repository.NonRelational
{
    public interface INonRelationalUoW
    {
        T GetRepository<T>() where T : INonRelationalRepository;
    }

    public interface INonRelationalRepository
    {

    }
}
