using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract.DTO.Domain.Definition;
using Contract.Enum.EntityDomain;
using Domain.Common;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Services.DesignService
{
    public class DesignerComponentFactory
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        #endregion

        #region Properties
        #endregion

        public DesignerComponentFactory(
            IRelationalUoW relationalUoW)
        {
            this.relationalUoW = relationalUoW;
        }

        #region Methods
        public async Task UpsertAndSaveAsync(
            ComponentDefinitionDTO dto,
            EntityType entityType,
            string entityDefinitionId)
        {
            // Route processing steps explicitly via the string contract, matching ComponentStringToDomainMapping philosophy
            switch (dto.ComponentType)
            {
                case nameof(AIDefinitionDTO):
                    await UpsertAIAsync((AIDefinitionDTO)dto, entityDefinitionId);
                    break;
                case nameof(AppearanceDefinitionDTO):
                    await UpsertAppearanceAsync((AppearanceDefinitionDTO)dto, entityDefinitionId);
                    break;
                case nameof(CollisionDefinitionDTO):
                    await UpsertCollisionAsync((CollisionDefinitionDTO)dto, entityDefinitionId, entityType);
                    break;
                case nameof(CharacteristicDefinitionDTO):
                    await UpsertCharacteristicAsync((CharacteristicDefinitionDTO)dto, entityDefinitionId);
                    break;
                case nameof(InteractableDefinitionDTO):
                    await UpsertInteractableAsync((InteractableDefinitionDTO)dto, entityDefinitionId);
                    break;
                case nameof(InventoryDefinitionDTO):
                    await UpsertInventoryAsync((InventoryDefinitionDTO)dto, entityDefinitionId);
                    break;
                case nameof(LifetimeDefinitionDTO):
                    await UpsertLifeTimeAsync((LifetimeDefinitionDTO)dto, entityDefinitionId);
                    break;
                case nameof(PortalDefinitionDTO):
                    await UpsertPortalAsync((PortalDefinitionDTO)dto, entityDefinitionId);
                    break;
                case nameof(ProjectileDefinitionDTO):
                    await UpsertProjectileAsync((ProjectileDefinitionDTO)dto, entityDefinitionId);
                    break;
                case nameof(TriggeredEffectDefinitionDTO):
                    await UpsertTriggeredEffectAsync((TriggeredEffectDefinitionDTO)dto, entityDefinitionId);
                    break;
                default:
                    throw new InternalException(
                        ApplicationCode.DesignHandlerCode.ComponentDTOMappingFailed,
                        $"Component payload identifier contract '{dto.ComponentType}' is unrecognized by the execution pipeline factory.");
            }
        }

        private async Task UpsertAIAsync(
            AIDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new AIDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.LeashDistance,
                dto.AggroRadius,
                dto.ThinkInterval,
                dto.IsAIControlled
            );

            await relationalUoW.GetRepository<IAIDefinitionRepository>().UpsertAsync(component);
        }

        private async Task UpsertAppearanceAsync(
            AppearanceDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new AppearanceDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.SkinID,
                new HSV(dto.SkinColor.H, dto.SkinColor.S, dto.SkinColor.V)
            );

            await relationalUoW.GetRepository<IAppearanceDefinitionRepository>().UpsertAsync(component);
        }

        private async Task UpsertCollisionAsync(
            CollisionDefinitionDTO dto,
            string entityDefinitionId,
            EntityType entityType)
        {
            var finalLayer = dto.Layer;
            var finalMask = dto.Mask;

            // Fallback Preset Engine: If frontend passes 0/None, infer correct defaults from the concrete EntityType
            if (finalLayer == CollisionLayer.None || finalMask == CollisionLayer.None)
            {
                switch (entityType)
                {
                    case EntityType.Player:
                        finalLayer = CollisionLayer.Player;
                        finalMask = CollisionPresets.PlayerMask;
                        break;

                    case EntityType.Creature:
                        finalLayer = CollisionLayer.Enemy;
                        finalMask = CollisionPresets.EnemyMask;
                        break;

                    case EntityType.WorldObject:
                        finalLayer = CollisionLayer.Wall;
                        finalMask = CollisionPresets.WallMask;
                        break;

                    case EntityType.Projectile:
                        // Defaulting to Player-sourced projectiles; game rules can override manually via frontend
                        finalLayer = CollisionLayer.PlayerProjectile;
                        finalMask = CollisionPresets.PlayerProjectileMask;
                        break;

                    case EntityType.AreaEffect:
                    case EntityType.Portal:
                        finalLayer = CollisionLayer.TriggerZone;
                        finalMask = CollisionPresets.TriggerZoneMask;
                        break;

                    case EntityType.Item:
                        finalLayer = CollisionLayer.Collectible;
                        finalMask = CollisionPresets.CollectibleMask;
                        break;

                    default:
                        // No mapping found: maintain structural values as passed from client
                        break;
                }
            }

            var component = new CollisionDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.ShapeType,
                dto.Width,
                dto.Height,
                dto.Radius,
                dto.IsBlocking,
                finalLayer,
                finalMask,
                dto.OffsetX,
                dto.OffsetY
            );

            await relationalUoW.GetRepository<ICollisionDefinitionRepository>().UpsertAsync(component);
        }

        private async Task UpsertCharacteristicAsync(
            CharacteristicDefinitionDTO dto, string entityDefinitionId)
        {
            var repo = relationalUoW.GetRepository<ICharacteristicDefinitionRepository>();
            var existing = await repo.GetByEntityIdAsync(entityDefinitionId);

            if (existing != null)
            {
                // Core Rule: Deep nested components must strip child nodes explicitly before the main record swaps
                await repo.ReplaceAttributeValuesAsync(existing.ID, new List<AttributeValue>());
            }

            var characteristicId = Guid.NewGuid();
            var characteristic = new CharacteristicDefinition(characteristicId, entityDefinitionId);

            var allAttributeValues = new List<AttributeValue>();
            var allGrowthValues = new List<AttributeGrowthValue>();

            foreach (var valDto in dto.AttributeValues)
            {
                var attrId = Guid.NewGuid();
                var attrType = valDto.Type;

                var attributeValue = new AttributeValue(
                    attrId,
                    attrType,
                    valDto.BaseValue,
                    valDto.Level,
                    valDto.Min,
                    valDto.Max,
                    characteristicId
                );

                foreach (var growthDto in valDto.AttributeGrowthValues)
                {
                    var growthId = Guid.NewGuid();
                    var growthValue = new AttributeGrowthValue(
                        growthId,
                        growthDto.Level,
                        growthDto.GrowthValue,
                        attrId
                    );
                    allGrowthValues.Add(growthValue);
                }

                allAttributeValues.Add(attributeValue);
            }

            // Using generic upsert method for root, then appending the custom mapped sub-collections
            await repo.UpsertAsync(characteristic);
            await repo.SaveAttributeValuesAsync(allAttributeValues);
            await repo.SaveAttributeGrowthValuesAsync(allGrowthValues);
        }

        private async Task UpsertInteractableAsync(
            InteractableDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new InteractableDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.Type
            );

            await relationalUoW.GetRepository<IInteractableDefinitionRepository>().UpsertAsync(component);
        }

        private async Task UpsertInventoryAsync(
            InventoryDefinitionDTO dto, string entityDefinitionId)
        {
            var repo = relationalUoW.GetRepository<IInventoryDefinitionRepository>();
            var existing = await repo.GetByEntityIdAsync(entityDefinitionId);

            if (existing != null)
            {
                // Purge sub-collection properties ahead of root swap execution
                await repo.ReplaceDefaultItemsAsync(existing.ID, new List<InventoryEntry>());
            }

            var inventoryId = Guid.NewGuid();
            var inventory = new InventoryDefinition(inventoryId, entityDefinitionId, dto.SlotCount);
            var defaultItems = new List<InventoryEntry>();

            foreach (var entryDto in dto.DefaultItems)
            {
                var entryId = Guid.NewGuid();

                var entry = new InventoryEntry(
                    entryId,
                    entryDto.DefinitionID,
                    entryDto.Amount,
                    entryDto.Quality,
                    inventoryId
                );

                defaultItems.Add(entry);
            }

            await repo.UpsertAsync(inventory);
            await repo.SaveDefaultItemsAsync(defaultItems);
        }

        private async Task UpsertLifeTimeAsync(
            LifetimeDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new LifetimeDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.Lifetime
            );

            await relationalUoW.GetRepository<ILifetimeDefinitionRepository>().UpsertAsync(component);
        }

        private async Task UpsertPortalAsync(
            PortalDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new PortalDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.LocalTriggerOffsetX,
                dto.LocalTriggerOffsetY
            );

            await relationalUoW.GetRepository<IPortalDefinitionRepository>().UpsertAsync(component);
        }

        private async Task UpsertProjectileAsync(
            ProjectileDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new ProjectileDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.Velocity
            );

            await relationalUoW.GetRepository<IProjectileDefinitionRepository>().UpsertAsync(component);
        }

        private async Task UpsertTriggeredEffectAsync(
            TriggeredEffectDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new TriggeredEffectDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.EffectDefinitionIDs
            );

            await relationalUoW.GetRepository<ITriggeredEffectDefinitionRepository>().UpsertAsync(component);
        }
        #endregion
    }
}