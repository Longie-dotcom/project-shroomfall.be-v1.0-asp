using Application.Interfaces.Factory;
using Domain.Document.EntityDomain;
using Domain.Runtime.EntityDomain;

namespace Infrastructure.Factory
{
    public class EntityDocumentFactory : IEntityDocumentFactory
    {
        #region Attributes
        private readonly IPlayerInstanceFactory player;
        private readonly ICreatureInstanceFactory creature;
        private readonly IWorldObjectInstanceFactory worldObject;
        #endregion

        #region Properties
        #endregion

        public EntityDocumentFactory(
            IPlayerInstanceFactory player,
            ICreatureInstanceFactory creature,
            IWorldObjectInstanceFactory worldObject)
        {
            this.player = player;
            this.creature = creature;
            this.worldObject = worldObject;
        }

        #region Methods
        public EntityInstance CreateFromDocument(
            EntityDocument doc)
        {
            return doc switch
            {
                PlayerDocument p => player.CreateFromDocument(p),
                CreatureDocument c => creature.CreateFromDocument(c),
                WorldObjectDocument w => worldObject.CreateFromDocument(w),
                _ => throw new Exception($"Unknown document type: {doc.GetType().Name}")
            };
        }  
        #endregion
    }
}