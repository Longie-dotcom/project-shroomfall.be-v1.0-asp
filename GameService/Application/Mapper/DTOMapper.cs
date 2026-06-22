using AutoMapper;
using Contract.DTO.Common;
using Contract.DTO.Design;
using Contract.DTO.Domain.Definition;
using Contract.DTO.Domain.Runtime;
using Contract.DTO.Identity;
using Domain.Abstraction;
using Domain.Common;
using Domain.Definition;
using Domain.Definition.EntityDomain;
using Domain.Definition.IdentityDomain;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.MetaDomain;
using Domain.Definition.WorldDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Spatial;
using Domain.Runtime.WorldDomain.Topology;
using Domain.Shared;

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
            CreateMap<EntityDefinition, EntityDefinitionDTO>();

            // Meta Domain
            CreateMap<AttributeDefinition, AttributeDefinitionDTO>();

            CreateMap<EffectPresentationDefinition, EffectPresentationDefinitionDTO>();
            CreateMap<EffectDefinition, EffectDefinitionDTO>();

            CreateMap<ItemPresentationDefinition, ItemPresentationDefinitionDTO>();
            CreateMap<SpawnEntityConfig, SpawnEntityConfigDTO>();
            CreateMap<ApplyEffectConfig, ApplyEffectConfigDTO>();
            CreateMap<EquipConfig, EquipConfigDTO>();
            CreateMap<CostConfig, CostConfigDTO>();
            CreateMap<ItemDefinition, ItemDefinitionDTO>();

            // World Domain
            CreateMap<Cell, CellDefinitionDTO>();
            CreateMap<EntitySpawnRule, EntitySpawnRuleDefinitionDTO>();
            CreateMap<RoomConnection, RoomConnectionDefinitionDTO>();
            CreateMap<RoomPresentationDefinition, RoomPresentationDefinitionDTO>();
            CreateMap<RoomDefinition, RoomDefinitionDTO>();

            // ─────────────────────────────
            // RUNTIME to DTO
            // ─────────────────────────────
            // Entity Domain
            CreateMap<EntityInstance, EntityInstanceDTO>();
            CreateMap<ComponentInstance, ComponentInstanceDTO>()
                .Include<ActionInstance, ActionInstanceDTO>()
                .Include<AIInstance, AIInstanceDTO>()
                .Include<AppearanceInstance, AppearanceInstanceDTO>()
                .Include<CharacteristicInstance, CharacteristicInstanceDTO>()
                .Include<EffectContainerInstance, EffectContainerInstanceDTO>()
                .Include<EquipmentInstance, EquipmentInstanceDTO>()
                .Include<InventoryInstance, InventoryInstanceDTO>()
                .Include<OwnershipInstance, OwnershipInstanceDTO>()
                .Include<TransformInstance, TransformInstanceDTO>()
                .Include<WorldItemPayloadInstance, WorldItemPayloadInstanceDTO>();

            CreateMap<ActionInstance, ActionInstanceDTO>();

            CreateMap<AIInstance, AIInstanceDTO>();

            CreateMap<AppearanceInstance, AppearanceInstanceDTO>();

            CreateMap<CharacteristicInstance, CharacteristicInstanceDTO>()
                .ForMember(dest => dest.Cores, opt => opt.MapFrom(src =>
                    src.GetCores().Select(kvp => new AttributeValueInstanceDTO { AttributeType = kvp.Key, Value = kvp.Value })))
                .ForMember(dest => dest.Vitals, opt => opt.MapFrom(src =>
                    src.GetVitals().Select(kvp => new AttributeValueInstanceDTO { AttributeType = kvp.Key, Value = kvp.Value })));

            CreateMap<EffectContainerInstance, EffectContainerInstanceDTO>();
            CreateMap<EffectInstance, EffectInstanceDTO>();

            CreateMap<EquipmentInstance, EquipmentInstanceDTO>()
                .ForMember(dest => dest.Slots, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.Slots
                        .Where(kvp => kvp.Value != null)
                        .Select(kvp => new EquipmentSlotDTO
                        {
                            AttributeType = kvp.Key,
                            Item = context.Mapper.Map<ItemInstanceDTO>(kvp.Value)
                        })
                        .ToList()));

            CreateMap<InventoryInstance, InventoryInstanceDTO>();
            CreateMap<ItemInstance, ItemInstanceDTO>();

            CreateMap<OwnershipInstance, OwnershipInstanceDTO>();

            CreateMap<TransformInstance, TransformInstanceDTO>();

            CreateMap<WorldItemPayloadInstance, WorldItemPayloadInstanceDTO>();

            // World Domain
            CreateMap<RoomSpatial, RoomRuntimeDTO>();
            CreateMap<RoomConnectionInstance, RoomConnectionRuntimeDTO>();
        }
    }
}