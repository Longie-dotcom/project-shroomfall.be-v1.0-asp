namespace Domain.Runtime.WorldDomain.World
{
    public class ChunkSpatial
    {
        #region Attributes
        private readonly Dictionary<(int x, int y), HashSet<string>> entities = new();
        #endregion

        #region Properties
        #endregion

        public ChunkSpatial()
        {

        }

        #region Methods
        public void Add(string entityId, int x, int y)
        {
            var key = (x, y);

            if (!entities.TryGetValue(key, out var set))
            {
                set = new HashSet<string>();
                entities[key] = set;
            }

            set.Add(entityId);
        }

        public void Remove(string entityId, int x, int y)
        {
            var key = (x, y);

            if (entities.TryGetValue(key, out var set))
            {
                set.Remove(entityId);

                if (set.Count == 0)
                    entities.Remove(key);
            }
        }

        public IEnumerable<string> Query(int x, int y)
        {
            var key = (x, y);

            if (!entities.TryGetValue(key, out var set))
                return Enumerable.Empty<string>();

            return set;
        }
        #endregion
    }
}