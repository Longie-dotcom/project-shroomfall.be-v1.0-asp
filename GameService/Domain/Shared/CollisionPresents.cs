using Contract.Enum.EntityDomain;

namespace Domain.Shared
{
    public static class CollisionPresets
    {
        // Walls block everything that physically moves, but ignore projectiles/triggers/items
        public const CollisionLayer WallMask =
            CollisionLayer.Player | CollisionLayer.Enemy | CollisionLayer.PlayerProjectile | CollisionLayer.EnemyProjectile;

        // Players bump into walls, enemies, enemy bullets, step on traps, and pick up items.
        // (They ignore ally players and their own bullets!)
        public const CollisionLayer PlayerMask =
            CollisionLayer.Wall | CollisionLayer.EnemyProjectile | CollisionLayer.Collectible;

        // Enemies bump into walls, players, player bullets, and step on traps.
        // (They ignore other enemies, enemy bullets, and dropped items on the floor!)
        public const CollisionLayer EnemyMask =
            CollisionLayer.Wall | CollisionLayer.PlayerProjectile;

        // Player Projectiles only care about slamming into walls or popping enemies.
        public const CollisionLayer PlayerProjectileMask =
            CollisionLayer.Wall | CollisionLayer.Enemy;

        // Enemy Projectiles only care about slamming into walls or hurting players.
        public const CollisionLayer EnemyProjectileMask =
            CollisionLayer.Wall | CollisionLayer.Player;

        // Collectibles (like items on the floor) only care when a player walks over them.
        public const CollisionLayer CollectibleMask =
            CollisionLayer.Player;
    }
}