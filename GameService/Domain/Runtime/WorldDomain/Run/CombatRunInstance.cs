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
            IEnumerable<string> playerEntityInstanceIds,
            string initialRoomSpatialId)
        {
            ID = id;
            CombatRunDefinitionID = combatRunDefinitionId;
            LeaderEntityInstanceID = leaderEntityInstanceId;
            CurrentRoomSpatialID = initialRoomSpatialId;

            participants = playerEntityInstanceIds.ToDictionary(
                pId => pId,
                pId => new CombatRunParticipant(pId));

            CurrentLevel = 1;
            Status = CombatRunStatus.InProgress;
        }

        #region Methods
        public void AddParticipant(
            string entityInstanceId)
        {
            if (!participants.ContainsKey(entityInstanceId))
            {
                participants.Add(entityInstanceId, new CombatRunParticipant(entityInstanceId));
            }
        }

        public void AdvanceFloor(string nextRoomSpatialId)
        {
            if (Status != CombatRunStatus.InProgress) return;

            CurrentLevel++;
            CurrentRoomSpatialID = nextRoomSpatialId;
        }

        public void Complete()
        {
            Status = CombatRunStatus.Completed;
        }

        public void CheckFail()
        {
            if (participants.Values.All(p => p.Mode == CombatRunParticipantMode.Spectator))
            {
                Status = CombatRunStatus.Failed;
            }
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