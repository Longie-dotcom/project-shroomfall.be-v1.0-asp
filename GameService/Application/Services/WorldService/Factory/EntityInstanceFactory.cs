using Application.Interfaces.Cache;
using Application.Services.AttributeService;
using Application.Services.WorldService.Factory.Component;
using Contract.Enum.EntityDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Snapshot.EntityDomain;
using ResponseCode;

namespace Application.Services.WorldService.Factory
{
    public class EntityInstanceFactory
    {
        #region Attributes
        private readonly ICacheProvider cacheProvider;
        private readonly DefinitionComponentFactory definitionComponentFactory;
        private readonly RuntimeComponentFactory runtimeComponentFactory;
        private readonly SnapshotComponentFactory snapshotComponentFactory;
        private readonly CharacteristicService characteristicService;
        #endregion

        #region Properties
        #endregion

        public EntityInstanceFactory(
            ICacheProvider cacheProvider,
            DefinitionComponentFactory definitionComponentFactory,
            RuntimeComponentFactory runtimeComponentFactory,
            SnapshotComponentFactory snapshotComponentFactory,
            CharacteristicService characteristicService)
        {
            this.cacheProvider = cacheProvider;
            this.definitionComponentFactory = definitionComponentFactory;
            this.runtimeComponentFactory = runtimeComponentFactory;
            this.snapshotComponentFactory = snapshotComponentFactory;
            this.characteristicService = characteristicService;
        }

        #region Methods
        public EntityInstance Rehydrate(
            EntitySnapshot snapshot)
        {
            var entity = new EntityInstance(snapshot.ID, snapshot.DefinitionID);

            foreach (var componentSnapshot in snapshot.Components)
            {
                var runtimeComponent = snapshotComponentFactory.Create(componentSnapshot);
                entity.AddComponent(runtimeComponent);
            }

            // Refresh entity
            characteristicService.InitializeCores(entity);

            return entity;
        }

        public EntityInstance Create(
            WorldEntityCreateContext context)
        {
            var entityDef = cacheProvider.Entity.Get(context.DefinitionID);
            if (entityDef == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.EntityDefinitionNotFound,
                    $"Failed to create entity. Definition ID '{context.DefinitionID}' not found in cache.");

            var entity = new EntityInstance(context.InstanceID, entityDef.ID);

            ConstructEntity(entity, context);

            switch (entityDef.Type)
            {
                case EntityType.AreaEffect:
                    ConstructAreaEffect(entity, context);
                    break;
                case EntityType.Portal:
                    ConstructPortal(entity, context);
                    break;
                case EntityType.Projectile:
                    ConstructProjectile(entity, context);
                    break;
                case EntityType.WorldObject:
                    ConstructWorldObject(entity, context);
                    break;
                case EntityType.Creature:
                    ConstructCreature(entity, context);
                    break;
                case EntityType.Player:
                    ConstructPlayer(entity, context);
                    break;
                case EntityType.Item:
                    ConstructItem(entity, context);
                    break;
                default:
                    throw new InternalException(
                        ApplicationCode.EntityInstanceFactoryCode.EntityTypeNotSupported,
                        $"Entity creation failed. Entity type '{entityDef.Type}' is not supported by this factory.");
            }

            return entity;
        }

        private void ConstructEntity(
            EntityInstance entity,
            WorldEntityCreateContext context)
        {
            var collision = cacheProvider.Collision.GetByEntity(entity.DefinitionID);
            if (collision == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.CollisionDefinitionNotFound,
                    $"WorldEntity '{entity.DefinitionID}' missing Collision definition.");

            entity.AddComponent(definitionComponentFactory.Create(collision));
            entity.AddComponent(runtimeComponentFactory.CreateTransform(context.RoomSpatialID, context.LayerZ, context.Position));
        }

        private void ConstructPortal(
            EntityInstance entity,
            WorldEntityCreateContext context)
        {

        }

        private void ConstructAreaEffect(
            EntityInstance entity,
            WorldEntityCreateContext context)
        {
            var lifetime = cacheProvider.Lifetime.GetByEntity(entity.DefinitionID);
            if (lifetime == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.LifetimeDefinitionNotFound,
                    $"AreaEffect '{entity.DefinitionID}' missing Lifetime definition in cache.");

            var triggeredEffect = cacheProvider.TriggeredEffect.GetByEntity(entity.DefinitionID);
            if (triggeredEffect == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.TriggeredEffectDefinitionNotFound,
                    $"AreaEffect '{entity.DefinitionID}' missing Triggered Effect configuration.");

            entity.AddComponent(definitionComponentFactory.Create(lifetime));
            entity.AddComponent(definitionComponentFactory.Create(triggeredEffect));
        }

        private void ConstructProjectile(EntityInstance entity, WorldEntityCreateContext context)
        {
            if (context is not ProjectileEntityCreateContext projectileContext)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.InvalidContextType,
                    $"Expected ProjectileEntityCreateContext for Projectile entity, but got {context.GetType().Name}.");

            var lifetime = cacheProvider.Lifetime.GetByEntity(entity.DefinitionID);
            if (lifetime == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.LifetimeDefinitionNotFound,
                    $"Projectile '{entity.DefinitionID}' missing Lifetime definition.");

            var triggeredEffect = cacheProvider.TriggeredEffect.GetByEntity(entity.DefinitionID);
            if (triggeredEffect == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.TriggeredEffectDefinitionNotFound,
                    $"Projectile '{entity.DefinitionID}' missing Triggered Effect configuration.");

            var projectile = cacheProvider.Projectile.GetByEntity(entity.DefinitionID);
            if (projectile == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.ProjectileDefinitionNotFound,
                    $"Projectile '{entity.DefinitionID}' missing Projectile configuration.");

            entity.AddComponent(definitionComponentFactory.Create(lifetime));
            entity.AddComponent(definitionComponentFactory.Create(triggeredEffect));
            entity.AddComponent(definitionComponentFactory.Create(projectile));

            // Set direction
            entity.GetComponent<ProjectileInstance>()!.Direction = projectileContext.Direction;
        }

        private void ConstructWorldObject(
            EntityInstance entity,
            WorldEntityCreateContext context)
        {

        }

        private void ConstructCreature(
            EntityInstance entity,
            WorldEntityCreateContext context)
        {
            var characteristic = cacheProvider.Characteristic.GetByEntity(entity.DefinitionID);
            if (characteristic == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.CharacteristicDefinitionNotFound,
                    $"Creature '{entity.DefinitionID}' missing Characteristic definition.");

            var inventory = cacheProvider.Inventory.GetByEntity(entity.DefinitionID);
            if (inventory == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.InventoryDefinitionNotFound,
                    $"Creature '{entity.DefinitionID}' missing Inventory definition.");

            var appearance = cacheProvider.Appearance.GetByEntity(entity.DefinitionID);
            if (appearance == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.AppearanceDefinitionNotFound,
                    $"Creature '{entity.DefinitionID}' missing Appearance definition.");

            var ai = cacheProvider.AI.GetByEntity(entity.DefinitionID);
            if (ai == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.AIDefinitionNotFound,
                    $"Creature '{entity.DefinitionID}' missing AI definition.");

            entity.AddComponent(definitionComponentFactory.Create(characteristic));
            entity.AddComponent(definitionComponentFactory.Create(inventory));
            entity.AddComponent(definitionComponentFactory.Create(appearance));
            entity.AddComponent(definitionComponentFactory.Create(ai));
            entity.AddComponent(runtimeComponentFactory.CreateEffectContainer());
            entity.AddComponent(runtimeComponentFactory.CreateEquipment());
            entity.AddComponent(runtimeComponentFactory.CreateAction());

            characteristicService.InitializeVitals(entity);
            characteristicService.InitializeCores(entity);
        }

        private void ConstructPlayer(
            EntityInstance entity,
            WorldEntityCreateContext context)
        {
            if (context is not PlayerEntityCreateContext playerContext)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.InvalidContextType,
                    $"Expected PlayerEntityCreateContext for Player entity, but got {context.GetType().Name}.");

            var characteristic = cacheProvider.Characteristic.GetByEntity(entity.DefinitionID);
            if (characteristic == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.CharacteristicDefinitionNotFound,
                    $"Player '{entity.DefinitionID}' missing Characteristic definition.");

            var inventory = cacheProvider.Inventory.GetByEntity(entity.DefinitionID);
            if (inventory == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.InventoryDefinitionNotFound,
                    $"Player '{entity.DefinitionID}' missing Inventory definition.");

            var appearance = cacheProvider.Appearance.GetByEntity(entity.DefinitionID);
            if (appearance == null)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.AppearanceDefinitionNotFound,
                    $"Player '{entity.DefinitionID}' missing Appearance definition.");

            entity.AddComponent(definitionComponentFactory.Create(characteristic));
            entity.AddComponent(definitionComponentFactory.Create(inventory));
            entity.AddComponent(definitionComponentFactory.Create(appearance));
            entity.AddComponent(runtimeComponentFactory.CreateEffectContainer());
            entity.AddComponent(runtimeComponentFactory.CreateEquipment());
            entity.AddComponent(runtimeComponentFactory.CreateAction());
            entity.AddComponent(runtimeComponentFactory.CreateOwnership(playerContext.UserID, playerContext.PersonalRoomID));

            characteristicService.InitializeVitals(entity);
            characteristicService.InitializeCores(entity);
        }

        private void ConstructItem(
            EntityInstance entity,
            WorldEntityCreateContext context)
        {
            if (context is not InventoryEntityCreateContext itemContext)
                throw new InternalException(
                    ApplicationCode.EntityInstanceFactoryCode.InvalidContextType,
                    $"Expected InventoryEntityCreateContext for Item entity, but got {context.GetType().Name}.");

            entity.AddComponent(runtimeComponentFactory.CreateWorldItemPayload(itemContext.Payload));
        }
        #endregion
    }
}