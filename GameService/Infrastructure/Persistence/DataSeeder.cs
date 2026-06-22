using Contract;
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
        #endregion
    }
}