using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Application.Services.AttributeService;
using Application.Services.ItemService;
using Domain.Common;
using Domain.Definition.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Document.EntityDomain;
using Domain.Document.EntityDomain.Component;
using Domain.DomainException;
using Domain.Runtime.AttributeDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class PlayerInstanceFactory : IPlayerInstanceFactory
    {
        #region Attributes
        private readonly IEntityCache entityCache;
        private readonly IInventoryInstanceFactory inventoryInstanceFactory; 
        private readonly ICharacteristicInstanceFactory characteristicInstanceFactory;
        private readonly IEffectInstanceFactory effectInstanceFactory;
        private readonly CharacteristicService characteristicService;
        private readonly EquipmentService equipmentService;
        #endregion

        #region Properties
        #endregion

        public PlayerInstanceFactory(
            IEntityCache entityCache,
            IInventoryInstanceFactory inventoryInstanceFactory,
            ICharacteristicInstanceFactory characteristicInstanceFactory,
            IEffectInstanceFactory effectInstanceFactory,
            CharacteristicService characteristicService,
            EquipmentService equipmentService)
        {
            this.entityCache = entityCache;
            this.inventoryInstanceFactory = inventoryInstanceFactory;
            this.characteristicInstanceFactory = characteristicInstanceFactory;
            this.effectInstanceFactory = effectInstanceFactory;
            this.characteristicService = characteristicService;
            this.equipmentService = equipmentService;
        }

        #region Methods
        public PlayerInstance Create(
            string definitionId,
            string instanceId,
            string userId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction)
        {
            var playerDef = entityCache.Get<Player>(definitionId);
            if (playerDef == null)
                throw new InternalException(
                    ResponseCode.PlayerInstanceFactory_DefinitionNotFound,
                    $"Player definition with ID: {definitionId} is not found in cache");

            var instance = new PlayerInstance(
                id: instanceId,
                definitionId: playerDef.ID,
                collisionShape: CollisionShapeMapper.FromDefinition(playerDef.Collision),
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                direction: direction,

                characteristic: characteristicInstanceFactory.Create(playerDef.CharacteristicID),
                inventory: inventoryInstanceFactory.Create(playerDef.InventoryID),
                level: playerDef.Level,
                activeEffects: new List<EffectInstance>(),

                userId: userId,
                playerAppearance: MapAppearance(playerDef.PlayerAppearance)
            );

            // Initialize persisted vitals
            characteristicService.InitializeVitals(instance);

            // Recalculate core values along with effects
            characteristicService.RecalculateCoreValues(instance);

            return instance;
        }

        public PlayerInstance CreateFromDocument(
            PlayerDocument doc)
        {
            if (doc == null)
                throw new InternalException(
                    ResponseCode.PlayerInstanceFactory_DocumentNotFound,
                    "Player document is null");

            var playerDef = entityCache.Get<Player>(doc.DefinitionID);
            if (playerDef == null)
                throw new InternalException(
                    ResponseCode.PlayerInstanceFactory_DefinitionFromDocumentNotFound,
                    $"Player definition with ID: {doc.DefinitionID} is not found in cache");

            var instance = new PlayerInstance(
                id: doc.ID,
                definitionId: doc.DefinitionID,
                collisionShape: CollisionShapeMapper.FromDefinition(playerDef.Collision),
                roomSpatialId: doc.RoomSpatialID,
                layerZ: doc.LayerZ,
                position: new Vector2(doc.Position.X, doc.Position.Y),
                direction: new Vector2(doc.Direction.X, doc.Direction.Y),

                characteristic: characteristicInstanceFactory.CreateFromDocument(doc.Characteristic),
                inventory: inventoryInstanceFactory.CreateFromDocument(doc.Inventory),
                level: doc.Level,
                activeEffects: doc.ActiveEffects
                    .Select(effect => effectInstanceFactory.CreateFromDocument(effect))
                    .ToList(),

                userId: doc.UserID,
                playerAppearance: MapAppearance(doc.PlayerAppearance)
            );

            // Restore equipment from document
            equipmentService.RehydrateEquipment(instance, doc.Equipment);

            // Restore persisted vitals
            characteristicService.RehydrateVitals(instance, doc.Characteristic);

            // Recompute derived core values with current effects
            characteristicService.RecalculateCoreValues(instance);

            return instance;
        }

        private PlayerAppearanceInstance MapAppearance(
            PlayerAppearance def)
        {
            return new PlayerAppearanceInstance(
                skinId: def.SkinID,
                skinColor: HSV.Clone(def.SkinColor),
                hairId: def.HairID,
                glassesId: def.GlassesID,
                shirtId: def.ShirtID,
                pantId: def.PantID,
                shoeId: def.ShoeID,
                eyesId: def.EyesID,
                hairColor: HSV.Clone(def.HairColor),
                pantColor: HSV.Clone(def.PantColor),
                eyeColor: HSV.Clone(def.EyeColor)
            );
        }

        private PlayerAppearanceInstance MapAppearance(
            PlayerAppearanceDocument doc)
        {
            return new PlayerAppearanceInstance(
                skinId: doc.SkinID,
                skinColor: new HSV(
                    doc.SkinColor.H,
                    doc.SkinColor.S,
                    doc.SkinColor.V),

                hairId: doc.HairID,
                glassesId: doc.GlassesID,
                shirtId: doc.ShirtID,
                pantId: doc.PantID,
                shoeId: doc.ShoeID,
                eyesId: doc.EyesID,

                hairColor: new HSV(
                    doc.HairColor.H,
                    doc.HairColor.S,
                    doc.HairColor.V),

                pantColor: new HSV(
                    doc.PantColor.H,
                    doc.PantColor.S,
                    doc.PantColor.V),

                eyeColor: new HSV(
                    doc.EyeColor.H,
                    doc.EyeColor.S,
                    doc.EyeColor.V)
            );
        }
        #endregion
    }
}