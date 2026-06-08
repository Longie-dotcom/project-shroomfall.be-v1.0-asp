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

        #region Methods
        public static async Task SeedAsync(RelationalDB db)
        {
            await db.Locales.ExecuteDeleteAsync();

            await db.Locales.AddRangeAsync(
                new Locale(
                    "en-US",
                    "English (US)",
                    true,
                    true
                ),
                new Locale(
                    "en-ME",
                    "Medieval English",
                    true,
                    false
                ),
                new Locale(
                    "vi-VN",
                    "Tiếng Việt",
                    true,
                    false
                ),
                new Locale(
                    "vi-MT",
                    "Tiếng Việt (Miền Tây)",
                    true,
                    false
                )
            );

            await db.SaveChangesAsync();
        }
        #endregion
    }
}