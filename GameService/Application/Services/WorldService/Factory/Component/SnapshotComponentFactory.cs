using Application.Interfaces.Cache;
using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;
using Domain.Snapshot.EntityDomain.Component;

namespace Application.Services.WorldService.Factory.Component
{
    public class SnapshotComponentFactory
    {
        #region Attributes
        private readonly ICacheProvider cacheProvider;
        #endregion

        public SnapshotComponentFactory(
            ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        public ComponentInstance Create(
            ComponentSnapshot snapshot)
        {
            return snapshot switch
            {
                ActionSnapshot action =>
                    CreateAction(action),
                AISnapshot ai => 
                    CreateAI(ai),
                AppearanceSnapshot appearance => 
                    CreateAppearance(appearance),
                CollisionSnapshot collision => 
                    CreateCollision(collision),
                CharacteristicSnapshot characteristic =>
                    CreateCharacteristic(characteristic),
                EffectContainerSnapshot effectContainer => 
                    CreateEffectContainer(effectContainer),
                EquipmentSnapshot equipment => 
                    CreateEquipment(equipment),
                InventorySnapshot inventory => 
                    CreateInventory(inventory),
                LifetimeSnapshot lifetime => 
                    CreateLifetime(lifetime),
                OwnershipSnapshot ownership => 
                    CreateOwnership(ownership),
                ProjectileSnapshot projectile => 
                    CreateProjectile(projectile),
                TransformSnapshot transform => 
                    CreateTransform(transform),
                TriggeredEffectSnapshot effectSnapshot =>
                    CreateTriggeredEffect(effectSnapshot),
                WorldItemPayloadSnapshot payload => 
                    CreateWorldItemPayload(payload),
                _ => throw new InternalException(
                    ApplicationCode.SnapshotComponentFactoryCode.SnapshotTypeNotSupported,
                    $"Snapshot type {snapshot.GetType().Name} is not supported by factory.")
            };
        }

        private ActionInstance CreateAction(
            ActionSnapshot snapshot)
        {
            return new ActionInstance();
        }

        private AIInstance CreateAI(
            AISnapshot snapshot)
        {
            var def = cacheProvider.AI.Get(snapshot.DefinitionID);
            if (def == null)
                throw new InternalException(
                    ApplicationCode.SnapshotComponentFactoryCode.AIDefinitionNotFound,
                    $"AI definition not found: {snapshot.DefinitionID}");

            return new AIInstance(
                def.ID,
                def.LeashDistance,
                def.AggroRadius,
                def.IsAIControlled,
                def.ThinkInterval);
        }

        private AppearanceInstance CreateAppearance(
            AppearanceSnapshot snapshot)
        {
            return new AppearanceInstance(
                snapshot.DefinitionID,
                snapshot.SkinID,
                snapshot.SkinColor
            );
        }

        private CollisionInstance CreateCollision(
            CollisionSnapshot snapshot)
        {
            var def = cacheProvider.Collision.Get(snapshot.DefinitionID);
            if (def == null)
                throw new InternalException(
                    ApplicationCode.SnapshotComponentFactoryCode.CollisionDefinitionNotFound,
                    $"Collision definition not found: {snapshot.DefinitionID}");
            
            return new CollisionInstance(
                snapshot.DefinitionID,
                CollisionShapeMapper.FromDefinition(def),
                new Vector2(def.OffsetX, def.OffsetY),
                def.Layer,
                def.Mask
            );
        }

        private CharacteristicInstance CreateCharacteristic(
            CharacteristicSnapshot snapshot)
        {
            var instance = new CharacteristicInstance(snapshot.DefinitionID, snapshot.CurrentLevel);

            // Restore current vitals
            foreach (var vital in snapshot.Vitals)
            {
                instance.SetVital(vital.Key, vital.Value);
            }

            return instance;
        }

        private EffectContainerInstance CreateEffectContainer(
            EffectContainerSnapshot snapshot)
        {
            var container = new EffectContainerInstance();

            foreach (var effectSnap in snapshot.ActiveEffects)
            {
                var effectDef = cacheProvider.Effect.Get(effectSnap.DefinitionID);

                if (effectDef == null) continue;

                var effectInstance = new EffectInstance(
                    effectSnap.DefinitionID,
                    effectSnap.RemainingTime,
                    effectDef.Interval,
                    effectSnap.IntervalAccumulator);

                container.ActiveEffects.Add(effectInstance);
            }

            return container;
        }

        private EquipmentInstance CreateEquipment(
            EquipmentSnapshot snapshot)
        {
            var equipment = new EquipmentInstance();

            foreach (var slot in snapshot.Slots)
            {
                if (slot.Value == null)
                {
                    equipment.LoadSlot(slot.Key, null);
                    continue;
                }

                var itemInstance = new ItemInstance(
                    slot.Value.ID,
                    slot.Value.DefinitionID,
                    slot.Value.Amount,
                    slot.Value.Quality,
                    slot.Value.Durability
                );

                equipment.LoadSlot(slot.Key, itemInstance);
            }

            return equipment;
        }

        private InventoryInstance CreateInventory(
            InventorySnapshot snapshot)
        {
            var itemInstances = snapshot.Items
                .Select(i => new ItemInstance(
                    i.ID,
                    i.DefinitionID,
                    i.Amount, 
                    i.Quality,
                    i.Durability))
                .ToList();

            return new InventoryInstance(
                snapshot.DefinitionID,
                itemInstances);
        }

        private LifetimeInstance CreateLifetime(
            LifetimeSnapshot snapshot)
        {
            return new LifetimeInstance(
                snapshot.DefinitionID,
                snapshot.Duration,
                snapshot.ElapsedLifetime);
        }

        private OwnershipInstance CreateOwnership(
            OwnershipSnapshot snapshot)
        {
            return new OwnershipInstance(snapshot.UserID);
        }

        private ProjectileInstance CreateProjectile(
            ProjectileSnapshot snapshot)
        {
            var def = cacheProvider.Projectile.Get(snapshot.DefinitionID);
            if (def == null)
                throw new InternalException(
                    ApplicationCode.SnapshotComponentFactoryCode.ProjectileDefinitionNotFound,
                    $"Projectile definition not found: {snapshot.DefinitionID}");

            return new ProjectileInstance(
                def.ID,
                def.Velocity,
                def.OnImpactSpawnEntityDefinitionID);
        }

        private TransformInstance CreateTransform(
            TransformSnapshot snapshot)
        {
            return new TransformInstance(
                snapshot.RoomSpatialID,
                snapshot.LayerZ,
                snapshot.Position);
        }

        private TriggeredEffectInstance CreateTriggeredEffect(
            TriggeredEffectSnapshot snapshot)
        {
            return new TriggeredEffectInstance(
                snapshot.DefinitionID,
                snapshot.EffectDefinitionIDs);
        }

        private WorldItemPayloadInstance CreateWorldItemPayload(
            WorldItemPayloadSnapshot snapshot)
        {
            var itemPayload = new ItemInstance(
                snapshot.Payload.ID,
                snapshot.Payload.DefinitionID,
                snapshot.Payload.Amount,
                snapshot.Payload.Quality,
                snapshot.Payload.Durability
            );

            return new WorldItemPayloadInstance(itemPayload);
        }
        #endregion
    }
}