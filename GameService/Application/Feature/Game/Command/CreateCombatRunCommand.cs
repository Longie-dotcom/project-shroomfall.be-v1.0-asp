namespace Application.Feature.Game.Command
{
    public class CreateCombatRunCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string CombatRunDefinitionID { get; }
        #endregion

        public CreateCombatRunCommand(
            string userId, 
            string combatRunDefinitionID)
        {
            UserID = userId;
            CombatRunDefinitionID = combatRunDefinitionID;
        }

        #region Methods
        #endregion
    }
}
