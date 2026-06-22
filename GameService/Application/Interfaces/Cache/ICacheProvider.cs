using Application.Interfaces.Cache.EntityDomain;
using Application.Interfaces.Cache.EntityDomain.Component;
using Application.Interfaces.Cache.LocalizationDomain;
using Application.Interfaces.Cache.MetaDomain;
using Application.Interfaces.Cache.WorldDomain;

namespace Application.Interfaces.Cache
{
    public interface ICacheProvider
    {
        Task LoadAllAsync();

        IAICache AI { get; }
        IAppearanceCache Appearance { get; }
        ICollisionCache Collision { get; }
        ICharacteristicCache Characteristic { get; }
        IEntityRelationshipCache EntityRelationship { get; }
        IInteractableCache Interactable { get; }
        IInventoryCache Inventory { get; }
        ILifetimeCache Lifetime { get; }
        IProjectileCache Projectile { get; }
        ISpawnCache Spawn { get; }
        ITriggeredEffectCache TriggeredEffect { get; }
        IEntityCache Entity { get; }

        ILocaleCache Locale { get; }

        IEffectCache Effect { get; }
        IItemCache Item { get; }

        IRoomConnectionCache RoomConnection { get; }
        IRoomCache Room { get; }
    }
}
