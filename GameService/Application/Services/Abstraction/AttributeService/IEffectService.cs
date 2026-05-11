using Domain.Definition.ItemDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Services.Abstraction.AttributeService
{
    public interface IEffectService
    {
        void ApplyItemEffects(
            CreatureInstance creature,
            Item itemDef,
            string sourceItemInstanceId);
        void RemoveItemEffects(
            CreatureInstance creature,
            string sourceItemInstanceId);
    }
}
