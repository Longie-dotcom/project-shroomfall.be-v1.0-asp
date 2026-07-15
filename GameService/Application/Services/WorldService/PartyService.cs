using Domain.Abstraction;
using Domain.DomainException;
using ResponseCode;

namespace Application.Services.WorldService
{
    public class PartyService<TRun, TParticipant>
        where TRun : IRunInstance<TParticipant>
        where TParticipant : IRunParticipant
    {
        #region Attributes
        private readonly Dictionary<string, TRun> activeRuns = new();
        private readonly Dictionary<string, string> playerToRun = new();
        #endregion

        #region Properties
        #endregion

        public PartyService() { }

        #region Commands
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

                playerToRun.Add(
                    participant.EntityInstanceID,
                    run.ID);
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
            }

            return activeRuns.Remove(runId);
        }
        #endregion

        #region Query
        public TRun? GetRun(
            string runId)
        {
            return activeRuns.TryGetValue(runId, out var run)
                ? run
                : default;
        }

        public TRun? GetRunByPlayer(
            string playerEntityInstanceId)
        {
            if (!playerToRun.TryGetValue(playerEntityInstanceId, out var runId))
                return default;

            return GetRun(runId);
        }

        public bool IsPlayerInRun(
            string playerEntityInstanceId)
        {
            return playerToRun.ContainsKey(playerEntityInstanceId);
        }

        public IEnumerable<TRun> GetAllRuns()
        {
            return activeRuns.Values;
        }
        #endregion
    }
}