using Contract;
using Domain.Definition.AttributeDomain;
using Domain.Definition.EntityDomain;
using Domain.Definition.ItemDomain;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.WorldDomain;
using Domain.Other.IdentityDomain;
using Domain.Other.VersionDomain;
using Infrastructure.Persistence.Seeder;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public static class DataInitializer
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static async Task SeedAsync(RelationalDB db)
        {
            await ClearDatabase(db);

            await LocaleSeeder.SeedAsync(db);
            await CharacteristicSeeder.SeedAsync(db);
            await EffectSeeder.SeedEffectDefinitionsAsync(db);

            await ItemSeeder.SeedAsync(db);
            await InventorySeeder.SeedAsync(db);
            await InventoryItemSeeder.SeedAsync(db);

            await EntitySeeder.SeedPlayerDefinitionsAsync(db);
            await EntitySeeder.SeedCreatureDefinitionsAsync(db);
            await EntitySeeder.SeedEntityDefinitionsAsync(db);

            await SeedVersionAsync(db);
        }

        public static async Task SeedVersionAsync(RelationalDB db)
        {
            // Create new version entry
            var newVersion = new DefinitionVersionLog(
                id: Guid.NewGuid().ToString(),
                key: Constraint.GLOBAL_DEFINITION_VERSION,
                version: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                description: "Updated item and entity definitions"
            );

            await db.DefinitionVersionLogs.AddAsync(newVersion);
            await db.SaveChangesAsync();
        }

        public static async Task ClearDatabase(RelationalDB db)
        {
            // 1. CLEAR DEPENDENTS (Junction Tables & Child Tables)
            // These tables reference other tables via Foreign Keys.
            await db.Set<InventoryItem>().ExecuteDeleteAsync();
            await db.Set<ItemEffect>().ExecuteDeleteAsync();
            await db.Set<EntityRelationship>().ExecuteDeleteAsync();
            await db.Set<LocalizationEntry>().ExecuteDeleteAsync();
            await db.Set<SpawnArea>().ExecuteDeleteAsync();
            await db.Set<EntitySpawnRule>().ExecuteDeleteAsync();
            await db.Set<RoomConnection>().ExecuteDeleteAsync();
            await db.Set<Cell>().ExecuteDeleteAsync();
            await db.Set<AttributeValue>().ExecuteDeleteAsync();

            // 2. CLEAR INDEPENDENT ENTITIES
            // These are the "parent" tables.
            await db.Set<Inventory>().ExecuteDeleteAsync();
            await db.Set<Item>().ExecuteDeleteAsync();
            await db.Set<Effect>().ExecuteDeleteAsync();
            await db.Set<Entity>().ExecuteDeleteAsync(); // Base class for many types
            await db.Set<Room>().ExecuteDeleteAsync();
            await db.Set<Characteristic>().ExecuteDeleteAsync();
            await db.Set<Locale>().ExecuteDeleteAsync();
            await db.Set<User>().ExecuteDeleteAsync();
            await db.Set<DefinitionVersionLog>().ExecuteDeleteAsync();

            // 3. Persist deletions
            await db.SaveChangesAsync();
        }
        #endregion
    }
}