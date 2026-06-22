using Domain.Definition.MetaDomain;

namespace Application.Interfaces.Cache.MetaDomain
{
    public interface IEffectCache
    {
        void Load(
            List<EffectDefinition> data);
        IEnumerable<EffectDefinition> GetAll();
        EffectDefinition? Get(
            string id);
    }
}
