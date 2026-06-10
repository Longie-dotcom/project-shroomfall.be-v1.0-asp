using Contract.Enum.ItemDomain;
using Domain.Definition.ItemDomain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeder
{
    public static class InventoryItemSeeder
    {
        public static async Task SeedAsync(RelationalDB db)
        {
            // 1. Clear existing items to prevent unique constraint violations on re-seed
            await db.Set<InventoryItem>().ExecuteDeleteAsync();

            // 2. Define the starter kits and creature loot drops
            var inventoryItems = new List<InventoryItem>
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

                // ==========================================
                // 🍄 FIRE SHROOM LOOT
                // ==========================================
                new InventoryItem(EntitySeeder.FireShroomInventoryId, ItemSeeder.HP_POTION_ID, 2, ItemQuality.Low),
                new InventoryItem(EntitySeeder.FireShroomInventoryId, ItemSeeder.STAMINA_POTION_ID, 1, ItemQuality.Low),

                // ==========================================
                // ❄️ ICE SHROOM LOOT
                // ==========================================
                new InventoryItem(EntitySeeder.IceShroomInventoryId, ItemSeeder.HP_POTION_ID, 1, ItemQuality.Low),
                new InventoryItem(EntitySeeder.IceShroomInventoryId, ItemSeeder.STAMINA_POTION_ID, 2, ItemQuality.Low),

                // ==========================================
                // ⛰️ EARTH SHROOM LOOT
                // ==========================================
                new InventoryItem(EntitySeeder.EarthShroomInventoryId, ItemSeeder.HP_POTION_ID, 3, ItemQuality.Low),

                // ==========================================
                // 🌌 DARK SHROOM LOOT
                // ==========================================
                new InventoryItem(EntitySeeder.DarkShroomInventoryId, ItemSeeder.STAMINA_POTION_ID, 3, ItemQuality.Low),

                // ==========================================
                // ☀️ LIGHT SHROOM LOOT
                // ==========================================
                new InventoryItem(EntitySeeder.LightShroomInventoryId, ItemSeeder.HP_POTION_ID, 2, ItemQuality.Low),
                new InventoryItem(EntitySeeder.LightShroomInventoryId, ItemSeeder.STAMINA_POTION_ID, 2, ItemQuality.Low),
            };

            // 3. Batch insert the items
            await db.Set<InventoryItem>().AddRangeAsync(inventoryItems);
            await db.SaveChangesAsync();
        }
    }
}