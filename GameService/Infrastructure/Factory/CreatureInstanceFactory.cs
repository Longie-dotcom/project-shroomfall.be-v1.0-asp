using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Application.Services.Abstraction.AttributeService;
using Application.Services.Abstraction.ItemService;
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
    public class CreatureInstanceFactory : ICreatureInstanceFactory
    {
        #region Attributes
        private readonly IEntityCache entityCache;
        private readonly IInventoryInstanceFactory inventoryInstanceFactory;
        private readonly ICharacteristicInstanceFactory characteristicInstanceFactory;
        private readonly IEffectInstanceFactory effectInstanceFactory;
        private readonly ICharacteristicService characteristicService;
        private readonly IEquipmentService equipmentService;
        #endregion

        #region Properties
        #endregion

        public CreatureInstanceFactory(
            IEntityCache entityCache,
            IInventoryInstanceFactory inventoryInstanceFactory,
            ICharacteristicInstanceFactory characteristicInstanceFactory,
            IEffectInstanceFactory effectInstanceFactory,
            ICharacteristicService characteristicService,
            IEquipmentService equipmentService)
        {
            this.entityCache = entityCache;
            this.inventoryInstanceFactory = inventoryInstanceFactory;
            this.characteristicInstanceFactory = characteristicInstanceFactory;
            this.effectInstanceFactory = effectInstanceFactory;
            this.characteristicService = characteristicService;
            this.equipmentService = equipmentService;
        }

        #region Methods
        public CreatureInstance Create(
            string definitionId,
            string instanceId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction)
        {
            var creatureDef = entityCache.Get<Creature>(definitionId);
            if (creatureDef == null)
                throw new InternalException(
                    ResponseCode.CreatureInstanceFactory_DefinitionNotFound,
                    $"Creature definition with ID: {definitionId} is not found in cache");

            var instance = new CreatureInstance(
                id: instanceId,
                definitionId: creatureDef.ID,
                collisionShape: CollisionShapeMapper.FromDefinition(creatureDef.Collision),
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                direction: direction,
                appearance: MapAppearance(creatureDef.Appearance),

                characteristic: characteristicInstanceFactory.Create(
                    creatureDef.CharacteristicID),
                inventory: inventoryInstanceFactory.Create(
                    creatureDef.InventoryID),
                level: creatureDef.Level,
                activeEffects: new List<EffectInstance>()
            );

            // Initialize persisted vitals
            characteristicService.InitializeVitals(instance);

            // Recalculate core values along with effects
            characteristicService.RecalculateCoreValues(instance);

            return instance;
        }

        public CreatureInstance CreateFromDocument(
            CreatureDocument doc)
        {
            if (doc == null)
                throw new InternalException(
                    ResponseCode.CreatureInstanceFactory_DocumentNotFound,
                    "Player document is null");

            var creatureDef = entityCache.Get<Creature>(doc.DefinitionID);
            if (creatureDef == null)
                throw new InternalException(
                    ResponseCode.CreatureInstanceFactory_DefinitionFromDocumentNotFound,
                    $"Creature definition with ID: {doc.DefinitionID} is not found in cache");

            var instance = new CreatureInstance(
                id: doc.ID,
                definitionId: doc.DefinitionID,
                collisionShape: CollisionShapeMapper.FromDefinition(creatureDef.Collision),
                roomSpatialId: doc.RoomSpatialID,
                layerZ: doc.LayerZ,
                position: new Vector2(doc.Position.X, doc.Position.Y),
                direction: new Vector2(doc.Direction.X, doc.Direction.Y),

                appearance: MapAppearance(doc.Appearance),
                characteristic: characteristicInstanceFactory.CreateFromDocument(
                    doc.Characteristic),
                inventory: inventoryInstanceFactory.CreateFromDocument(
                    doc.Inventory),
                level: doc.Level,
                activeEffects: doc.ActiveEffects
                    .Select(effect => effectInstanceFactory.CreateFromDocument(effect))
                    .ToList()
            );

            // Restore equipment from document
            equipmentService.RehydrateEquipment(instance, doc.Equipment);

            // Restore persisted vitals
            characteristicService.RehydrateVitals(instance, doc.Characteristic);

            // Recompute derived core values with current effects
            characteristicService.RecalculateCoreValues(instance);

            return instance;
        }

        private AppearanceInstance MapAppearance(
            Appearance def)
        {
            return new AppearanceInstance(
                skinId: def.SkinID,
                skinColor: HSV.Clone(def.SkinColor)
            );
        }

        private AppearanceInstance MapAppearance(
            AppearanceDocument doc)
        {
            return new AppearanceInstance(
                skinId: doc.SkinID,
                skinColor: new HSV(
                    doc.SkinColor.H,
                    doc.SkinColor.S,
                    doc.SkinColor.V)
            );
        }
        #endregion
    }
}