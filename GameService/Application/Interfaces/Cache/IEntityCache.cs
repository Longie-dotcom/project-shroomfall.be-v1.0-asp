using Domain.Definition.EntityDomain;

namespace Application.Interfaces.Cache
{
    public interface IEntityCache
    {
        void Load(
            IEnumerable<Entity> data);
        IReadOnlyCollection<Entity> GetAll();
        T? Get<T>(
            string id) where T : Entity;
    }
}