using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using Contract.DTO.Abstraction;
using Contract.DTO.Definition.EntityDomain.Component;
using Contract.Enum.EntityDomain;
using Domain.Definition.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.DomainException;
using Domain.Shared;
using ResponseCode;

namespace Application.Features.Design.Handlers
{
    public class UpsertEntityDefinitionHandler : IHandler<UpsertEntityDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly LocalizationEntryFactory localizationEntryFactory;
        private readonly DefinitionComponentFactory definitionComponentFactory;

        private static readonly Dictionary<string, Type> ComponentStringToDomainMapping = new(StringComparer.OrdinalIgnoreCase)
        {
            { nameof(AIDefinitionDTO), typeof(AIDefinition) },
            { nameof(AppearanceDefinitionDTO), typeof(AppearanceDefinition) },
            { nameof(CollisionDefinitionDTO), typeof(CollisionDefinition) },
            { nameof(CharacteristicDefinitionDTO), typeof(CharacteristicDefinition) },
            { nameof(InventoryDefinitionDTO), typeof(InventoryDefinition) },
            { nameof(LifetimeDefinitionDTO), typeof(LifetimeDefinition) },
            { nameof(ProjectileDefinitionDTO), typeof(ProjectileDefinition) },
            { nameof(TriggeredEffectDefinitionDTO), typeof(TriggeredEffectDefinition) }
        };
        #endregion

        #region Properties
        #endregion
        public UpsertEntityDefinitionHandler(
            IRelationalUoW relationalUoW,
            LocalizationEntryFactory localizationEntryFactory,
            DefinitionComponentFactory definitionComponentFactory)
        {
            this.relationalUoW = relationalUoW;
            this.localizationEntryFactory = localizationEntryFactory;
            this.definitionComponentFactory = definitionComponentFactory;
        }

        #region Methods
        public async Task Handle(
            UpsertEntityDefinitionCommand command)
        {
            var dto = command.DTO;

            if (dto.Components == null)
                return;

            // Validate batch payload against target EntityType schemas baseline
            ValidateRequiredComponents(dto.Type, dto.Components);

            var entityRepo = relationalUoW.GetRepository<IEntityDefinitionRepository>();
            var existingEntity = await entityRepo.GetByIdAsync(dto.Id);

            if (existingEntity == null)
            {
                var localizedText = LocalizationFactory.ForEntity(dto.Id);
                var presentation = new EntityPresentationDefinition(localizedText, dto.Id);
                var entity = new EntityDefinition(dto.Id, dto.Type, presentation);

                await entityRepo.AddAsync(entity);
                await localizationEntryFactory.PreSavePlaceholderKeysAsync(localizedText);
            }

            // Pipeline component definitions generation
            foreach (var componentDto in dto.Components)
            {
                await definitionComponentFactory.UpsertAndSaveAsync(componentDto, dto.Type, dto.Id);
            }

            int rowsAffected = await relationalUoW.SaveChangesAsync();
        }

        private void ValidateRequiredComponents(
            EntityType type,
            List<ComponentDefinitionDTO> providedComponents)
        {
            var requiredDomainTypes = EntityDefinitionSchemas.GetRequiredComponentTypes(type).ToList();
            var providedDomainTypes = providedComponents.Select(MapDtoToDomainType).ToList();

            // Intersect layout configurations discrepancies
            var missingComponents = requiredDomainTypes.Except(providedDomainTypes).ToList();

            if (missingComponents.Any())
            {
                var missingNames = string.Join(", ", missingComponents.Select(t => t.Name));
                throw new InternalException(
                    ApplicationCode.DesignHandlerCode.MandatorySchemaElementsMissing,
                    $"Failed to build entity setup variant '{type}'. Missing mandatory schema elements: {missingNames}");
            }
        }

        private Type MapDtoToDomainType(ComponentDefinitionDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ComponentType))
            {
                throw new InternalException(
                    ApplicationCode.DesignHandlerCode.ComponentSignatureNotFound,
                    "Component Definition DTO is missing its string identifier ComponentType.");
            }

            if (ComponentStringToDomainMapping.TryGetValue(dto.ComponentType, out var domainType))
            {
                return domainType;
            }

            throw new InternalException(
                ApplicationCode.DesignHandlerCode.ComponentSignatureMappingFailed,
                $"Component metadata contract '{dto.ComponentType}' cannot be translated to a target domain signature.");
        }
        #endregion
    }
}