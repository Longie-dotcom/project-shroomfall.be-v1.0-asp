using Application.Interfaces.Repository.Relational;
using Domain.Definition.WorldDomain;
using Infrastructure.Persistence;

namespace Infrastructure.Repository.Relational
{
    public class TileRepository : SQLGenericRepository<Tile>, ITileRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public TileRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        #endregion
    }
}