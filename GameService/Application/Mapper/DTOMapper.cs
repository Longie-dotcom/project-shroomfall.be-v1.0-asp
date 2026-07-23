using Application.Services.WorldService.Creation;
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
            FromCommon();
            FromDefinition();
            FromRuntime();
        }

        #region Methods
        public void FromCommon()
        {
            // Common
            CreateMap<HSV, HSVDTO>();
            CreateMap<Vector2, Vector2DTO>();
        }

        public void FromDefinition()
        {
            // ─────────────────────────────
            // Entity Domain
            // ─────────────────────────────
            CreateMap<ComponentDefinition, ComponentDefinitionDTO>()
                .Include<AIDefinition, AIDefinitionDTO>()
                .Include<AppearanceDefinition, AppearanceDefinitionDTO>()
                .Include<CharacteristicDefinition, CharacteristicDefinitionDTO>()
                .Include<CollisionDefinition, CollisionDefinitionDTO>()
                .Include<InventoryDefinition, InventoryDefinitionDTO>()
                .Include<LifetimeDefinition, LifetimeDefinitionDTO>()
                .Include<ProjectileDefinition, ProjectileDefinitionDTO>()
                .Include<TriggeredEffectDefinition, TriggeredEffectDefinitionDTO>();

            // AI Definition
            CreateMap<AIDefinition, AIDefinitionDTO>();

            // Appearance Definition
            CreateMap<AppearanceDefinition, AppearanceDefinitionDTO>();

            // Collision Definition
            CreateMap<CollisionDefinition, CollisionDefinitionDTO>();

            // Characteristic Definition
            CreateMap<CharacteristicDefinition, CharacteristicDefinitionDTO>();
            CreateMap<AttributeValue, AttributeValueDTO>();
            CreateMap<AttributeGrowthValue, AttributeGrowthValueDTO>();

            // Inventory Definition
            CreateMap<InventoryDefinition, InventoryDefinitionDTO>();
            CreateMap<InventoryEntry, InventoryEntryDTO>();

            // Lifetime Definition
            CreateMap<LifetimeDefinition, LifetimeDefinitionDTO>();

            // Projectile Definition
            CreateMap<ProjectileDefinition, ProjectileDefinitionDTO>();

            // Triggered Effect Definition
            CreateMap<TriggeredEffectDefinition, TriggeredEffectDefinitionDTO>();

            // Entity
            CreateMap<EntityPresentationDefinition, EntityPresentationDefinitionDTO>();
            CreateMap<EntityDefinition, EntityDefinitionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));

            // ─────────────────────────────
            // Identity Domain
            // ─────────────────────────────
            CreateMap<User, UserDTO>();

            // ─────────────────────────────
            // Localization Domain
            // ─────────────────────────────
            CreateMap<LocalizationEntry, LocalizationEntryDTO>();
            CreateMap<LocalizedText, LocalizedTextDTO>();
            CreateMap<Locale, LocaleDTO>();

            // ─────────────────────────────
            // Entity Domain
            // ─────────────────────────────
            // Effect
            CreateMap<EffectPresentationDefinition, EffectPresentationDefinitionDTO>();
            CreateMap<EffectDefinition, EffectDefinitionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));

            // Item
            CreateMap<ConsumableConfig, ConsumableConfigDTO>();
            CreateMap<EquippableConfig, EquippableConfigDTO>();
            CreateMap<PlaceableConfig, PlaceableConfigDTO>();
            CreateMap<RangedConfig, RangedConfigDTO>();
            CreateMap<MeleeConfig, MeleeConfigDTO>();
            CreateMap<CostConfig, CostConfigDTO>();
            CreateMap<ItemPresentationDefinition, ItemPresentationDefinitionDTO>();
            CreateMap<ItemDefinition, ItemDefinitionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));

            // ─────────────────────────────
            // World Domain
            // ─────────────────────────────
            // Run
            CreateMap<CombatRunDefinition, CombatRunDefinitionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));
            
            // Room
            CreateMap<EntitySpawnRule, EntitySpawnRuleDTO>();
            CreateMap<Cell, CellDTO>();
            CreateMap<RoomPresentationDefinition, RoomPresentationDefinitionDTO>();
            CreateMap<RoomDefinition, RoomDefinitionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));

            // ─────────────────────────────
            // Definition Versioning
            // ─────────────────────────────
            CreateMap<DefinitionVersionLog, DefinitionVersionLogDTO>();
        }

        public void FromRuntime()
        {
            // ─────────────────────────────
            // Entity Domain
            // ─────────────────────────────
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

            // Action Instance
            CreateMap<ActionInstance, ActionInstanceDTO>();

            // AI Instance
            CreateMap<AIInstance, AIInstanceDTO>();

            // Appearance Instance
            CreateMap<AppearanceInstance, AppearanceInstanceDTO>();

            // Collision Instance
            CreateMap<CollisionInstance, CollisionInstanceDTO>();

            // Characteristic Instance
            CreateMap<CharacteristicInstance, CharacteristicInstanceDTO>()
                .ForMember(dest => dest.Cores, opt => opt.MapFrom(src => src.GetCores().Select(kvp => new AttributeValueInstanceDTO { AttributeType = kvp.Key, Value = kvp.Value })))
                .ForMember(dest => dest.Vitals, opt => opt.MapFrom(src => src.GetVitals().Select(kvp => new AttributeValueInstanceDTO { AttributeType = kvp.Key, Value = kvp.Value })));

            // Effect Container Instance
            CreateMap<EffectContainerInstance, EffectContainerInstanceDTO>();

            // Inventory Instance
            CreateMap<InventoryInstance, InventoryInstanceDTO>();

            // Lifetime Instance
            CreateMap<LifetimeInstance, LifetimeInstanceDTO>();

            // Ownership Instance
            CreateMap<OwnershipInstance, OwnershipInstanceDTO>();

            // Projectile Instance
            CreateMap<ProjectileInstance, ProjectileInstanceDTO>();

            // Transform Instance
            CreateMap<TransformInstance, TransformInstanceDTO>();

            // Triggered Effect Instance
            CreateMap<TriggeredEffectInstance, TriggeredEffectInstanceDTO>();

            // World Item Payload Instance
            CreateMap<WorldItemPayloadInstance, WorldItemPayloadInstanceDTO>();

            // Entity
            CreateMap<EntityInstance, EntityInstanceDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));

            // ─────────────────────────────
            // Meta Domain
            // ─────────────────────────────
            // Effect
            CreateMap<EffectInstance, EffectInstanceDTO>();

            // Item
            CreateMap<ItemInstance, ItemInstanceDTO>();

            // ─────────────────────────────
            // World Domain
            // ─────────────────────────────
            // Run
            CreateMap<CombatRunInstance, CombatRunInstanceDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));

            // Room
            CreateMap<RoomSpatial, RoomSpatialDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));
            CreateMap<RoomInstance, RoomInstanceDTO>();
        }
        #endregion
    }
}