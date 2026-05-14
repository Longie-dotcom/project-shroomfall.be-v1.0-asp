using Application.Interfaces.Factory;
using Domain.Document.EntityDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class EntityInstanceFactory : IEntityInstanceFactory
    {
        #region Attributes
        private readonly IPlayerInstanceFactory playerInstanceFactory;
        private readonly ICreatureInstanceFactory creatureInstanceFactory;
        private readonly IWorldObjectInstanceFactory worldObjectInstanceFactory;
        #endregion

        #region Properties
        #endregion

        public EntityInstanceFactory(
            IPlayerInstanceFactory playerInstanceFactory,
            ICreatureInstanceFactory creatureInstanceFactory,
            IWorldObjectInstanceFactory worldObjectInstanceFactory)
        {
            this.playerInstanceFactory = playerInstanceFactory;
            this.creatureInstanceFactory = creatureInstanceFactory;
            this.worldObjectInstanceFactory = worldObjectInstanceFactory;
        }

        #region Methods
        public EntityInstance CreateFromDocument(
            EntityDocument doc)
        {
            return doc switch
            {
                PlayerDocument p => playerInstanceFactory.CreateFromDocument(p),
                CreatureDocument c => creatureInstanceFactory.CreateFromDocument(c),
                WorldObjectDocument w => worldObjectInstanceFactory.CreateFromDocument(w),
                _ => throw new InternalException(
                    ResponseCode.EntityInstanceFactory_UnknownDocumentType,
                    $"Unknown document type: {doc.GetType().Name}")
            };
        }  
        #endregion
    }
}