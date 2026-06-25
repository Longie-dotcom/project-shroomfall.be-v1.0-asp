using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using Contract.DTO.Domain.Definition;
using Contract.Enum.EntityDomain;
using Domain.Definition.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Features.Design.Handlers
{
    public class UpsertEntityDefinitionHandler : IHandler<UpsertEntityDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly LocalizationEntryFactory localizationEntryFactory;
        private readonly DesignerComponentFactory designerComponentFactory;

        private static readonly Dictionary<Type, Type> DtoToDomainMapping = new()
        {
            { typeof(AIDefinitionDTO), typeof(AIDefinition) },
            { typeof(AppearanceDefinitionDTO), typeof(AppearanceDefinition) },
            { typeof(CollisionDefinitionDTO), typeof(CollisionDefinition) },
            { typeof(CharacteristicDefinitionDTO), typeof(CharacteristicDefinition) },
            { typeof(InteractableDefinitionDTO), typeof(InteractableDefinition) },
            { typeof(InventoryDefinitionDTO), typeof(InventoryDefinition) },
            { typeof(LifetimeDefinitionDTO), typeof(LifetimeDefinition) },
            { typeof(PortalDefinitionDTO), typeof(PortalDefinition) },
            { typeof(ProjectileDefinitionDTO), typeof(ProjectileDefinition) },
            { typeof(TriggeredEffectDefinitionDTO), typeof(TriggeredEffectDefinition) }
        };
        #endregion

        #region Properties
        #endregion
        public UpsertEntityDefinitionHandler(
            IRelationalUoW relationalUoW,
            LocalizationEntryFactory localizationEntryFactory,
            DesignerComponentFactory designerComponentFactory)
        {
            this.relationalUoW = relationalUoW;
            this.localizationEntryFactory = localizationEntryFactory;
            this.designerComponentFactory = designerComponentFactory;
        }

        #region Methods
        public async Task Handle(
            UpsertEntityDefinitionCommand command)
        {
            var dto = command.DTO;

            // Validate batch payload against target EntityType schemas baseline
            ValidateRequiredComponents(dto.Type, dto.Components);

            var entityRepo = relationalUoW.GetRepository<IEntityDefinitionRepository>();
            var existingEntity = await entityRepo.GetByIdAsync(dto.ID);

            // True Immutable Meta Rule: If the root definition metadata does not exist yet, build it once
            if (existingEntity == null)
            {
                var localizedText = LocalizationFactory.ForEntity(dto.ID);
                var presentation = new EntityPresentationDefinition(localizedText, dto.ID);
                var entity = new EntityDefinition(dto.ID, dto.Type, presentation);

                await entityRepo.AddAsync(entity);
                await localizationEntryFactory.PreSavePlaceholderKeysAsync(localizedText);
            }

            // Pipeline component definitions generation (Inner factory overwrites or inserts completely fresh components dynamically)
            foreach (var componentDto in dto.Components)
            {
                await designerComponentFactory.UpsertAndSaveAsync(componentDto, dto.ID);
            }

            await relationalUoW.SaveChangesAsync();
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

        private Type MapDtoToDomainType(
            ComponentDefinitionDTO dto)
        {
            if (DtoToDomainMapping.TryGetValue(dto.GetType(), out var domainType))
            {
                return domainType;
            }

            throw new InternalException(
                ApplicationCode.DesignHandlerCode.ComponentSignatureMappingFailed,
                $"Component metadata contract '{dto.GetType().Name}' cannot be translated to a target domain signature.");
        }
        #endregion
    }
}