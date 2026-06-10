using Domain.Definition.LocalizationDomain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeder
{
    public static class LocaleSeeder
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static async Task SeedAsync(
            RelationalDB db)
        {
            await db.Locales.AddRangeAsync(
                new Locale(
                    "en-US",
                    "English (US)",
                    true
                ),
                new Locale(
                    "en-ME",
                    "English (Fancy)"
                ),
                new Locale(
                    "vi-VN",
                    "Vietnamese"
                )
            );

            await db.SaveChangesAsync();
        }
        #endregion
    }
}