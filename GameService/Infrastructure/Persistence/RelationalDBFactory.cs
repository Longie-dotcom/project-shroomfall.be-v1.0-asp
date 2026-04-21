using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence
{
    public class RelationalDBFactory : IDesignTimeDbContextFactory<RelationalDB>
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public RelationalDBFactory()
        {
        }

        #region Methods
        public RelationalDB CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RelationalDB>();

            // Fallback connection string for migrations (LOCAL DEV ONLY)
            var connectionString =
                Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
                ?? "Server=localhost;Database=GameServiceDB;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new RelationalDB(optionsBuilder.Options);
        }
        #endregion
    }
}