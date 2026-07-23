using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract.DTO.Definition.MetaDomain;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.MetaDomain;

namespace Application.Services.DesignService
{
    public class EffectDefinitionService
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly LocalizationEntryFactory localizationEntryFactory;
        #endregion

        #region Properties
        #endregion

        public EffectDefinitionService(
            IRelationalUoW relationalUoW,
            LocalizationEntryFactory localizationEntryFactory)
        {
            this.relationalUoW = relationalUoW;
            this.localizationEntryFactory = localizationEntryFactory;
        }

        #region Methods
        public async Task UpsertWithoutSave(
            EffectDefinitionDTO dto)
        {
            // Upsert flow
            var effectRepo = relationalUoW.GetRepository<IEffectDefinitionRepository>();
            var existingEffect = await effectRepo.GetByIdAsync(dto.Id);
            if (existingEffect == null)
            {
                // CREATE FLOW (Prepare the localization entries and presentation)
                var localizedText = ForEffect(dto.Id);
                var presentation = new EffectPresentationDefinition(localizedText, dto.Id);
                var effect = new EffectDefinition(
                    dto.Id,
                    dto.Type,
                    dto.AttributeType,
                    dto.Value,
                    dto.Duration,
                    dto.Interval,
                    presentation);

                await effectRepo.AddAsync(effect);
                await localizationEntryFactory.PreSavePlaceholderKeysAsync(localizedText);
            }
            else
            {
                // UPDATE FLOW (Exclude localization and presentation)
                existingEffect.UpdateFields(
                    dto.Type,
                    dto.AttributeType,
                    dto.Value,
                    dto.Duration,
                    dto.Interval);

                await effectRepo.UpdateAsync(existingEffect);
            }
        }

        private static LocalizedText ForEffect(
            string effectId)
        {
            effectId = string.IsNullOrWhiteSpace(effectId) ? "unknown" : effectId.Trim().ToLowerInvariant();

            return new LocalizedText
            {
                NameKey = $"effect.{effectId}.name",
                DescriptionKey = $"effect.{effectId}.description"
            };
        }
        #endregion
    }
}
