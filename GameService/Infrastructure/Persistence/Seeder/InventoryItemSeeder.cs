using Contract.Enum.ItemDomain;
using Domain.Definition.ItemDomain;

namespace Infrastructure.Persistence.Seeder
{
    public static class InventoryItemSeeder
    {
        public static async Task SeedAsync(RelationalDB db)
        {
            // 2. Define the new starter kits
            var starterItems = new List<InventoryItem>
            {
                // ==========================================
                // 🏹 ARCHER STARTER KIT
                // ==========================================
                new InventoryItem(EntitySeeder.ArcherInventoryId, ItemSeeder.WOODEN_BOW_ID, 1, ItemQuality.Medium),
                new InventoryItem(EntitySeeder.ArcherInventoryId, ItemSeeder.HP_POTION_ID, 5, ItemQuality.Low),
                new InventoryItem(EntitySeeder.ArcherInventoryId, ItemSeeder.STAMINA_POTION_ID, 3, ItemQuality.Low),

                // ==========================================
                // 🔨 JOKER STARTER KIT
                // ==========================================
                new InventoryItem(EntitySeeder.JokerInventoryId, ItemSeeder.STUN_GRENADE_ID, 3, ItemQuality.Low),
                new InventoryItem(EntitySeeder.JokerInventoryId, ItemSeeder.FIRE_BOMB_ID, 2, ItemQuality.Low),
                new InventoryItem(EntitySeeder.JokerInventoryId, ItemSeeder.STAMINA_POTION_ID, 5, ItemQuality.Low),
                new InventoryItem(EntitySeeder.JokerInventoryId, ItemSeeder.WOODEN_CLUB_ID, 1, ItemQuality.Medium),

                // ==========================================
                // ⚔️ WARRIOR STARTER KIT
                // ==========================================
                new InventoryItem(EntitySeeder.WarriorInventoryId, ItemSeeder.IRON_SWORD_ID, 1, ItemQuality.Medium),
                new InventoryItem(EntitySeeder.WarriorInventoryId, ItemSeeder.IRON_CHEST_ID, 1, ItemQuality.Medium),
                new InventoryItem(EntitySeeder.WarriorInventoryId, ItemSeeder.HP_POTION_ID, 3, ItemQuality.Low),
            };

            // 3. Batch insert the items
            await db.Set<InventoryItem>().AddRangeAsync(starterItems);
            await db.SaveChangesAsync();
        }
    }
}