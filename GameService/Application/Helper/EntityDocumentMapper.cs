using Application.Interfaces.Factory;
using AutoMapper;
using Domain.Document.EntityDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Helper
{
    public static class EntityDocumentMapper
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static EntityInstance ToRuntime(EntityDocument doc, IEntityDocumentFactory factory)
        {
            return doc switch
            {
                PlayerDocument p => factory.CreateFromDocument(p),
                CreatureDocument c => factory.CreateFromDocument(c),
                WorldObjectDocument w => factory.CreateFromDocument(w),
                _ => throw new Exception($"Unknown document type: {doc.GetType().Name}")
            };
        }

        public static EntityDocument ToDocument(EntityInstance entity, IMapper mapper)
        {
            return entity switch
            {
                PlayerInstance p => mapper.Map<PlayerDocument>(p),
                CreatureInstance c => mapper.Map<CreatureDocument>(c),
                WorldObjectInstance w => mapper.Map<WorldObjectDocument>(w),
                _ => throw new Exception($"Unknown entity type: {entity.GetType().Name}")
            };
        }
        #endregion
    }
}