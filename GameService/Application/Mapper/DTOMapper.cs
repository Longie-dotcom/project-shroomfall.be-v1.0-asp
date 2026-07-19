using Application.Services.WorldService;
using AutoMapper;
using Contract.DTO.Abstraction;
using Contract.DTO.Common;
using Contract.DTO.Definition;
using Contract.DTO.Definition.EntityDomain.Component;
using Contract.DTO.Definition.IdentityDomain;
using Contract.DTO.Definition.LocalizationDomain;
using Contract.DTO.Definition.MetaDomain;
using Contract.DTO.Definition.WorldDomain;
using Contract.DTO.Runtime.EntityDomain;
using Contract.DTO.Runtime.EntityDomain.Component;
using Contract.DTO.Runtime.MetaDomain;
using Contract.DTO.Runtime.WorldDomain;
using Domain.Abstraction;
using Domain.Common;
using Domain.Definition;
using Domain.Definition.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.IdentityDomain;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.MetaDomain;
using Domain.Definition.WorldDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;
using Domain.Runtime.WorldDomain.Run;
using Domain.Runtime.WorldDomain.Spatial;

namespace Application.Mapper
{
    public class DTOMapper : Profile
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public DTOMapper()
        {
            // Common
            CreateMap<HSV, HSVDTO>();
            CreateMap<Vector2, Vector2DTO>();

            // ─────────────────────────────
            // DEFINITION to DTO
            // ─────────────────────────────
            // Identity Domain
            CreateMap<User, UserDTO>();

            // Version Domain
            CreateMap<DefinitionVersionLog, DefinitionVersionLogDTO>();

            // Localization Domain
            CreateMap<LocalizationEntry, LocalizationEntryDTO>();
            CreateMap<LocalizedText, LocalizedTextDTO>();
            CreateMap<Locale, LocaleDTO>();

            // Entity Domain
            CreateMap<EntityPresentationDefinition, EntityPresentationDefinitionDTO>();
            CreateMap<EntityDefinition, EntityDefinitionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));
            CreateMap<ComponentDefinition, ComponentDefinitionDTO>()
                .Include<AIDefinition, AIDefinitionDTO>()
                .Include<AppearanceDefinition, AppearanceDefinitionDTO>()
                .Include<CharacteristicDefinition, CharacteristicDefinitionDTO>()
                .Include<CollisionDefinition, CollisionDefinitionDTO>()
                .Include<InventoryDefinition, InventoryDefinitionDTO>()
                .Include<LifetimeDefinition, LifetimeDefinitionDTO>()
                .Include<ProjectileDefinition, ProjectileDefinitionDTO>()
                .Include<TriggeredEffectDefinition, TriggeredEffectDefinitionDTO>();

            CreateMap<AIDefinition, AIDefinitionDTO>();

            CreateMap<AppearanceDefinition, AppearanceDefinitionDTO>();

            CreateMap<CharacteristicDefinition, CharacteristicDefinitionDTO>();
            CreateMap<AttributeValue, AttributeValueDTO>();
            CreateMap<AttributeGrowthValue, AttributeGrowthValueDTO>();

            CreateMap<CollisionDefinition, CollisionDefinitionDTO>();

            CreateMap<InventoryDefinition, InventoryDefinitionDTO>();
            CreateMap<InventoryEntry, InventoryEntryDTO>();

            CreateMap<LifetimeDefinition, LifetimeDefinitionDTO>();

            CreateMap<ProjectileDefinition, ProjectileDefinitionDTO>();

            CreateMap<TriggeredEffectDefinition, TriggeredEffectDefinitionDTO>();

            // Meta Domain
            CreateMap<EffectPresentationDefinition, EffectPresentationDefinitionDTO>();
            CreateMap<EffectDefinition, EffectDefinitionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));

            CreateMap<ItemPresentationDefinition, ItemPresentationDefinitionDTO>();
            CreateMap<ConsumableConfig, ConsumableConfigDTO>();
            CreateMap<EquippableConfig, EquippableConfigDTO>();
            CreateMap<PlaceableConfig, PlaceableConfigDTO>();
            CreateMap<RangedConfig, RangedConfigDTO>();
            CreateMap<MeleeConfig, MeleeConfigDTO>();
            CreateMap<CostConfig, CostConfigDTO>();
            CreateMap<ItemDefinition, ItemDefinitionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));

            // World Domain
            CreateMap<CombatRunDefinition, CombatRunDefinitionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));
            CreateMap<RoomDefinition, RoomDefinitionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));
            CreateMap<RoomPresentationDefinition, RoomPresentationDefinitionDTO>();
            CreateMap<EntitySpawnRule, EntitySpawnRuleDTO>();
            CreateMap<Cell, CellDTO>();

            // ─────────────────────────────
            // RUNTIME to DTO
            // ─────────────────────────────
            // Entity Domain
            CreateMap<EntityInstance, EntityInstanceDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));
            CreateMap<ComponentInstance, ComponentInstanceDTO>()
                .Include<ActionInstance, ActionInstanceDTO>()
                .Include<AIInstance, AIInstanceDTO>()
                .Include<AppearanceInstance, AppearanceInstanceDTO>()
                .Include<CollisionInstance, CollisionInstanceDTO>()
                .Include<CharacteristicInstance, CharacteristicInstanceDTO>()
                .Include<EffectContainerInstance, EffectContainerInstanceDTO>()
                .Include<InventoryInstance, InventoryInstanceDTO>()
                .Include<LifetimeInstance, LifetimeInstanceDTO>()
                .Include<OwnershipInstance, OwnershipInstanceDTO>()
                .Include<ProjectileInstance, ProjectileInstanceDTO>()
                .Include<TransformInstance, TransformInstanceDTO>()
                .Include<TriggeredEffectInstance, TriggeredEffectInstanceDTO>()
                .Include<WorldItemPayloadInstance, WorldItemPayloadInstanceDTO>();

            CreateMap<ActionInstance, ActionInstanceDTO>();

            CreateMap<AIInstance, AIInstanceDTO>();

            CreateMap<AppearanceInstance, AppearanceInstanceDTO>();

            CreateMap<CollisionInstance, CollisionInstanceDTO>();

            CreateMap<CharacteristicInstance, CharacteristicInstanceDTO>()
                .ForMember(dest => dest.Cores, opt => opt.MapFrom(src =>
                    src.GetCores().Select(kvp => new AttributeValueInstanceDTO { AttributeType = kvp.Key, Value = kvp.Value })))
                .ForMember(dest => dest.Vitals, opt => opt.MapFrom(src =>
                    src.GetVitals().Select(kvp => new AttributeValueInstanceDTO { AttributeType = kvp.Key, Value = kvp.Value })));

            CreateMap<EffectContainerInstance, EffectContainerInstanceDTO>();

            CreateMap<InventoryInstance, InventoryInstanceDTO>();

            CreateMap<LifetimeInstance, LifetimeInstanceDTO>();

            CreateMap<OwnershipInstance, OwnershipInstanceDTO>();

            CreateMap<ProjectileInstance, ProjectileInstanceDTO>();

            CreateMap<TransformInstance, TransformInstanceDTO>();

            CreateMap<TriggeredEffectInstance, TriggeredEffectInstanceDTO>();

            CreateMap<WorldItemPayloadInstance, WorldItemPayloadInstanceDTO>();

            // Meta Domain
            CreateMap<EffectInstance, EffectInstanceDTO>();
            CreateMap<ItemInstance, ItemInstanceDTO>();

            // World Domain
            CreateMap<RoomSpatial, RoomSpatialDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));
            CreateMap<CombatRunInstance, CombatRunInstanceDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));
            CreateMap<RoomInstance, RoomInstanceDTO>();
        }
    }
}