using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using Domain.Definition.MetaDomain;
using Domain.Shared;

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
        public async Task Handle(UpsertItemDefinitionCommand command)
        {
            var dto = command.DTO;

            var itemRepo = relationalUoW.GetRepository<IItemDefinitionRepository>();
            var existingItem = await itemRepo.GetByIdAsync(dto.ID);

            // Build config structures using their properties as defined in the domain
            SpawnEntityConfig? spawnConfig = null;
            if (dto.SpawnEntityConfig != null)
            {
                spawnConfig = new SpawnEntityConfig
                {
                    EntityDefinitionID = dto.SpawnEntityConfig.EntityDefinitionID,
                    TargetType = dto.SpawnEntityConfig.TargetType,
                    MaxRange = dto.SpawnEntityConfig.MaxRange
                };
            }

            ApplyEffectConfig? applyEffectConfig = null;
            if (dto.ApplyEffectConfig != null)
            {
                applyEffectConfig = new ApplyEffectConfig
                {
                    EffectDefinitionIDs = dto.ApplyEffectConfig.EffectDefinitionIDs
                };
            }

            EquipConfig? equipConfig = null;
            if (dto.EquipConfig != null)
            {
                equipConfig = new EquipConfig
                {
                    Slot = dto.EquipConfig.Slot
                };
            }

            var costConfig = new CostConfig
            {
                Method = dto.CostConfig.Method,
                Value = dto.CostConfig.Value
            };

            if (existingItem == null)
            {
                // CREATE FLOW (Set identity, presentation, and icons ONCE)
                var localizedText = LocalizationFactory.ForItem(dto.ID);
                var presentation = new ItemPresentationDefinition(localizedText, dto.ID);

                var item = new ItemDefinition(
                    dto.ID,
                    dto.Type,
                    dto.Category,
                    dto.MaxStack,
                    dto.MaxDurability,
                    dto.TriggeredAction,
                    presentation,
                    spawnConfig,
                    applyEffectConfig,
                    equipConfig,
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
                    spawnConfig,
                    applyEffectConfig,
                    equipConfig,
                    costConfig
                );

                await itemRepo.UpdateAsync(existingItem);
            }

            // Flush changes down to your database engine context securely
            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}