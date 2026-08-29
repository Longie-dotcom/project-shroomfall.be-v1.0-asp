namespace Application.Interface.Repository.Base
{
    public interface IUnitOfWork
    {
        T GetRepository<T>() where T : IRepository;
    }

    public interface IRepository
    {

    }
}
