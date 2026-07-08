using Domain.Abstraction;
using Domain.DomainException;
using ResponseCode;

namespace Application.Services.WorldService
{
    public class PartyService
    {
        #region Attributes
        private readonly Dictionary<string, IRunInstance> activeRuns = new();
        private readonly Dictionary<string, string> playerToRun = new();
        #endregion

        #region Properties
        #endregion

        public PartyService() { }

        #region Commands
        public void RegisterRun(
            IRunInstance run)
        {
            if (activeRuns.ContainsKey(run.ID))
                throw new InternalException(
                    ApplicationCode.PartyServiceCode.RunAlreadyRegistered,
                    $"Run '{run.ID}' has already been registered.");

            activeRuns.Add(run.ID, run);

            foreach (var playerId in run.PlayerEntityInstanceIDs)
            {
                if (playerToRun.ContainsKey(playerId))
                    throw new InternalException(
                        ApplicationCode.PartyServiceCode.PlayerAlreadyInRun,
                        $"Player '{playerId}' is already participating in another run.");

                playerToRun.Add(playerId, run.ID);
            }
        }

        public bool RemoveRun(
            string runId)
        {
            if (!activeRuns.TryGetValue(runId, out var run))
                return false;

            foreach (var playerId in run.PlayerEntityInstanceIDs)
            {
                playerToRun.Remove(playerId);
            }

            return activeRuns.Remove(runId);
        }
        #endregion

        #region Query
        public IRunInstance? GetRun(
            string runId)
        {
            return activeRuns.TryGetValue(runId, out var run) ? run : null;
        }

        public IRunInstance? GetRunByPlayer(
            string playerEntityInstanceId)
        {
            if (!playerToRun.TryGetValue(playerEntityInstanceId, out var runId))
                return null;

            return GetRun(runId);
        }

        public bool IsPlayerInRun(
            string playerEntityInstanceId)
        {
            return playerToRun.ContainsKey(playerEntityInstanceId);
        }

        public IEnumerable<IRunInstance> GetAllRuns()
        {
            return activeRuns.Values;
        }
        #endregion
    }
}