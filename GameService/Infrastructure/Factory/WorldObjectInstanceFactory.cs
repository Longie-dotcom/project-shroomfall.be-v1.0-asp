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
        public WorldObjectInstance Create(
            string definitionId,
            string instanceId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 movementVector)
        {
            var worldObjectDef = entityCache.Get<WorldObject>(definitionId);
            if (worldObjectDef == null)
                throw new InternalException(
                    ResponseCode.WorldObjectInstanceFactory_DefinitionNotFound,
                    $"World object definition with ID: {definitionId} is not found in cache");

            var instance = new WorldObjectInstance(
                id: instanceId,
                definitionId: worldObjectDef.ID,
                collisionShape: CollisionShapeMapper.FromDefinition(worldObjectDef.Collision),
                collisionOffset: new Vector2(worldObjectDef.Collision.OffsetX, worldObjectDef.Collision.OffsetY),
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                movementVector: movementVector,
                appearance: AppearanceMapper.MapAppearance(worldObjectDef.Appearance),

                inventory: !string.IsNullOrWhiteSpace(worldObjectDef.InventoryID) 
                    ? inventoryInstanceFactory.Create(worldObjectDef.InventoryID) 
                    : null
            );

            return instance;
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
                collisionOffset: new Vector2(worldObjectDef.Collision.OffsetX, worldObjectDef.Collision.OffsetY),
                roomSpatialId: doc.RoomSpatialID,
                layerZ: doc.LayerZ,
                position: new Vector2(doc.Position.X, doc.Position.Y),
                movementVector: new Vector2(doc.MovementVector.X, doc.MovementVector.Y),
                appearance: AppearanceMapper.MapAppearance(doc.Appearance),

                inventory: doc.Inventory != null
                    ? inventoryInstanceFactory.CreateFromDocument(doc.Inventory)
                    : null
            );

            return instance;
        }
        #endregion
    }
}