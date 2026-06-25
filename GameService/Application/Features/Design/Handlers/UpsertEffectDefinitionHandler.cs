using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using Domain.Definition.MetaDomain;
using Domain.Shared;

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

            var effectRepo = relationalUoW.GetRepository<IEffectDefinitionRepository>();
            var existingEffect = await effectRepo.GetByIdAsync(dto.ID);

            if (existingEffect == null)
            {
                // CREATE FLOW 
                var localizedText = LocalizationFactory.ForEffect(dto.ID);
                var presentation = new EffectPresentationDefinition(localizedText, dto.ID);

                var effect = new EffectDefinition(
                    dto.ID,
                    dto.Type,
                    dto.AttributeType,
                    dto.SourceType,
                    dto.Value,
                    dto.Duration,
                    dto.Interval,
                    presentation);

                await effectRepo.AddAsync(effect);

                await localizationEntryFactory.PreSavePlaceholderKeysAsync(localizedText);
            }
            else
            {
                // UPDATE FLOW
                existingEffect.UpdateFields(
                    dto.Type,
                    dto.AttributeType,
                    dto.SourceType,
                    dto.Value,
                    dto.Duration,
                    dto.Interval);

                await effectRepo.UpdateAsync(existingEffect);
            }

            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}