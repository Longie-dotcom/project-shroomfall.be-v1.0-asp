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
        IInteractableCache Interactable { get; }
        IInventoryCache Inventory { get; }
        ILifetimeCache Lifetime { get; }
        IPortalCache Portal { get; }
        IProjectileCache Projectile { get; }
        ITriggeredEffectCache TriggeredEffect { get; }
        IEntityCache Entity { get; }

        ILocaleCache Locale { get; }

        IEffectCache Effect { get; }
        IItemCache Item { get; }

        ICombatRunCache CombatRun { get; }
        IRoomCache Room { get; }
    }
}
