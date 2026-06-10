using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Common;
using Domain.Definition.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class ProjectileInstanceFactory : IProjectileInstanceFactory
    {
        #region Attributes
        private readonly IEntityCache entityCache;
        private readonly IEntityRelationshipCache entityRelationshipCache;
        #endregion

        #region Properties
        #endregion

        public ProjectileInstanceFactory(
            IEntityCache entityCache,
            IEntityRelationshipCache entityRelationshipCache)
        {
            this.entityCache = entityCache;
            this.entityRelationshipCache = entityRelationshipCache;
        }

        #region Methods
        public ProjectileInstance Create(
            string definitionId,
            string instanceId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 movementVector,
            string entityInstanceOwnerId,
            string sourceDefinitionId)
        {
            var projectileDef = entityCache.Get<Projectile>(definitionId);
            if (projectileDef == null)
                throw new InternalException(
                    ResponseCode.ProjectileInstanceFactory_DefinitionNotFound,
                    $"Projectile definition with ID: {definitionId} is not found in cache");

            // Fetch the raw relationships from the cache
            var relationshipData = entityRelationshipCache.GetBySourceID(definitionId) ?? Enumerable.Empty<EntityRelationship>();

            // Group by the relationship type, mapping into our dictionary structure
            var relationships = relationshipData
                .GroupBy(r => r.Type)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.TargetEntityID).ToList()
                );

            var instance = new ProjectileInstance(
                id: instanceId,
                definitionId: projectileDef.ID,
                collisionShape: CollisionShapeMapper.FromDefinition(projectileDef.Collision),
                roomSpatialId: roomSpatialId,
                layerZ: layerZ,
                position: position,
                movementVector: movementVector,
                appearance: MapAppearance(projectileDef.Appearance),
                entityInstanceOwnerId: entityInstanceOwnerId,
                sourceDefinitionId: sourceDefinitionId,
                duration: projectileDef.Duration,
                velocity: projectileDef.Velocity,
                relationships: relationships
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
        #endregion
    }
}