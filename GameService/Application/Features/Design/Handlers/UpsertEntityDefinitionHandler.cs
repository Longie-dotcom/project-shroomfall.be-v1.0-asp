using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using Contract.DTO.Abstraction;
using Contract.Enum.EntityDomain;
using Domain.Definition.EntityDomain;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using ResponseCode;

namespace Application.Features.Design.Handlers
{
    public class UpsertEntityDefinitionHandler : IHandler<UpsertEntityDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly LocalizationEntryFactory localizationEntryFactory;
        private readonly DefinitionComponentFactory definitionComponentFactory;
        private readonly ComponentDiscoveryRegistry discoveryRegistry;
        #endregion

        #region Properties
        #endregion
        public UpsertEntityDefinitionHandler(
            IRelationalUoW relationalUoW,
            LocalizationEntryFactory localizationEntryFactory,
            DefinitionComponentFactory definitionComponentFactory,
            ComponentDiscoveryRegistry discoveryRegistry)
        {
            this.relationalUoW = relationalUoW;
            this.localizationEntryFactory = localizationEntryFactory;
            this.definitionComponentFactory = definitionComponentFactory;
            this.discoveryRegistry = discoveryRegistry;
        }

        #region Methods
        public async Task Handle(
            UpsertEntityDefinitionCommand command)
        {
            var dto = command.DTO;

            // Validate batch payload against target EntityType schemas baseline
            ValidateRequiredComponents(dto.Type, dto.Components);

            // Upsert flow (Create flow)
            var entityRepo = relationalUoW.GetRepository<IEntityDefinitionRepository>();
            var existingEntity = await entityRepo.GetByIdAsync(dto.Id);
            if (existingEntity == null)
            {
                var localizedText = ForEntity(dto.Id);
                var presentation = new EntityPresentationDefinition(localizedText, dto.Id);
                var entity = new EntityDefinition(dto.Id, dto.Type, presentation);

                await entityRepo.AddAsync(entity);
                await localizationEntryFactory.PreSavePlaceholderKeysAsync(localizedText);
            }

            // Upsert flow (Update flow: Pipeline component definitions generation)
            foreach (var componentDto in dto.Components)
            {
                await definitionComponentFactory.UpsertAndSaveAsync(componentDto, dto.Type, dto.Id);
            }

            // Apply persistence
            int rowsAffected = await relationalUoW.SaveChangesAsync();
        }

        private void ValidateRequiredComponents(
            EntityType type,
            List<ComponentDefinitionDTO> providedComponents)
        {
            var requiredComponents = discoveryRegistry
                .GetComponents()
                .Where(x => x.SupportedEntityTypes.Contains(type))
                .ToList();

            var missing = requiredComponents
                .Where(required => !providedComponents
                    .Any(dto => string.Equals(dto.ComponentType, required.Id, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (missing.Any())
                throw new InternalException(
                    ApplicationCode.DesignHandlerCode.MandatorySchemaElementsMissing,
                    $"Failed to build entity setup variant '{type}'. Missing mandatory schema elements: {string.Join(", ", missing.Select(x => x.Id))}");
        }

        private static LocalizedText ForEntity(
            string entityId)
        {
            entityId = string.IsNullOrWhiteSpace(entityId) ? "unknown" : entityId.Trim().ToLowerInvariant();

            return new LocalizedText
            {
                NameKey = $"entity.{entityId}.name",
                DescriptionKey = $"entity.{entityId}.description"
            };
        }
        #endregion
    }
}