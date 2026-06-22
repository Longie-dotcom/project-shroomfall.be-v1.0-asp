namespace Domain.Abstraction
{
    public abstract class ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid ID { get; protected set; }
        public string EntityDefinitionID { get; protected set; } = string.Empty;
        #endregion

        protected ComponentDefinition() { }

        public ComponentDefinition(
            Guid id, 
            string entityDefinitionID)
        {
            ID = id;
            EntityDefinitionID = entityDefinitionID;
        }

        #region Methods
        #endregion
    }
}