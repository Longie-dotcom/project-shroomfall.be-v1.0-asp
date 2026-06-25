using Contract;
using Domain.Abstraction;
using Domain.Common;
using Domain.Definition.EntityDomain.Component;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Services.WorldService.Factory.Component
{
    public class DefinitionComponentFactory
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public DefinitionComponentFactory() { }

        #region Methods
        public ComponentInstance Create(
            ComponentDefinition definition)
        {
            return definition switch
            {
                AIDefinition ai =>
                    CreateAI(ai),
                AppearanceDefinition appearance =>
                    CreateAppearance(appearance),
                CollisionDefinition collision =>
                    CreateCollision(collision),
                CharacteristicDefinition characteristic =>
                    CreateCharacteristic(characteristic),
                InventoryDefinition inventory =>
                    CreateInventory(inventory),
                LifetimeDefinition lifetine =>
                    CreateLifeTime(lifetine),
                ProjectileDefinition projectile =>
                    CreateProjectile(projectile),
                TriggeredEffectDefinition triggeredEffect =>
                    CreateTriggeredEffect(triggeredEffect),
                _ => throw new InternalException(
                    ApplicationCode.DefinitionComponentFactoryCode.ComponentDefinitionNotSupported,
                    $"Component definition type '{definition.GetType().Name}' is not supported by the factory.")
            };
        }

        private AIInstance CreateAI(
            AIDefinition def)
        {
            return new AIInstance(
                def.ID,
                def.LeashDistance,
                def.AggroRadius,
                def.IsAIControlled,
                def.ThinkInterval);
        }

        private AppearanceInstance CreateAppearance(
            AppearanceDefinition def)
        {
            return new AppearanceInstance(
                def.ID,
                def.SkinID,
                def.SkinColor);
        }

        private CollisionInstance CreateCollision(
            CollisionDefinition def)
        {
            return new CollisionInstance(
                def.ID,
                CollisionShapeMapper.FromDefinition(def),
                new Vector2(def.OffsetX, def.OffsetY),
                def.Layer,
                def.Mask);
        }

        private CharacteristicInstance CreateCharacteristic(
            CharacteristicDefinition def)
        {
            return new CharacteristicInstance(
                def.ID,
                Constraint.DEFAULT_CHARACTERISTIC_LEVEL);
        }

        private InventoryInstance CreateInventory(
            InventoryDefinition def)
        {
            return new InventoryInstance(
                def.ID,
                def.DefaultItems
                    .Select(d => new ItemInstance(
                        Guid.NewGuid().ToString(), 
                        d.DefinitionID,
                        d.Amount, 
                        d.Quality,
                        d.Durability))
                    .ToList());
        }

        private LifetimeInstance CreateLifeTime(
            LifetimeDefinition def)
        {
            return new LifetimeInstance(
                def.ID, 
                def.Lifetime);
        }

        private ProjectileInstance CreateProjectile(
            ProjectileDefinition def)
        {
            return new ProjectileInstance(
                def.ID, 
                def.Velocity,
                def.OnImpactSpawnEntityDefinitionID);
        }

        private TriggeredEffectInstance CreateTriggeredEffect(
            TriggeredEffectDefinition def)
        {
            return new TriggeredEffectInstance(
                def.ID,
                def.EffectDefinitionIDs);
        }
        #endregion
    }
}