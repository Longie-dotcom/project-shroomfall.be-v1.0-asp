using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Common;
using Domain.Definition.EntityDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class AreaEffectInstanceFactory : IAreaEffectInstanceFactory
    {
        #region Attributes
        private readonly IEntityCache entityCache;
        #endregion

        #region Properties
        #endregion

        public AreaEffectInstanceFactory(
            IEntityCache entityCache)
        {
            this.entityCache = entityCache;
        }

        #region Methods
        public AreaEffectInstance Create(
            string definitionId,
            string instanceId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 movementVector,
            string entityInstanceOwnerId,
            string? sourceDefinitionId)
        {
            var areaEffectDef = entityCache.Get<AreaEffect>(definitionId);
            if (areaEffectDef == null)
                throw new InternalException(
                    ResponseCode.AreaEffectInstanceFactory_DefinitionNotFound,
                    $"Area effect definition with ID: {definitionId} is not found in cache");

            var instance = new AreaEffectInstance(
                id: instanceId,
                definitionId: areaEffectDef.ID,
                collisionShape: CollisionShapeMapper.FromDefinition(areaEffectDef.Collision),
                collisionOffset: new Vector2(areaEffectDef.Collision.OffsetX, areaEffectDef.Collision.OffsetY),
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                movementVector: movementVector,
                appearance: AppearanceMapper.MapAppearance(areaEffectDef.Appearance),
                entityInstanceOwnerId: entityInstanceOwnerId,
                sourceDefinitionId: sourceDefinitionId,
                duration: areaEffectDef.Duration,
                2f
            );

            return instance;
        }
        #endregion
    }
}