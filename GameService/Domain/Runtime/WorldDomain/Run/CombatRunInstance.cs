using Contract.Enum.WorldDomain;
using Domain.Abstraction;

namespace Domain.Runtime.WorldDomain.Run
{
    public class CombatRunInstance : IRunInstance<CombatRunParticipant>
    {
        #region Attributes
        private readonly Dictionary<string, CombatRunParticipant> participants;
        #endregion

        #region Properties
        public string ID { get; private set; }
        public string CombatRunDefinitionID { get; private set; }
        public int CurrentLevel { get; private set; }
        public string CurrentRoomSpatialID { get; private set; }
        public string LeaderEntityInstanceID { get; private set; }
        public IEnumerable<CombatRunParticipant> Participants => participants.Values;
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
            participants = playerEntityInstanceIds.ToDictionary(
                id => id,
                id => new CombatRunParticipant(id));

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

    public class CombatRunParticipant : IRunParticipant
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; private set; }
        public CombatRunParticipantMode Mode { get; private set; }
        #endregion

        public CombatRunParticipant(
            string entityInstanceID)
        {
            EntityInstanceID = entityInstanceID;
            Mode = CombatRunParticipantMode.Alive;
        }

        #region Methods
        public void SetMode(
            CombatRunParticipantMode mode)
        {
            Mode = mode;
        }
        #endregion
    }
}