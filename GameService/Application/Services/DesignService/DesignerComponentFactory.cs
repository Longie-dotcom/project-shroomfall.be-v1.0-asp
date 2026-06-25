using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract.DTO.Domain.Definition;
using Domain.Common;
using Domain.Definition.EntityDomain.Component;
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
            string entityDefinitionId)
        {
            switch (dto)
            {
                case AIDefinitionDTO ai:
                    await UpsertAIAsync(ai, entityDefinitionId);
                    break;
                case AppearanceDefinitionDTO appearance:
                    await UpsertAppearanceAsync(appearance, entityDefinitionId);
                    break;
                case CollisionDefinitionDTO collision:
                    await UpsertCollisionAsync(collision, entityDefinitionId);
                    break;
                case CharacteristicDefinitionDTO characteristic:
                    await UpsertCharacteristicAsync(characteristic, entityDefinitionId);
                    break;
                case InteractableDefinitionDTO interactable:
                    await UpsertInteractableAsync(interactable, entityDefinitionId);
                    break;
                case InventoryDefinitionDTO inventory:
                    await UpsertInventoryAsync(inventory, entityDefinitionId);
                    break;
                case LifetimeDefinitionDTO lifetime:
                    await UpsertLifeTimeAsync(lifetime, entityDefinitionId);
                    break;
                case PortalDefinitionDTO portal:
                    await UpsertPortalAsync(portal, entityDefinitionId);
                    break;
                case ProjectileDefinitionDTO projectile:
                    await UpsertProjectileAsync(projectile, entityDefinitionId);
                    break;
                case TriggeredEffectDefinitionDTO triggeredEffect:
                    await UpsertTriggeredEffectAsync(triggeredEffect, entityDefinitionId);
                    break;
                default:
                    throw new InternalException(
                        ApplicationCode.DesignHandlerCode.ComponentSignatureMappingFailed,
                        $"Component DTO type '{dto.GetType().Name}' is not supported by the designer factory.");
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
            var skinColor = new HSV(dto.SkinColor.H, dto.SkinColor.S, dto.SkinColor.V);
            var hairColor = dto.HairColor != null ? new HSV(dto.HairColor.H, dto.HairColor.S, dto.HairColor.V) : new HSV();
            var pantColor = dto.PantColor != null ? new HSV(dto.PantColor.H, dto.PantColor.S, dto.PantColor.V) : new HSV();

            var component = new AppearanceDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.SkinID,
                skinColor,
                dto.HairID,
                dto.EyesID,
                dto.ShirtID,
                dto.PantID,
                hairColor,
                pantColor
            );

            await relationalUoW.GetRepository<IAppearanceDefinitionRepository>().UpsertAsync(component);
        }

        private async Task UpsertCollisionAsync(
            CollisionDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new CollisionDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.ShapeType,
                dto.Width,
                dto.Height,
                dto.Radius,
                dto.IsBlocking,
                dto.Layer,
                dto.Mask,
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