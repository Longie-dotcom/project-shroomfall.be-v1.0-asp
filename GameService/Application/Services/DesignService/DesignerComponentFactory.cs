using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract.DTO.Domain.Definition;
using Contract.Enum.EntityDomain;
using Contract.Enum.MetaDomain.Effect;
using Contract.Enum.MetaDomain.Item;
using Domain.Abstraction;
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
        public async Task CreateAndSaveAsync(
            ComponentDefinitionDTO dto,
            string entityDefinitionId)
        {
            switch (dto)
            {
                case AIDefinitionDTO ai:
                    await CreateAIAsync(ai, entityDefinitionId);
                    break;
                case AppearanceDefinitionDTO appearance:
                    await CreateAppearanceAsync(appearance, entityDefinitionId);
                    break;
                case CollisionDefinitionDTO collision:
                    await CreateCollisionAsync(collision, entityDefinitionId);
                    break;
                case CharacteristicDefinitionDTO characteristic:
                    await CreateCharacteristicAsync(characteristic, entityDefinitionId);
                    break;
                case InventoryDefinitionDTO inventory:
                    await CreateInventoryAsync(inventory, entityDefinitionId);
                    break;
                case LifetimeDefinitionDTO lifetime:
                    await CreateLifeTimeAsync(lifetime, entityDefinitionId);
                    break;
                case ProjectileDefinitionDTO projectile:
                    await CreateProjectileAsync(projectile, entityDefinitionId);
                    break;
                case TriggeredEffectDefinitionDTO triggeredEffect:
                    await CreateTriggeredEffectAsync(triggeredEffect, entityDefinitionId);
                    break;
                case PortalDefinitionDTO portal:
                    await CreatePortalAsync(portal, entityDefinitionId);
                    break;
                case InteractableDefinitionDTO interactable:
                    await CreateInteractableAsync(interactable, entityDefinitionId);
                    break;
                default:
                    throw new InternalException(
                        ApplicationCode.DefinitionComponentFactoryCode.ComponentDefinitionNotSupported,
                        $"Component DTO type '{dto.GetType().Name}' is not supported by the designer factory.");
            }
        }

        private async Task CreateAIAsync(
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

            await relationalUoW.GetRepository<IAIDefinitionRepository>().AddAsync(component);
        }

        private async Task CreateAppearanceAsync(
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

            await relationalUoW.GetRepository<IAppearanceDefinitionRepository>().AddAsync(component);
        }

        private async Task CreateCollisionAsync(
            CollisionDefinitionDTO dto, string entityDefinitionId)
        {
            var shapeType = Enum.Parse<CollisionShapeType>(dto.ShapeType, true);
            var layer = Enum.Parse<CollisionLayer>(dto.Layer, true);
            var mask = Enum.Parse<CollisionLayer>(dto.Mask, true);

            var component = new CollisionDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                shapeType,
                dto.Width,
                dto.Height,
                dto.Radius,
                dto.IsBlocking,
                layer,
                mask,
                dto.OffsetX,
                dto.OffsetY
            );

            await relationalUoW.GetRepository<ICollisionDefinitionRepository>().AddAsync(component);
        }

        private async Task CreateCharacteristicAsync(
            CharacteristicDefinitionDTO dto, string entityDefinitionId)
        {
            var characteristicId = Guid.NewGuid();
            var characteristic = new CharacteristicDefinition(characteristicId, entityDefinitionId);

            var allAttributeValues = new List<AttributeValue>();
            var allGrowthValues = new List<AttributeGrowthValue>();

            foreach (var valDto in dto.AttributeValues)
            {
                var attrId = valDto.ID == Guid.Empty ? Guid.NewGuid() : valDto.ID;
                var attrType = Enum.Parse<AttributeType>(valDto.Type, true);

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
                    var growthId = growthDto.ID == Guid.Empty ? Guid.NewGuid() : growthDto.ID;
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

            var repo = relationalUoW.GetRepository<ICharacteristicDefinitionRepository>();
            await repo.AddAsync(characteristic);
            await repo.SaveAttributeValuesAsync(allAttributeValues);
            await repo.SaveAttributeGrowthValuesAsync(allGrowthValues);
        }

        private async Task CreateInventoryAsync(
            InventoryDefinitionDTO dto, string entityDefinitionId)
        {
            var inventoryId = Guid.NewGuid();
            var inventory = new InventoryDefinition(inventoryId, entityDefinitionId, dto.SlotCount);
            var defaultItems = new List<InventoryEntry>();

            foreach (var entryDto in dto.DefaultItems)
            {
                var entryId = entryDto.ID == Guid.Empty ? Guid.NewGuid() : entryDto.ID;
                var quality = Enum.Parse<ItemQuality>(entryDto.Quality, true);

                var entry = new InventoryEntry(
                    entryId,
                    entryDto.DefinitionID,
                    entryDto.Amount,
                    quality,
                    inventoryId
                );

                defaultItems.Add(entry);
            }

            var repo = relationalUoW.GetRepository<IInventoryDefinitionRepository>();
            await repo.AddAsync(inventory);
            await repo.SaveDefaultItemsAsync(defaultItems);
        }

        private async Task CreateLifeTimeAsync(
            LifetimeDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new LifetimeDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.Lifetime
            );

            await relationalUoW.GetRepository<ILifetimeDefinitionRepository>().AddAsync(component);
        }

        private async Task CreateProjectileAsync(
            ProjectileDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new ProjectileDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.Velocity
            );

            await relationalUoW.GetRepository<IProjectileDefinitionRepository>().AddAsync(component);
        }

        private async Task CreateTriggeredEffectAsync(
            TriggeredEffectDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new TriggeredEffectDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.EffectDefinitionIDs
            );

            await relationalUoW.GetRepository<ITriggeredEffectDefinitionRepository>().AddAsync(component);
        }

        private async Task CreatePortalAsync(
            PortalDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new PortalDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.LocalTriggerOffsetX,
                dto.LocalTriggerOffsetY
            );

            await relationalUoW.GetRepository<IPortalDefinitionRepository>().AddAsync(component);
        }

        private async Task CreateInteractableAsync(
            InteractableDefinitionDTO dto, string entityDefinitionId)
        {
            var component = new InteractableDefinition(
                Guid.NewGuid(),
                entityDefinitionId,
                dto.Type
            );

            await relationalUoW.GetRepository<IInteractableDefinitionRepository>().AddAsync(component);
        }
        #endregion
    }
}