using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using Contract.DTO.Definition.MetaDomain;
using Contract.Enum.MetaDomain.Item;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.MetaDomain;
using Domain.DomainException;
using ResponseCode;

namespace Application.Features.Design.Handlers
{
    public class UpsertItemDefinitionHandler : IHandler<UpsertItemDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly LocalizationEntryFactory localizationEntryFactory;
        #endregion

        #region Properties
        #endregion

        public UpsertItemDefinitionHandler(
            IRelationalUoW relationalUoW,
            LocalizationEntryFactory localizationEntryFactory)
        {
            this.relationalUoW = relationalUoW;
            this.localizationEntryFactory = localizationEntryFactory;
        }

        #region Methods
        public async Task Handle(
            UpsertItemDefinitionCommand command)
        {
            var dto = command.DTO;

            // Validate upsert DTO
            ValidateCategoryConfigurations(dto);

            var itemRepo = relationalUoW.GetRepository<IItemDefinitionRepository>();
            var existingItem = await itemRepo.GetByIdAsync(dto.Id);

            // Build category-specific configuration structures
            ConsumableConfig? consumableConfig = null;
            if (dto.ConsumableConfig != null)
            {
                consumableConfig = new ConsumableConfig
                {
                    EffectDefinitionIDs = dto.ConsumableConfig.EffectDefinitionIDs
                };
            }

            EquippableConfig? equippableConfig = null;
            if (dto.EquippableConfig != null)
            {
                equippableConfig = new EquippableConfig
                {
                    Slot = dto.EquippableConfig.Slot,
                    EffectDefinitionIDs = dto.EquippableConfig.EffectDefinitionIDs
                };
            }

            PlaceableConfig? placeableConfig = null;
            if (dto.PlaceableConfig != null)
            {
                placeableConfig = new PlaceableConfig
                {
                    EntityDefinitionID = dto.PlaceableConfig.EntityDefinitionID
                };
            }

            RangedConfig? rangedConfig = null;
            if (dto.RangedConfig != null)
            {
                rangedConfig = new RangedConfig
                {
                    EntityDefinitionID = dto.RangedConfig.EntityDefinitionID
                };
            }

            MeleeConfig? meleeConfig = null;
            if (dto.MeleeConfig != null)
            {
                meleeConfig = new MeleeConfig
                {
                    EntityDefinitionID = dto.MeleeConfig.EntityDefinitionID
                };
            }

            var costConfig = new CostConfig
            {
                Method = dto.CostConfig.Method,
            };

            if (existingItem == null)
            {
                // CREATE FLOW (Set identity, presentation, and icons ONCE)
                var localizedText = ForItem(dto.Id);
                var presentation = new ItemPresentationDefinition(localizedText, dto.Id);

                var item = new ItemDefinition(
                    dto.Id,
                    dto.Type,
                    dto.Category,
                    dto.MaxStack,
                    dto.MaxDurability,
                    dto.TriggeredAction,
                    presentation,
                    consumableConfig,
                    equippableConfig,
                    placeableConfig,
                    rangedConfig,
                    meleeConfig,
                    costConfig
                );

                await itemRepo.AddAsync(item);
                await localizationEntryFactory.PreSavePlaceholderKeysAsync(localizedText);
            }
            else
            {
                // UPDATE FLOW 
                existingItem.UpdateFields(
                    dto.Type,
                    dto.Category,
                    dto.MaxStack,
                    dto.MaxDurability,
                    dto.TriggeredAction,
                    consumableConfig,
                    equippableConfig,
                    placeableConfig,
                    rangedConfig,
                    meleeConfig,
                    costConfig
                );

                await itemRepo.UpdateAsync(existingItem);
            }

            // Flush changes down to your database engine context securely
            await relationalUoW.SaveChangesAsync();
        }

        private void ValidateCategoryConfigurations(
            ItemDefinitionDTO dto)
        {
            var category = dto.Category;
            AssertConfigRule(nameof(dto.ConsumableConfig), category == ItemCategory.Consumable, dto.ConsumableConfig != null);
            AssertConfigRule(nameof(dto.EquippableConfig), category == ItemCategory.Equippable, dto.EquippableConfig != null);
            AssertConfigRule(nameof(dto.PlaceableConfig), category == ItemCategory.Placeable, dto.PlaceableConfig != null);
            AssertConfigRule(nameof(dto.RangedConfig), category == ItemCategory.Ranged, dto.RangedConfig != null);
            AssertConfigRule(nameof(dto.MeleeConfig), category == ItemCategory.Melee, dto.MeleeConfig != null);
        }

        private void AssertConfigRule(
            string configName, 
            bool isTargetCategory,
            bool hasConfig)
        {
            // Triggered if: 
            // 1. It IS the target category but lacks the config (Missing)
            // 2. It IS NOT the target category but has the config (Conflict)
            if ((isTargetCategory && !hasConfig) || (!isTargetCategory && hasConfig))
            {
                var code = isTargetCategory
                    ? ApplicationCode.DesignHandlerCode.ItemCategoryConfigMissing
                    : ApplicationCode.DesignHandlerCode.ItemCategoryConfigConflict;

                var message = isTargetCategory
                    ? $"{configName} configuration is required for this item category."
                    : $"{configName} configuration conflicts with this item category.";

                throw new BadRequest(code, message);
            }
        }

        private static LocalizedText ForItem(
            string itemId)
        {
            itemId = string.IsNullOrWhiteSpace(itemId) ? "unknown" : itemId.Trim().ToLowerInvariant();

            return new LocalizedText
            {
                NameKey = $"item.{itemId}.name",
                DescriptionKey = $"item.{itemId}.description"
            };
        }
        #endregion
    }
}