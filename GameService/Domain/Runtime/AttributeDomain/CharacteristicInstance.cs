using Domain.Definition.AttributeDomain.Enum;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Runtime.AttributeDomain
{
    public class CharacteristicInstance
    {
        #region Attributes
        private readonly Dictionary<AttributeType, float> cores;
        private readonly Dictionary<AttributeType, float> vitals;
        #endregion

        #region Properties
        public string ID { get; }
        public string DefinitionID { get; }
        #endregion

        public CharacteristicInstance(
            string id,
            string definitionId)
        {
            ID = id;
            DefinitionID = definitionId;

            vitals = new Dictionary<AttributeType, float>();
            cores = new Dictionary<AttributeType, float>();
        }

        #region Methods
        public IReadOnlyDictionary<AttributeType, float> GetVitals()
        {
            return vitals;
        }

        public float GetVital(AttributeType type)
        {
            return vitals.TryGetValue(type, out var v) ? v : 0f;
        }

        public void SetVital(AttributeType type, float value)
        {
            if (AttributeDefinitions.Get(type).DomainType != DomainType.Vital)
                throw new InternalException(
                    ResponseCode.CharacteristicInstance_NotAVitalAttribute,
                    $"{type} is not a Vital attribute.");

            vitals[type] = value;
        }

        public IReadOnlyDictionary<AttributeType, float> GetCores()
        {
            return cores;
        }

        public float GetCore(AttributeType type)
        {
            return cores.TryGetValue(type, out var v) ? v : 0f;
        }

        public void SetCore(AttributeType type, float value)
        {
            if (AttributeDefinitions.Get(type).DomainType != DomainType.Core)
                throw new InternalException(
                    ResponseCode.CharacteristicInstance_NotACoreAttribute,
                    $"{type} is not a Core attribute.");

            cores[type] = value;
        }
        #endregion
    }
}