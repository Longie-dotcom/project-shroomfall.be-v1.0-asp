using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Application.Services.AttributeService;
using Domain.Common;
using Domain.Definition.EntityDomain;
using Domain.Document.EntityDomain;
using Domain.DomainException;
using Domain.Runtime.AttributeDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.ItemDomain;
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
        #endregion

        #region Properties
        #endregion

        public PlayerInstanceFactory(
            IEntityCache entityCache,
            IInventoryInstanceFactory inventoryInstanceFactory,
            ICharacteristicInstanceFactory characteristicInstanceFactory,
            IEffectInstanceFactory effectInstanceFactory,
            CharacteristicService characteristicService)
        {
            this.entityCache = entityCache;
            this.inventoryInstanceFactory = inventoryInstanceFactory;
            this.characteristicInstanceFactory = characteristicInstanceFactory;
            this.effectInstanceFactory = effectInstanceFactory;
            this.characteristicService = characteristicService;
        }

        #region Methods
        public PlayerInstance Create(
            string definitionId,
            string instanceId,
            string userId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 movementVector)
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
                collisionOffset: new Vector2(playerDef.Collision.OffsetX, playerDef.Collision.OffsetY),
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                movementVector: movementVector,

                characteristic: characteristicInstanceFactory.Create(playerDef.CharacteristicID),
                inventory: inventoryInstanceFactory.Create(playerDef.InventoryID),
                level: playerDef.Level,
                activeEffects: new List<EffectInstance>(),

                userId: userId,
                appearance: AppearanceMapper.MapAppearance(playerDef.Appearance)
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
                collisionOffset: new Vector2(playerDef.Collision.OffsetX, playerDef.Collision.OffsetY),
                roomSpatialId: doc.RoomSpatialID,
                layerZ: doc.LayerZ,
                position: new Vector2(doc.Position.X, doc.Position.Y),
                movementVector: new Vector2(doc.MovementVector.X, doc.MovementVector.Y),

                characteristic: characteristicInstanceFactory.CreateFromDocument(doc.Characteristic),
                inventory: inventoryInstanceFactory.CreateFromDocument(doc.Inventory),
                level: doc.Level,
                activeEffects: doc.ActiveEffects
                    .Select(effect => effectInstanceFactory.CreateFromDocument(effect))
                    .ToList(),

                userId: doc.UserID,
                appearance: AppearanceMapper.MapAppearance(doc.Appearance)
            );

            // Restore equipment from document
            foreach (var kv in doc.Equipment)
            {
                if (kv.Value == null)
                    continue;

                var item = new ItemInstance(
                    kv.Value.ID,
                    kv.Value.DefinitionID,
                    kv.Value.Count,
                    kv.Value.CurrentDurability,
                    kv.Value.Quality);

                instance.SetEquipment(kv.Key, item);
            }

            // Restore persisted vitals
            characteristicService.RehydrateVitals(instance, doc.Characteristic);

            // Recompute derived core values with current effects
            characteristicService.RecalculateCoreValues(instance);

            return instance;
        }
        #endregion
    }
}