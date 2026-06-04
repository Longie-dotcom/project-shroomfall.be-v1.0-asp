using Contract;
using Domain.Definition.LocalizationDomain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public static class DataInitializer
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

            public static async Task SeedAsync(RelationalDB db)
            {
                // Check if default locale exists
                var exists = await db.Locales
                    .AnyAsync(x => x.Code == Constraint.DEFAULT_LOCALE);

                if (exists)
                    return;

                // Create default locale
                var locale = new Locale
                (
                    Constraint.DEFAULT_LOCALE,
                    "English (US)",
                    true,
                    true
                );

                await db.Locales.AddAsync(locale);
                await db.SaveChangesAsync();
            }

        #region Methods
        #endregion
    }
}