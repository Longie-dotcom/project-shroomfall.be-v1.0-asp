using Contract.DTO.Definition.MetaDomain;

namespace Application.Interface.Cache.MetaDomain
{
    public interface IEffectCache
    {
        void Load(
            List<EffectDefinitionDTO> data);
        IEnumerable<EffectDefinitionDTO> GetAll();
        EffectDefinitionDTO? Get(
            string id);
    }
}
