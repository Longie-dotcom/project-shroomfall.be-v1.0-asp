using Contract;
using Contract.Enum.MetaDomain.Effect;
using Domain.Abstraction;
using Domain.DomainException;
using ResponseCode;

namespace Domain.Runtime.EntityDomain.Component
{
    public class CharacteristicInstance : ComponentInstance
    {
        #region Attributes
        private readonly Dictionary<AttributeType, float> cores;
        private readonly Dictionary<AttributeType, float> vitals;
        #endregion

        #region Properties
        public int CurrentLevel { get; private set; }
        #endregion

        public CharacteristicInstance(
            Guid definitionId,
            int level) : base(definitionId)
        {
            CurrentLevel = level;
            vitals = new Dictionary<AttributeType, float>();
            cores = new Dictionary<AttributeType, float>();
        }

        #region Methods
        public IReadOnlyDictionary<AttributeType, float> GetVitals()
        {
            return vitals;
        }

        public float GetVital(
            AttributeType type)
        {
            return vitals.TryGetValue(type, out var v) ? v : 0f;
        }

        public void SetVital(
            AttributeType type,
            float value)
        {
            if (AttributeDefinitions.Get(type).DomainType != DomainType.Vital)
                throw new InternalException(
                    DomainCode.CharacteristicInstanceCode.NotAVitalAttribute,
                    $"Stat modification rejected for definition '{DefinitionID}'. The attribute type '{type}' is not configured as a Vital field.");

            vitals[type] = value;
        }

        public IReadOnlyDictionary<AttributeType, float> GetCores()
        {
            return cores;
        }

        public float GetCore(
            AttributeType type)
        {
            return cores.TryGetValue(type, out var v) ? v : 0f;
        }

        public void SetCore(
            AttributeType type,
            float value)
        {
            if (AttributeDefinitions.Get(type).DomainType != DomainType.Core)
                throw new InternalException(
                    DomainCode.CharacteristicInstanceCode.NotACoreAttribute,
                    $"Stat modification rejected for definition '{DefinitionID}'. The attribute type '{type}' is not configured as a Core field.");

            cores[type] = value;
        }
        #endregion
    }
}