using Application.DTO.Common;
using Application.DTO.Definition;
using Application.DTO.Design;
using Application.DTO.Identity;
using Application.DTO.Runtime;
using AutoMapper;
using Domain.Common;
using Domain.Definition.AttributeDomain;
using Domain.Definition.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.ItemDomain;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.WorldDomain;
using Domain.Document.AttributeDomain;
using Domain.Document.EntityDomain;
using Domain.Document.EntityDomain.Component;
using Domain.Document.ItemDomain;
using Domain.Document.WorldDomain;
using Domain.Other.IdentityDomain;
using Domain.Other.VersionDomain;
using Domain.Runtime.AttributeDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.ItemDomain;
using Domain.Runtime.WorldDomain;

namespace Application.Helper
{
    public class Mapper : Profile
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public Mapper()
        {
            // Common
            CreateMap<HSV, HSVDocument>();
            CreateMap<Vector2, Vector2Document>();
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
            CreateMap<LocalizedText, LocalizedTextDTO>();
            CreateMap<Locale, LocaleDTO>();

            // Attribute Domain
            CreateMap<AttributeDefinition, AttributeDefinitionDTO>()
                .ForMember(d => d.LocalizedText, o => o.MapFrom(s => s.LocalizedText));
            CreateMap<AttributeValue, AttributeValueDefinitionDTO>();
            CreateMap<Characteristic, CharacteristicDefinitionDTO>()
                .ForMember(d => d.LocalizedText, o => o.MapFrom(s => s.LocalizedText));
            CreateMap<Effect, EffectDefinitionDTO>()
                .ForMember(d => d.LocalizedText, o => o.MapFrom(s => s.LocalizedText));

            // Entity Domain
            CreateMap<Appearance, AppearanceDefinitionDTO>()
                .ForMember(d => d.SkinColor, o => o.MapFrom(s => s.SkinColor));
            CreateMap<Collision, CollisionDefinitionDTO>();
            CreateMap<PlayerAppearance, PlayerAppearanceDefinitionDTO>()
                .IncludeBase<Appearance, AppearanceDefinitionDTO>()
                .ForMember(d => d.HairColor, o => o.MapFrom(s => s.HairColor))
                .ForMember(d => d.EyeColor, o => o.MapFrom(s => s.EyeColor))
                .ForMember(d => d.PantColor, o => o.MapFrom(s => s.PantColor));

            CreateMap<Entity, EntityDefinitionDTO>()
                .Include<Creature, CreatureDefinitionDTO>()
                .Include<WorldObject, WorldObjectDefinitionDTO>()
                .Include<Player, PlayerDefinitionDTO>()
                .ForMember(d => d.LocalizedText, o => o.MapFrom(s => s.LocalizedText))
                .ForMember(d => d.Appearance, o => o.MapFrom(s => s.Appearance))
                .ForMember(d => d.Collision, o => o.MapFrom(s => s.Collision));

            CreateMap<AreaEffect, AreaEffectDefinitionDTO>()
                .IncludeBase<Entity, EntityDefinitionDTO>();
            CreateMap<Creature, CreatureDefinitionDTO>()
                .IncludeBase<Entity, EntityDefinitionDTO>();
            CreateMap<Projectile, ProjectileDefinitionDTO>()
                .IncludeBase<Entity, EntityDefinitionDTO>();
            CreateMap<WorldObject, WorldObjectDefinitionDTO>()
                .IncludeBase<Entity, EntityDefinitionDTO>();

            CreateMap<Player, PlayerDefinitionDTO>()
                .IncludeBase<Creature, CreatureDefinitionDTO>()
                .ForMember(d => d.PlayerAppearance, o => o.MapFrom(s => s.PlayerAppearance));

            // Item Domain
            CreateMap<Inventory, InventoryDefinitionDTO>()
                .ForMember(d => d.LocalizedText, o => o.MapFrom(s => s.LocalizedText));
            CreateMap<InventoryItem, InventoryItemDefinitionDTO>();
            CreateMap<Item, ItemDefinitionDTO>()
                .ForMember(d => d.LocalizedText, o => o.MapFrom(s => s.LocalizedText));
            CreateMap<ItemEffect, ItemEffectDefinitionDTO>();

            // World Domain
            CreateMap<Cell, CellDefinitionDTO>();
            CreateMap<EntitySpawnRule, EntitySpawnRuleDefinitionDTO>();
            CreateMap<Room, RoomDefinitionDTO>()
                .ForMember(d => d.LocalizedText, o => o.MapFrom(s => s.LocalizedText));
            CreateMap<SpawnArea, SpawnAreaDefinitionDTO>();
            CreateMap<Tile, TileDefinitionDTO>()
                .ForMember(d => d.LocalizedText, o => o.MapFrom(s => s.LocalizedText));

            // ─────────────────────────────
            // RUNTIME to DTO
            // ─────────────────────────────
            // Attribute Domain
            CreateMap<CharacteristicInstance, CharacteristicRuntimeDTO>()
                .ForMember(dest => dest.Cores, opt => opt.MapFrom(src =>
                    src.GetCores().Select(x => new AttributeValueRuntimeDTO
                    {
                        AttributeType = x.Key,
                        Value = x.Value
                    })
                ))
                .ForMember(dest => dest.Vitals, opt => opt.MapFrom(src =>
                    src.GetVitals().Select(x => new AttributeValueRuntimeDTO
                    {
                        AttributeType = x.Key,
                        Value = x.Value
                    })
                ));
            CreateMap<EffectInstance, EffectRuntimeDTO>();

            // Item Domain
            CreateMap<ItemInstance, ItemRuntimeDTO>();
            CreateMap<InventoryInstance, InventoryRuntimeDTO>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

            // Entity Domain
            CreateMap<AppearanceInstance, AppearanceRuntimeDTO>()
                .ForMember(d => d.SkinColor, o => o.MapFrom(s => s.SkinColor));
            CreateMap<PlayerAppearanceInstance, PlayerAppearanceRuntimeDTO>()
                .IncludeBase<AppearanceInstance, AppearanceRuntimeDTO>()
                .ForMember(d => d.HairColor, o => o.MapFrom(s => s.HairColor))
                .ForMember(d => d.EyeColor, o => o.MapFrom(s => s.EyeColor))
                .ForMember(d => d.PantColor, o => o.MapFrom(s => s.PantColor));

            CreateMap<EntityInstance, EntityRuntimeDTO>()
                .Include<CreatureInstance, CreatureRuntimeDTO>()
                .Include<WorldObjectInstance, WorldObjectRuntimeDTO>()
                .Include<PlayerInstance, PlayerRuntimeDTO>()
                .ForMember(d => d.Position, o => o.MapFrom(s => s.Position))
                .ForMember(d => d.Direction, o => o.MapFrom(s => s.Direction))
                .ForMember(d => d.Appearance, o => o.MapFrom(s => s.Appearance));

            CreateMap<CreatureInstance, CreatureRuntimeDTO>()
                .IncludeBase<EntityInstance, EntityRuntimeDTO>()
                .ForMember(d => d.Characteristic, o => o.MapFrom(s => s.Characteristic))
                .ForMember(d => d.Inventory, o => o.MapFrom(s => s.Inventory))
                .ForMember(d => d.ActiveEffects, o => o.MapFrom(s => s.ActiveEffects))
                .ForMember(d => d.Equipment, o => o.MapFrom((s, d, _, context) =>
                    s.GetEquipment()
                        .Where(x => x.Value != null)
                        .Select(x => new EquipmentRuntimeDTO
                        {
                            EquipmentSlot = x.Key,
                            Item = context.Mapper.Map<ItemRuntimeDTO>(x.Value)
                        })
                        .ToList()
                ));
            CreateMap<WorldObjectInstance, WorldObjectRuntimeDTO>()
                .IncludeBase<EntityInstance, EntityRuntimeDTO>()
                .ForMember(d => d.Inventory, o => o.MapFrom(s => s.Inventory));

            CreateMap<PlayerInstance, PlayerRuntimeDTO>()
                .IncludeBase<CreatureInstance, CreatureRuntimeDTO>()
                .ForMember(d => d.PlayerAppearance, o => o.MapFrom(s => s.PlayerAppearance));

            // World Domain
            CreateMap<RoomSpatial, RoomRuntimeDTO>();

            // ─────────────────────────────
            // RUNTIME to DOCUMENT
            // ─────────────────────────────
            // Attribute Domain
            CreateMap<CharacteristicInstance, CharacteristicDocument>()
                .ForMember(d => d.Vitals, o => o.MapFrom(s => s.GetVitals()));
            CreateMap<EffectInstance, EffectDocument>();

            // Item Domain
            CreateMap<ItemInstance, ItemDocument>();
            CreateMap<InventoryInstance, InventoryDocument>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

            // Entity Domain
            CreateMap<AppearanceInstance, AppearanceDocument>()
                .ForMember(d => d.SkinColor, o => o.MapFrom(s => s.SkinColor));
            CreateMap<PlayerAppearanceInstance, PlayerAppearanceDocument>()
                .IncludeBase<AppearanceInstance, AppearanceDocument>()
                .ForMember(d => d.HairColor, o => o.MapFrom(s => s.HairColor))
                .ForMember(d => d.EyeColor, o => o.MapFrom(s => s.EyeColor))
                .ForMember(d => d.PantColor, o => o.MapFrom(s => s.PantColor));

            CreateMap<EntityInstance, EntityDocument>()
                .Include<CreatureInstance, CreatureDocument>()
                .Include<WorldObjectInstance, WorldObjectDocument>()
                .Include<PlayerInstance, PlayerDocument>()
                .ForMember(d => d.Position, o => o.MapFrom(s => s.Position))
                .ForMember(d => d.Direction, o => o.MapFrom(s => s.Direction))
                .ForMember(d => d.Appearance, o => o.MapFrom(s => s.Appearance));

            CreateMap<CreatureInstance, CreatureDocument>()
                .IncludeBase<EntityInstance, EntityDocument>()
                .ForMember(d => d.Characteristic, o => o.MapFrom(s => s.Characteristic))
                .ForMember(d => d.Inventory, o => o.MapFrom(s => s.Inventory))
                .ForMember(d => d.ActiveEffects, o => o.MapFrom(s => s.ActiveEffects))
                .ForMember(d => d.Equipment, o => o.MapFrom(s => s.GetEquipment()));
            CreateMap<WorldObjectInstance, WorldObjectDocument>()
                .IncludeBase<EntityInstance, EntityDocument>()
                .ForMember(d => d.Inventory, o => o.MapFrom(s => s.Inventory));

            CreateMap<PlayerInstance, PlayerDocument>()
                .IncludeBase<CreatureInstance, CreatureDocument>()
                .ForMember(d => d.PlayerAppearance, o => o.MapFrom(s => s.PlayerAppearance));

            // World Domain
            CreateMap<RoomSpatial, RoomDocument>();
        }
    }
}