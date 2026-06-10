using Contract.Enum.ItemDomain;
using Domain.Definition.ItemDomain;
using Domain.Definition.LocalizationDomain;

namespace Infrastructure.Persistence.Seeder
{
    public static class InventorySeeder
    {
        public static async Task SeedAsync(RelationalDB db)
        {
            // Define the 3 playable hero inventory keys from EntitySeeder
            var classInventoryIds = new List<string>
            {
                EntitySeeder.ArcherInventoryId,  // "inv_player_archer"
                EntitySeeder.JokerInventoryId,   // "inv_player_joker"
                EntitySeeder.WarriorInventoryId  // "inv_player_warrior"
            };

            var inventoriesToSeed = new List<Inventory>();

            // 1. Generate the 3 structural configurations for player characters
            foreach (var invId in classInventoryIds)
            {
                // Converts "inv_player_archer" -> "player.archer" to format localization keys cleanly
                string localizationBaseKey = invId.Replace("inv_", "").Replace("_", ".");

                var playerLocale = new LocalizedText
                {
                    NameKey = $"{localizationBaseKey}.name",
                    DescriptionKey = $"{localizationBaseKey}.desc"
                };

                inventoriesToSeed.Add(new Inventory(
                    id: invId,
                    type: InventoryType.PlayerInventory,
                    localizedText: playerLocale,
                    slotCount: 24
                ));
            }

            // 2. Satisfy the structural dependency expected by WORLD_CHEST ("inv_chest_01")
            var chestLocale = new LocalizedText
            {
                NameKey = "world.chest.name",
                DescriptionKey = "world.chest.desc"
            };

            inventoriesToSeed.Add(new Inventory(
                id: "inv_chest_01",
                type: InventoryType.Container,
                localizedText: chestLocale,
                slotCount: 12
            ));

            // Track into tracking graph and commit transaction securely
            await db.Set<Inventory>().AddRangeAsync(inventoriesToSeed);
            await db.SaveChangesAsync();
        }
    }
}