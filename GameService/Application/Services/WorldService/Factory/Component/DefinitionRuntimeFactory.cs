using Application.Interfaces.Cache;
using Contract;
using Domain.Common;
using Domain.Definition.EntityDomain.Component;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;
using Domain.Shared;

namespace Application.Services.WorldService.Factory.Component
{
    public class DefinitionRuntimeFactory
    {
        #region Attributes
        #endregion

        #region Properties
        private readonly ICacheProvider cacheProvider;
        #endregion

        public DefinitionRuntimeFactory(
            ICacheProvider cacheProvider) 
        {
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        public AIInstance CreateAI(
            AIDefinition def)
        {
            return new AIInstance(
                def.ID,
                def.LeashDistance,
                def.AggroRadius,
                def.IsAIControlled,
                def.ThinkInterval);
        }

        public AppearanceInstance CreateAppearance(
            AppearanceDefinition def)
        {
            return new AppearanceInstance(
                def.ID,
                def.SkinID,
                def.SkinColor);
        }

        public CollisionInstance CreateCollision(
            CollisionDefinition def)
        {
            return new CollisionInstance(
                def.ID,
                CollisionShapeMapper.FromDefinition(def),
                new Vector2(def.OffsetX, def.OffsetY),
                def.Layer,
                def.Mask);
        }

        public CharacteristicInstance CreateCharacteristic(
            CharacteristicDefinition def)
        {
            return new CharacteristicInstance(
                def.ID,
                Constraint.DEFAULT_CHARACTERISTIC_LEVEL);
        }

        public InventoryInstance CreateInventory(
            InventoryDefinition def)
        {
            var validDefaultItems = new List<ItemInstance>();

            foreach(var entry in def.DefaultItems)
            {
                var itemDef = cacheProvider.Item.Get(entry.DefinitionID);
                if (itemDef != null)
                    validDefaultItems.Add(
                        new ItemInstance(
                            Guid.NewGuid().ToString(),
                            itemDef.ID,
                            entry.Amount,
                            entry.Quality,
                            itemDef.MaxDurability));
            }

            var inventory = new InventoryInstance(
                def.ID,
                validDefaultItems);

            return inventory;
        }

        public LifetimeInstance CreateLifeTime(
            LifetimeDefinition def)
        {
            return new LifetimeInstance(
                def.ID, 
                def.Duration);
        }

        public ProjectileInstance CreateProjectile(
            ProjectileDefinition def)
        {
            return new ProjectileInstance(
                def.ID, 
                def.Velocity,
                def.OnImpactSpawnEntityDefinitionID);
        }

        public TriggeredEffectInstance CreateTriggeredEffect(
            TriggeredEffectDefinition def)
        {
            var validEffects = new List<string>();

            foreach (var entry in def.EffectDefinitionIDs)
            {
                var effectDef = cacheProvider.Effect.Get(entry);
                if (effectDef != null)
                    validEffects.Add(entry);
            }

            return new TriggeredEffectInstance(
                def.ID,
                validEffects);
        }

        public ActionInstance CreateAction()
        {
            return new ActionInstance();
        }

        public EffectContainerInstance CreateEffectContainer()
        {
            return new EffectContainerInstance();
        }

        public OwnershipInstance CreateOwnership(
            string userId,
            string personalRoomId)
        {
            return new OwnershipInstance(userId, personalRoomId);
        }

        public TransformInstance CreateTransform(
            string roomSpatialId,
            int layerZ,
            Vector2 position)
        {
            return new TransformInstance(roomSpatialId, layerZ, position);
        }

        public WorldItemPayloadInstance CreateWorldItemPayload(
            ItemInstance itemInstance)
        {
            return new WorldItemPayloadInstance(itemInstance);
        }
        #endregion
    }
}