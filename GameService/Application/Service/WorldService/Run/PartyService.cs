using Domain.Abstraction;
using Domain.DomainException;
using ResponseCode;

namespace Application.Service.WorldService.Run
{
    public class PartyService<TRun, TParticipant>
        where TRun : IRunInstance<TParticipant>
        where TParticipant : IRunParticipant
    {
        #region Attributes
        private readonly Dictionary<string, TRun> activeRuns = new();
        private readonly Dictionary<string, string> playerToRun = new();
        private readonly Dictionary<string, TParticipant> inactiveParticipants = new();
        #endregion

        #region Properties
        #endregion

        public PartyService() { }

        #region Registration & Cleanup
        public void RegisterRun(
            TRun run)
        {
            if (activeRuns.ContainsKey(run.ID))
                throw new InternalException(
                    ApplicationCode.PartyServiceCode.RunAlreadyRegistered,
                    $"Run '{run.ID}' has already been registered.");

            activeRuns.Add(run.ID, run);

            foreach (var participant in run.Participants)
            {
                if (playerToRun.ContainsKey(participant.EntityInstanceID))
                    throw new InternalException(
                        ApplicationCode.PartyServiceCode.PlayerAlreadyInRun,
                        $"Player '{participant.EntityInstanceID}' is already participating in another run.");

                playerToRun.Add(participant.EntityInstanceID, run.ID);
            }
        }

        public bool RemoveRun(
            string runId)
        {
            if (!activeRuns.TryGetValue(runId, out var run))
                return false;

            foreach (var participant in run.Participants)
            {
                playerToRun.Remove(participant.EntityInstanceID);
                inactiveParticipants.Remove(participant.EntityInstanceID);
            }

            return activeRuns.Remove(runId);
        }

        public void AddPlayerToRun(
            string runId,
            string playerId)
        {
            if (!activeRuns.TryGetValue(runId, out var run))
                return;

            if (playerToRun.ContainsKey(playerId))
                throw new InternalException(
                    ApplicationCode.PartyServiceCode.PlayerAlreadyRegistered,
                    $"Player '{playerId}' is already registered a run.");

            run.AddParticipant(playerId);
            playerToRun[playerId] = runId;
        }
        #endregion

        #region Lifecycle Commands (Connect, Disconnect, Quit)

        /// <summary>
        /// ACCIDENTAL DROP (SignalR disconnect): Enters 10-second grace period (Inactive).
        /// </summary>
        public bool HandleDisconnect(
            string playerInstanceId)
        {
            var run = GetRunByPlayer(playerInstanceId);
            if (run == null) 
                return false;

            var participant = run.Participants.FirstOrDefault(p => p.EntityInstanceID == playerInstanceId);
            if (participant == null) 
                return false;

            participant.SetInactive();
            inactiveParticipants[playerInstanceId] = participant;
            return true;
        }

        /// <summary>
        /// RECONNECTION: Restores player from Inactive back to Active state.
        /// </summary>
        public bool HandleReconnect(
            string playerInstanceId)
        {
            if (!inactiveParticipants.TryGetValue(playerInstanceId, out var participant))
                return false;

            participant.SetActive();
            inactiveParticipants.Remove(playerInstanceId);
            return true;
        }

        /// <summary>
        /// INTENTIONAL QUIT (HTTP /unload): Immediately removes player from run and evaluates CheckFail.
        /// </summary>
        public bool HandleQuit(
            string playerInstanceId)
        {
            var run = GetRunByPlayer(playerInstanceId);
            if (run == null) 
                return false;

            run.RemoveParticipant(playerInstanceId);
            playerToRun.Remove(playerInstanceId);
            inactiveParticipants.Remove(playerInstanceId);

            run.CheckFail();
            return true;
        }

        /// <summary>
        /// TTL EXPIRATION TICK: Called in game loop to clean up expired inactive participants.
        /// </summary>
        public IEnumerable<(TRun Run, TParticipant Participant)> GetExpiredInactiveParticipants(
            TimeSpan timeout)
        {
            var now = DateTime.UtcNow;

            foreach (var participant in inactiveParticipants.Values)
            {
                if (!participant.InactiveSinceUtc.HasValue)
                    continue;

                if (now - participant.InactiveSinceUtc.Value < timeout)
                    continue;

                var run = GetRunByPlayer(participant.EntityInstanceID);
                if (run != null)
                    yield return (run, participant);
            }
        }
        #endregion

        #region Queries
        public TRun? GetRun(
            string runId)
        {
            return activeRuns.TryGetValue(runId, out var run) ? run : default;
        }

        public TRun? GetRunByPlayer(
            string playerInstanceId)
        {
            if (!playerToRun.TryGetValue(playerInstanceId, out var runId))
                return default;

            return GetRun(runId);
        }
        #endregion
    }
}