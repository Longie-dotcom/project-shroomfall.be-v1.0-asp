using Domain.Abstraction;

namespace Domain.Runtime.EntityDomain
{
    public class EntityInstance
    {
        #region Attributes
        private readonly Dictionary<Type, ComponentInstance> components = new();
        #endregion

        #region Properties
        public string ID { get; }
        public string DefinitionID { get; }
        public IReadOnlyCollection<ComponentInstance> Components => components.Values;
        #endregion

        public EntityInstance(
            string id,
            string definitionId)
        {
            ID = id;
            DefinitionID = definitionId;
        }

        #region Methods
        public bool AddComponent(
            ComponentInstance component)
        {
            var type = component.GetType();

            if (components.ContainsKey(type))
                return false;

            component.Attach(this);

            components.Add(type, component);

            return true;
        }

        public T? GetComponent<T>()
            where T : ComponentInstance
        {
            return components.TryGetValue(
                typeof(T),
                out var component)
                    ? component as T
                    : null;
        }
        #endregion
    }
}