namespace Domain.Definition.WorldDomain
{
    public class CombatRunDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; } = string.Empty;
        public List<Floor> Floors { get; private set; } = new();
        #endregion

        protected CombatRunDefinition() : base() { }

        public CombatRunDefinition(
            string id)
        {
            ID = id;
        }

        #region Methods
        #endregion
    }

    public class Floor
    {
        #region Attributes
        #endregion

        #region Properties
        public int Level { get; private set; }
        public string RoomDefinitionID { get; private set; } = string.Empty;

        public string CombatRunDefinitionID { get; private set; } = string.Empty;
        public CombatRunDefinition CombatRunDefinition { get; private set; }
        #endregion

        protected Floor() { }

        public Floor(
            int level,
            string roomDefinitionId,
            string combatRunDefinitionId)
        {
            Level = level;
            RoomDefinitionID = roomDefinitionId;
            CombatRunDefinitionID = combatRunDefinitionId;
        }

        #region Methods
        #endregion
    }
}