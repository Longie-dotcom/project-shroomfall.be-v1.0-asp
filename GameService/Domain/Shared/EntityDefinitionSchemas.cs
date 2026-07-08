using Contract.Enum.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;

namespace Domain.Shared
{
    public static class EntityDefinitionSchemas
    {
        private static readonly Dictionary<EntityType, List<Type>> SchemaRules = new()
        {
            {
                EntityType.AreaEffect, new()
                {
                    typeof(CollisionDefinition),
                    typeof(LifetimeDefinition),
                    typeof(TriggeredEffectDefinition)
                }
            },

            {
                EntityType.Portal, new()
                {
                    typeof(CollisionDefinition),
                    typeof(PortalDefinition)
                }
            },

            {
                EntityType.Projectile, new()
                {
                    typeof(CollisionDefinition),
                    typeof(LifetimeDefinition),
                    typeof(TriggeredEffectDefinition),
                    typeof(ProjectileDefinition)
                }
            },

            {
                EntityType.WorldObject, new()
                {
                    typeof(CollisionDefinition),
                    typeof(InteractableDefinition)
                }
            },

            {
                EntityType.Creature, new()
                {
                    typeof(CollisionDefinition),
                    typeof(CharacteristicDefinition),
                    typeof(InventoryDefinition),
                    typeof(AppearanceDefinition),
                    typeof(AIDefinition)
                }
            },

            {
                EntityType.Player, new()
                {
                    typeof(CollisionDefinition),
                    typeof(CharacteristicDefinition),
                    typeof(InventoryDefinition),
                    typeof(AppearanceDefinition)
                }
            },

            {
                EntityType.Item, new()
                {
                    typeof(CollisionDefinition)
                }
            },
        };

        public static IEnumerable<Type> GetRequiredComponentTypes(EntityType type)
        {
            if (SchemaRules.TryGetValue(type, out var componentTypes))
            {
                return componentTypes;
            }

            throw new InternalException(
                DomainCode.EntityDefinitionSchemaCode.EntityTypeNotConfigured,
                $"No component layout mapping configured for entity type '{type}'."
            );
        }
    }
}