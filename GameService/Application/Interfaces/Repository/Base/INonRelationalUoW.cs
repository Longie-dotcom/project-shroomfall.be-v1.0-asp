namespace Application.Interfaces.Repository.Base
{
    public interface INonRelationalUoW
    {
        T GetRepository<T>() where T : INonRelationalRepository;
    }

    public interface INonRelationalRepository
    {

    }
}
