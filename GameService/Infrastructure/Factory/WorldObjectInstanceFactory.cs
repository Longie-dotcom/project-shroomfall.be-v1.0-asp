using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Common;
using Domain.Definition.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Document.EntityDomain;
using Domain.Document.EntityDomain.Component;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class WorldObjectInstanceFactory : IWorldObjectInstanceFactory
    {
        #region Attributes
        private readonly IEntityCache entityCache;
        private readonly IInventoryInstanceFactory inventoryInstanceFactory;
        #endregion

        #region Properties
        #endregion

        public WorldObjectInstanceFactory(
            IEntityCache entityCache,
            IInventoryInstanceFactory inventoryInstanceFactory)
        {
            this.entityCache = entityCache;
            this.inventoryInstanceFactory = inventoryInstanceFactory;
        }

        #region Methods
        public (WorldObjectInstance worldObject, string? roomSpatialReferenceId) Create(
            string definitionId,
            string instanceId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction)
        {
            var worldObjectDef = entityCache.Get<WorldObject>(definitionId);
            if (worldObjectDef == null)
                throw new InternalException(
                    ResponseCode.WorldObjectInstanceFactory_DefinitionNotFound,
                    $"World object definition with ID: {definitionId} is not found in cache");

            var roomSpatialReferenceId = !string.IsNullOrWhiteSpace(worldObjectDef.RoomID) ? Guid.NewGuid().ToString() : null;

            var instance = new WorldObjectInstance(
                id: instanceId,
                definitionId: worldObjectDef.ID,
                collisionShape: CollisionShapeMapper.FromDefinition(worldObjectDef.Collision),
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                direction: direction,
                appearance: MapAppearance(worldObjectDef.Appearance),

                inventory: !string.IsNullOrWhiteSpace(worldObjectDef.InventoryID) 
                    ? inventoryInstanceFactory.Create(worldObjectDef.InventoryID) 
                    : null,
                roomSpatialReferenceId: roomSpatialReferenceId
            );

            return (instance, roomSpatialReferenceId);
        }

        public WorldObjectInstance CreateFromDocument(
            WorldObjectDocument doc)
        {
            if (doc == null)
                throw new InternalException(
                    ResponseCode.WorldObjectInstanceFactory_DocumentNotFound,
                    "World object document is null");

            var worldObjectDef = entityCache.Get<WorldObject>(doc.DefinitionID);
            if (worldObjectDef == null)
                throw new InternalException(
                    ResponseCode.WorldObjectInstanceFactory_DefinitionFromDocumentNotFound,
                    $"World object definition with ID: {doc.DefinitionID} is not found in cache");

            var instance = new WorldObjectInstance(
                id: doc.ID,
                definitionId: doc.DefinitionID,
                collisionShape: CollisionShapeMapper.FromDefinition(worldObjectDef.Collision),
                roomSpatialId: doc.RoomSpatialID,
                layerZ: doc.LayerZ,
                position: new Vector2(doc.Position.X, doc.Position.Y),
                direction: new Vector2(doc.Direction.X, doc.Direction.Y),
                appearance: MapAppearance(doc.Appearance),

                inventory: doc.Inventory != null
                    ? inventoryInstanceFactory.CreateFromDocument(doc.Inventory)
                    : null,
                roomSpatialReferenceId: doc.RoomSpatialReferenceID
            );

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