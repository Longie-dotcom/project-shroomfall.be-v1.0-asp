using Application.Interfaces.Repository.Relational;
using Domain.Definition.WorldDomain;
using Infrastructure.Persistence;

namespace Infrastructure.Repository.Relational
{
    public class RoomConnectionRepository : SQLGenericRepository<RoomConnection>, IRoomConnectionRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public RoomConnectionRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        #endregion
    }
}