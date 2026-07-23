using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.MetaDomain;

namespace Application.Features.Design.Handlers
{
    public class UpsertEffectDefinitionHandler : IHandler<UpsertEffectDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly LocalizationEntryFactory localizationEntryFactory;
        #endregion

        #region Properties
        #endregion

        public UpsertEffectDefinitionHandler(
            IRelationalUoW relationalUoW,
            LocalizationEntryFactory localizationEntryFactory)
        {
            this.relationalUoW = relationalUoW;
            this.localizationEntryFactory = localizationEntryFactory;
        }

        #region Methods
        public async Task Handle(
            UpsertEffectDefinitionCommand command)
        {
            var dto = command.DTO;
            
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

            // Apply persistence
            await relationalUoW.SaveChangesAsync();
        }

        public static LocalizedText ForEffect(
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