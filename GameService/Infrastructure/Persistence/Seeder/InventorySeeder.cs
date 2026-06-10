using Contract.Enum.ItemDomain;
using Domain.Definition.ItemDomain;
using Domain.Definition.LocalizationDomain;

namespace Infrastructure.Persistence.Seeder
{
    public static class InventorySeeder
    {
        public static async Task SeedAsync(RelationalDB db)
        {
            var inventoriesToSeed = new List<Inventory>();

            #region 👤 Player Inventories
            // Define the 3 playable hero inventory keys from EntitySeeder
            var classInventoryIds = new List<string>
            {
                EntitySeeder.ArcherInventoryId,  // "inv_player_archer"
                EntitySeeder.JokerInventoryId,   // "inv_player_joker"
                EntitySeeder.WarriorInventoryId  // "inv_player_warrior"
            };

            foreach (var invId in classInventoryIds)
            {
                // Converts "inv_player_archer" -> "player.archer"
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
            #endregion

            #region 🍄 Creature Inventories (Loot Bags)
            // Define the 5 elemental Shroom inventory keys from EntitySeeder
            var shroomInventoryIds = new List<string>
            {
                EntitySeeder.FireShroomInventoryId,   // "inv_creature_shroom_fire"
                EntitySeeder.IceShroomInventoryId,    // "inv_creature_shroom_ice"
                EntitySeeder.EarthShroomInventoryId,  // "inv_creature_shroom_earth"
                EntitySeeder.DarkShroomInventoryId,   // "inv_creature_shroom_dark"
                EntitySeeder.LightShroomInventoryId   // "inv_creature_shroom_light"
            };

            foreach (var invId in shroomInventoryIds)
            {
                // Converts "inv_creature_shroom_fire" -> "creature.shroom.fire"
                string localizationBaseKey = invId.Replace("inv_", "").Replace("_", ".");

                var shroomLocale = new LocalizedText
                {
                    NameKey = $"{localizationBaseKey}.name",
                    DescriptionKey = $"{localizationBaseKey}.desc"
                };

                // Using CreatureBag with 6 slots, ideal for basic monster drops and materials
                inventoriesToSeed.Add(new Inventory(
                    id: invId,
                    type: InventoryType.CreatureBag,
                    localizedText: shroomLocale,
                    slotCount: 6
                ));
            }
            #endregion

            // Track into tracking graph and commit transaction securely
            await db.Set<Inventory>().AddRangeAsync(inventoriesToSeed);
            await db.SaveChangesAsync();
        }
    }
}