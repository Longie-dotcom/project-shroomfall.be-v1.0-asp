using Domain.Definition.EntityDomain.Component;
using Domain.Definition.EntityDomain.Enum;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.EntityDomain
{
    public class Creature : Entity
    {
        #region Attributes
        #endregion

        #region Properties
        public string CharacteristicID { get; private set; }
        public string InventoryID { get; private set; }
        public int Level { get; private set; }
        #endregion
        
        protected Creature() 
        { 
        
        }

        public Creature(
            string id,
            EntityType type,
            LocalizedText localizedText,
            Appearance appearance,
            Collision collision,
            string characteristicId,
            string inventoryId,
            int level) : base(
                id,
                type,
                localizedText,
                appearance,
                collision)
        {
            if (string.IsNullOrEmpty(characteristicId))
                throw new BadRequest(ResponseCode.Creature_InvalidCharacteristicId);

            if (string.IsNullOrEmpty(inventoryId))
                throw new BadRequest(ResponseCode.Creature_InvalidInventoryId);

            if (level < 0)
                throw new BadRequest(ResponseCode.Creature_InvalidLevel);

            CharacteristicID = characteristicId;
            InventoryID = inventoryId;
            Level = level;
        }

        #region Methods
        #endregion
    }
}
