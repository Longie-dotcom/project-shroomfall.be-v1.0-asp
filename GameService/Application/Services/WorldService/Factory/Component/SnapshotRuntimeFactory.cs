using Application.Interfaces.Cache;
using Domain.Abstraction;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;
using Domain.Shared;
using Domain.Snapshot.EntityDomain.Component;
using ResponseCode;

namespace Application.Services.WorldService.Factory.Component
{
    public class SnapshotRuntimeFactory
    {
        #region Attributes
        private readonly ICacheProvider cacheProvider;
        #endregion

        public SnapshotRuntimeFactory(
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
                def.ThinkInterval,
                def.EquippedItemDefinitionID,
                def.AttackRange);
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

            return container;
        }
        
        private InventoryInstance CreateInventory(
            InventorySnapshot snapshot)
        {
            var validItemInstances = new List<ItemInstance>();

            foreach (var i in snapshot.Items)
            {
                var itemDef = cacheProvider.Item.Get(i.DefinitionID);
                if (itemDef != null)
                {
                    validItemInstances.Add(new ItemInstance(
                        i.ID,
                        i.DefinitionID,
                        i.Amount,
                        i.Quality,
                        i.Durability,
                        i.EquippedSlot));
                }
            }

            return new InventoryInstance(
                snapshot.DefinitionID,
                validItemInstances);
        }

        private LifetimeInstance CreateLifetime(
            LifetimeSnapshot snapshot)
        {
            var def = cacheProvider.Lifetime.Get(snapshot.DefinitionID);
            if (def == null)
                throw new InternalException(
                    ApplicationCode.SnapshotComponentFactoryCode.LifetimeDefinitionNotFound,
                    $"Lifetime definition not found: {snapshot.DefinitionID}");

            return new LifetimeInstance(
                snapshot.DefinitionID,
                def.Duration,
                snapshot.ElapsedLifetime);
        }

        private OwnershipInstance CreateOwnership(
            OwnershipSnapshot snapshot)
        {
            return new OwnershipInstance(snapshot.UserID, snapshot.PersonalRoomID);
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
            var def = cacheProvider.TriggeredEffect.Get(snapshot.DefinitionID);
            if (def == null)
                throw new InternalException(
                    ApplicationCode.SnapshotComponentFactoryCode.TriggeredEffectDefinitionNotFound,
                    $"Triggered Effect definition not found: {snapshot.DefinitionID}");

            var validEffects = new List<string>();

            foreach (var effectId in def.EffectDefinitionIDs)
            {
                if (cacheProvider.Effect.Get(effectId) != null)
                {
                    validEffects.Add(effectId);
                }
            }

            return new TriggeredEffectInstance(
                snapshot.DefinitionID,
                validEffects,
                snapshot.SourceEntityID);
        }

        private WorldItemPayloadInstance CreateWorldItemPayload(
            WorldItemPayloadSnapshot snapshot)
        {
            var itemDef = cacheProvider.Item.Get(snapshot.Payload.DefinitionID);
            if (itemDef == null)
                throw new InternalException(
                    ApplicationCode.SnapshotComponentFactoryCode.ItemDefinitionNotFound,
                    $"Item definition not found for world item payload: {snapshot.Payload.DefinitionID}");

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