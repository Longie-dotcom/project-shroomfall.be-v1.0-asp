using Application.Interfaces.Repository.Relational;
using Domain.Definition.WorldDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class RoomConnectionRepository : SQLGenericRepository<RoomConnection>, IRoomConnectionRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public RoomConnectionRepository(RelationalDB context) : base(context) { }

        #region Methods
        public override async Task<IEnumerable<RoomConnection>> GetAllAsync()
        {
            return await dbSet
                .Include(l => l.SourceRoom)
                .Include(l => l.SourceEntity)
                .Include(l => l.DestinationRoom)
                .Include(l => l.DestinationEntity)
                .ToListAsync();
        }
        #endregion
    }
}