using Domain.Definition.AttributeDomain;

namespace Application.Interfaces.Cache
{
    public interface IEffectCache
    {
        void Load(
            IEnumerable<Effect> data);
        IReadOnlyCollection<Effect> GetAll();
        Effect? Get(
            string id);
    }
}
