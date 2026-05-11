using Application.Interfaces.Repository.Relational;
using Domain.Definition.AttributeDomain;
using Infrastructure.Persistence;

namespace Infrastructure.Repository.Relational
{
    public class EffectRepository : SQLGenericRepository<Effect>, IEffectRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public EffectRepository(
            RelationalDB context) : base(
                context)
        {
            this.context = context;
        }

        #region Methods
        #endregion
    }
}