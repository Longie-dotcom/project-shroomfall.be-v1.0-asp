using Application.Context;
using Application.Interfaces.Factory;
using Application.Interfaces.Repository.NonRelational;

namespace Application.Bootstrapper
{
    public class TopologyBootstrap
    {
        #region Attributes
        private readonly INonRelationalUoW nonRelational;
        private readonly WorldContext worldContext;
        private readonly IRoomConnectionInstanceFactory factory;
        #endregion

        #region Properties
        #endregion

        public TopologyBootstrap(
            INonRelationalUoW nonRelational,
            WorldContext worldContext,
            IRoomConnectionInstanceFactory factory)
        {
            this.nonRelational = nonRelational;
            this.worldContext = worldContext;
            this.factory = factory;
        }

        #region Methods
        public async Task LoadAsync()
        {
            var repo = nonRelational.GetRepository<IRoomConnectionDocumentRepository>();
            var docs = await repo.GetAllAsync();

            foreach (var doc in docs)
            {
                var connection = factory.CreateFromDocument(doc);
                worldContext.AddConnection(connection);
            }
        }
        #endregion
    }
}