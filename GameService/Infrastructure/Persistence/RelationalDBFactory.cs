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

            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=GameServiceDB;Trusted_Connection=True;TrustServerCertificate=True;"
            );

            return new RelationalDB(optionsBuilder.Options);
        }
        #endregion
    }
}