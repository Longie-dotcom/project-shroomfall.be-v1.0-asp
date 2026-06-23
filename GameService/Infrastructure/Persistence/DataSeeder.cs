using Contract;
using Contract.Enum.IdentityDomain;
using Domain.Definition.IdentityDomain;
using Domain.Definition.LocalizationDomain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public static class DataSeeder
    {
        #region Methods
        public static async Task SeedAsync(
            RelationalDB context)
        {
            await SeedLocale(context);
            await SeedGlobalDefinitionVersion(context);
            await SeedAdministrativeAccounts(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedLocale(
            RelationalDB context)
        {
            var existed = await context.Locales
                .FirstOrDefaultAsync(x =>
                    x.Code == Constraint.DEFAULT_LOCALE);

            if (existed != null)
                return;

            var locale = new Locale(
                code: Constraint.DEFAULT_LOCALE,
                name: "English (United States)",
                isDefault: true,
                isEnabled: true);

            await context.Locales.AddAsync(locale);
        }

        private static async Task SeedGlobalDefinitionVersion(
            RelationalDB context)
        {
            var existedLocale = await context.Locales
                .FirstOrDefaultAsync(x =>
                    x.Code == Constraint.DEFAULT_LOCALE);

            if (existedLocale == null)
                return;


            var existed = await context.LocalizationEntries
                .AnyAsync(x =>
                    x.Key == Constraint.GLOBAL_DEFINITION_VERSION &&
                    x.LocaleCode == Constraint.DEFAULT_LOCALE);

            if (existed)
                return;


            var entry = new LocalizationEntry(
                id: Guid.NewGuid(),
                key: Constraint.GLOBAL_DEFINITION_VERSION,
                localeCode: Constraint.DEFAULT_LOCALE,
                value: "1",
                description: "Global definition version");

            await context.Set<LocalizationEntry>()
                .AddAsync(entry);
        }

        private static async Task SeedAdministrativeAccounts(
            RelationalDB context)
        {
            const string EasyPassword = "password123";
            var sharedPasswordHash = Password.Create(EasyPassword);

            var administrativeSeeds = new List<User>
            {
                // Admin Account
                new User(
                    id: "usr_admin_01",
                    name: "Admin Workspace",
                    preferredLocale: Constraint.DEFAULT_LOCALE,
                    role: Role.Admin,
                    password: sharedPasswordHash,
                    email: "admin@shroomfall.com"
                ),

                // Designer Account
                new User(
                    id: "usr_designer_01",
                    name: "Designer Workspace",
                    preferredLocale: Constraint.DEFAULT_LOCALE,
                    role: Role.Designer,
                    password: sharedPasswordHash,
                    email: "designer@shroomfall.com"
                )
            };

            foreach (var userSeed in administrativeSeeds)
            {
                var exists = await context.Set<User>().AnyAsync(x => x.ID == userSeed.ID || x.Email == userSeed.Email);
                if (!exists)
                {
                    await context.Set<User>().AddAsync(userSeed);
                }
            }
        }
        #endregion
    }
}