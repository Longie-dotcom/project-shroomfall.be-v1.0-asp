using AutoMapper;
using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Spatial;
using Domain.Snapshot.EntityDomain;
using Domain.Snapshot.EntityDomain.Component;
using Domain.Snapshot.WorldDomain;

namespace Application.Mapper
{
    public class SnapshotMapper : Profile
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public SnapshotMapper()
        {
            // Common
            CreateMap<HSV, HSV>();
            CreateMap<Vector2, Vector2>();

            // ─────────────────────────────
            // RUNTIME to SNAPSHOT
            // ─────────────────────────────
            // Entity Domain
            CreateMap<EntityInstance, EntitySnapshot>()
                .ForMember(dest => dest.Components, opt => opt.MapFrom(src => src.Components));

            CreateMap<ComponentInstance, ComponentSnapshot>()
                .Include<ActionInstance, ActionSnapshot>()
                .Include<AIInstance, AISnapshot>()
                .Include<AppearanceInstance, AppearanceSnapshot>()
                .Include<CollisionInstance, CollisionSnapshot>()
                .Include<CharacteristicInstance, CharacteristicSnapshot>()
                .Include<EffectContainerInstance, EffectContainerSnapshot>()
                .Include<EquipmentInstance, EquipmentSnapshot>()
                .Include<InventoryInstance, InventorySnapshot>()
                .Include<LifetimeInstance, LifetimeSnapshot>()
                .Include<OwnershipInstance, OwnershipSnapshot>()
                .Include<ProjectileInstance, ProjectileSnapshot>()
                .Include<TransformInstance, TransformSnapshot>()
                .Include<TriggeredEffectInstance, TriggeredEffectSnapshot>()
                .Include<WorldItemPayloadInstance, WorldItemPayloadSnapshot>();

            CreateMap<ActionInstance, ActionSnapshot>();

            CreateMap<AIInstance, AISnapshot>();

            CreateMap<AppearanceInstance, AppearanceSnapshot>();

            CreateMap<CollisionInstance, CollisionSnapshot>();

            CreateMap<CharacteristicInstance, CharacteristicSnapshot>()
                .ForMember(dest => dest.Vitals, opt => opt.MapFrom(src => src.GetVitals()))
                .ForMember(dest => dest.Cores, opt => opt.MapFrom(src => src.GetCores()));

            CreateMap<EffectContainerInstance, EffectContainerSnapshot>();
            CreateMap<EffectInstance, EffectSnapshot>();

            CreateMap<EquipmentInstance, EquipmentSnapshot>();

            CreateMap<InventoryInstance, InventorySnapshot>();
            CreateMap<ItemInstance, ItemSnapshot>();

            CreateMap<LifetimeInstance, LifetimeSnapshot>();

            CreateMap<OwnershipInstance, OwnershipSnapshot>();

            CreateMap<ProjectileInstance, ProjectileSnapshot>();

            CreateMap<TransformInstance, TransformSnapshot>();

            CreateMap<TriggeredEffectInstance, TriggeredEffectSnapshot>();

            CreateMap<WorldItemPayloadInstance, WorldItemPayloadSnapshot>();

            // World Domain
            CreateMap<RoomSpatial, RoomSnapshot>();
        }
    }
}