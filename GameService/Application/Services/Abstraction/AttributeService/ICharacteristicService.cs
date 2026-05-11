using Domain.Definition.AttributeDomain.Enum;
using Domain.Document.AttributeDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Services.Abstraction.AttributeService
{
    public interface ICharacteristicService
    {
        public void InitializeVitals(
            CreatureInstance creature);
        float ModifyVitalValue(
            CreatureInstance creature,
            AttributeType type,
            float delta);
        void RehydrateVitals(
            CreatureInstance creature,
            CharacteristicDocument doc);
        void RecalculateCoreValues(
            CreatureInstance creature);
    }
}
