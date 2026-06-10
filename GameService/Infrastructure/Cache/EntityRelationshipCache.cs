using Application.Interfaces.Cache;
using Domain.Definition.EntityDomain;

namespace Infrastructure.Cache
{
    public class EntityRelationshipCache : IEntityRelationshipCache
    {
        #region Attributes
        private Dictionary<string, List<EntityRelationship>> map = new();        
        #endregion

        #region Properties
        #endregion

        public EntityRelationshipCache()
        {

        }

        #region Methods
        public void Load(
            IEnumerable<EntityRelationship> data)
        {
            map = data
                .GroupBy(x => x.SourceEntityID)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public IReadOnlyCollection<EntityRelationship> GetAll()
        {
            return map.Values.SelectMany(x => x).ToList();
        }

        public IEnumerable<EntityRelationship> GetBySourceID(
            string id)
        {
            return map.TryGetValue(id, out var items)
                ? items
                : Enumerable.Empty<EntityRelationship>();
        }
        #endregion
    }
}