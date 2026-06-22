using Contract.Enum.MetaDomain.Effect;
using Domain.Abstraction;

namespace Domain.Definition.EntityDomain.Component
{
    public class CharacteristicDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public List<AttributeValue> AttributeValues { get; private set; } = new();
        #endregion

        protected CharacteristicDefinition() : base() { }

        public CharacteristicDefinition(Guid id, string entityDefinitionId) : base(id, entityDefinitionId) { }

        #region Methods
        #endregion
    }

    public class AttributeValue
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid ID { get; private set; }
        public AttributeType Type { get; private set; }
        public float BaseValue { get; private set; } // base value at this level
        public int Level { get; private set; } // progression control
        public float Min { get; private set; }
        public float Max { get; private set; }

        public Guid CharacteristicDefinitionID { get; private set; }
        public CharacteristicDefinition CharacteristicDefinition { get; private set; }
        public List<AttributeGrowthValue> AttributeGrowthValues { get; private set; } = new();
        #endregion

        protected AttributeValue() { }

        public AttributeValue(
            Guid id,
            AttributeType type,
            float baseValue,
            int level,
            float min,
            float max,
            Guid characteristicDefinitionId)
        {
            ID = id;
            Type = type;
            BaseValue = baseValue;
            Level = level;
            Min = min;
            Max = max;
            CharacteristicDefinitionID = characteristicDefinitionId;
        }

        #region Methods
        #endregion
    }

    public class AttributeGrowthValue
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid ID { get; private set; }
        public int Level { get; private set; }
        public float GrowthValue { get; private set; }

        public Guid AttributeValueID { get; private set; }
        public AttributeValue AttributeValue { get; private set; }
        #endregion

        protected AttributeGrowthValue() { }

        public AttributeGrowthValue(
            Guid id,
            int level,
            float growthValue,
            Guid attributeValueId)
        {
            ID = id;
            Level = level;
            GrowthValue = growthValue;
            AttributeValueID = attributeValueId;
        }

        #region Methods
        #endregion
    }
}