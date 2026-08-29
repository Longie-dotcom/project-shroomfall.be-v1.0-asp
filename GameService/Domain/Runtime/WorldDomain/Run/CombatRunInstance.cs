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

        public bool RemoveParticipant(
            string entityInstanceId)
        {
            if (!participants.Remove(entityInstanceId))
                return false;

            // Maintain leader invariant
            if (LeaderEntityInstanceID == entityInstanceId)
            {
                var newLeader = participants.Values.FirstOrDefault(p => p.Mode == CombatRunParticipantMode.Alive)
                             ?? participants.Values.FirstOrDefault(p => p.Mode == CombatRunParticipantMode.Inactive);

                LeaderEntityInstanceID = newLeader?.EntityInstanceID ?? string.Empty;
            }

            return true;
        }

        public void CheckFail()
        {
            // Fail ONLY if no players are left alive (except for reconnecting)
            bool allDead = participants.Values.All(p => p.Mode == CombatRunParticipantMode.Spectator);
            bool isEmpty = participants.Count == 0;

            if (allDead || isEmpty)
            {
                Status = CombatRunStatus.Failed;
            }
        }

        public void AdvanceFloor(
            string nextRoomSpatialId)
        {
            if (Status != CombatRunStatus.InProgress) return;

            CurrentLevel++;
            CurrentRoomSpatialID = nextRoomSpatialId;
        }

        public void Complete()
        {
            Status = CombatRunStatus.Completed;
        }
        #endregion
    }

    public class CombatRunParticipant : IRunParticipant
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; private set; }
        public DateTime? InactiveSinceUtc { get; private set; }
        public CombatRunParticipantMode Mode { get; private set; }
        #endregion

        public CombatRunParticipant(
            string entityInstanceID)
        {
            EntityInstanceID = entityInstanceID;
            Mode = CombatRunParticipantMode.Alive;
        }

        #region Methods
        public void SetInactive()
        {
            Mode = CombatRunParticipantMode.Inactive;
            InactiveSinceUtc = DateTime.UtcNow;
        }

        public void SetActive()
        {
            Mode = CombatRunParticipantMode.Alive;
            InactiveSinceUtc = null;
        }

        public void SetSpectator()
        {
            Mode = CombatRunParticipantMode.Spectator;
            InactiveSinceUtc = null;
        }
        #endregion
    }
}