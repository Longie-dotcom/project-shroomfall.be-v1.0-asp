using Contract.Enum.WorldDomain;
using Domain.Abstraction;

namespace Domain.Runtime.WorldDomain.Run
{
    public class CombatRunInstance : IRunInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public string CombatRunDefinitionID { get; private set; }
        public int CurrentLevel { get; private set; }
        public string CurrentRoomSpatialID { get; private set; }
        public string LeaderEntityInstanceID { get; private set; }
        public IReadOnlyCollection<string> PlayerEntityInstanceIDs { get; private set; }
        public CombatRunStatus Status { get; private set; }
        #endregion

        public CombatRunInstance(
            string id,
            string combatRunDefinitionId,
            string leaderEntityInstanceId,
            IEnumerable<string> playerEntityInstanceIds)
        {
            ID = id;
            CombatRunDefinitionID = combatRunDefinitionId;
            LeaderEntityInstanceID = leaderEntityInstanceId;
            PlayerEntityInstanceIDs = playerEntityInstanceIds.ToList();

            CurrentLevel = 1;
            CurrentRoomSpatialID = string.Empty;
            Status = CombatRunStatus.Waiting;
        }

        #region Methods
        public void Start(
            string roomSpatialId)
        {
            CurrentRoomSpatialID = roomSpatialId;
            Status = CombatRunStatus.InProgress;
        }

        public void AdvanceFloor(
            string nextRoomSpatialId)
        {
            CurrentLevel++;
            CurrentRoomSpatialID = nextRoomSpatialId;
        }

        public void Complete()
        {
            Status = CombatRunStatus.Completed;
        }

        public void Fail()
        {
            Status = CombatRunStatus.Failed;
        }
        #endregion
    }
}