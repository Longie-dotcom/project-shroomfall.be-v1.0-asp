using Domain.Definition.AttributeDomain.Enum;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.AttributeDomain
{
    public class AttributeValue
    {
        #region Attributes
        #endregion

        #region Properties
        public AttributeType Type { get; private set; }
        public float Value { get; private set; } // base value at this level
        public int Level { get; private set; } // progression control
        public float Min { get; private set; }
        public float Max { get; private set; }
        public string CharacteristicID { get; private set; }

        public Characteristic Characteristic { get; private set; }
        #endregion

        protected AttributeValue() 
        { 
        
        }
        
        public AttributeValue(
            AttributeType type,
            float value,
            int level,
            float min,
            float max,
            string characteristicId)
        {
            if (value < 0)
                throw new BadRequest(ResponseCode.AttributeValue_InvalidValue);

            if (level < 0)
                throw new BadRequest(ResponseCode.AttributeValue_InvalidLevel);

            if (min < 0)
                throw new BadRequest(ResponseCode.AttributeValue_InvalidMin);

            if (min > max)
                throw new BadRequest(ResponseCode.AttributeValue_InvalidMax);

            if (string.IsNullOrWhiteSpace(characteristicId))
                throw new BadRequest(ResponseCode.AttributeValue_InvalidCharacteristicId);

            Type = type;
            Value = value;
            Level = level;
            Min = min;
            Max = max;
            CharacteristicID = characteristicId;
        }

        #region Methods
        #endregion
    }
}