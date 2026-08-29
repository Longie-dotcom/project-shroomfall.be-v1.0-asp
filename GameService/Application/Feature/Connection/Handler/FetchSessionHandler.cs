using Application.Feature.Abstraction;
using Application.Feature.Connection.Command;
using Application.Interface.Repository;
using Application.Interface.Repository.Base;
using Application.Service.WorldService;
using Contract.Common;
using Contract.DTO.Feature.Connection.Response;
using Contract.DTO.Runtime.EntityDomain.Component;
using Domain.Runtime.EntityDomain.Component;
using Domain.Snapshot.EntityDomain.Component;

namespace Application.Feature.Connection.Handler
{
    public class FetchSessionHandler : IHandler<FetchSessionCommand, ExistedSessionDTO>
    {
        #region Attributes
        private readonly IUnitOfWork nonRelational;
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public FetchSessionHandler(
            IUnitOfWork nonRelational,
            WorldContext worldContext)
        {
            this.nonRelational = nonRelational;
            this.worldContext = worldContext;
        }

        #region Methods
        public async Task<ExistedSessionDTO> Handle(FetchSessionCommand command)
        {
            // Get the baseline ownership from the DB (The Directory)
            var repo = nonRelational.GetRepository<IEntitySnapshotRepository>();
            var dbSnapshots = await repo.GetPlayerSnapshotByUserIdAsync(command.UserID);

            // Mapping to result and ensure session existence
            var result = new ExistedSessionDTO { Sessions = new List<ExistedSessionEntryDTO>() };
            foreach (var snapshot in dbSnapshots)
            {
                // Check if this specific entity is currently Hot/Warm in RAM
                var liveEntity = worldContext.GetEntity(snapshot.ID);

                if (liveEntity != null)
                {
                    // STATE: HOT/WARM (Entity is in RAM, DB might be stale)
                    var liveAppearance = liveEntity.GetComponent<AppearanceInstance>();

                    result.Sessions.Add(new ExistedSessionEntryDTO
                    {
                        PlayerInstanceID = liveEntity.ID,
                        PlayerAppearance = MapAppearanceDTO(liveAppearance)
                    });
                }
                else
                {
                    // STATE: COLD (Entity is NOT in RAM, DB is perfectly up to date)
                    var coldAppearance = snapshot.GetComponent<AppearanceSnapshot>();

                    result.Sessions.Add(new ExistedSessionEntryDTO
                    {
                        PlayerInstanceID = snapshot.ID,
                        PlayerAppearance = MapAppearanceDTO(coldAppearance)
                    });
                }
            }

            return result;
        }

        private AppearanceInstanceDTO MapAppearanceDTO(
            AppearanceSnapshot? appearance)
        {
            if (appearance == null) return new AppearanceInstanceDTO();

            return new AppearanceInstanceDTO
            {
                SkinID = appearance.SkinID,
                SkinColor = new HSV
                {
                    H = appearance.SkinColor.H,
                    S = appearance.SkinColor.S,
                    V = appearance.SkinColor.V
                },
            };
        }

        private AppearanceInstanceDTO MapAppearanceDTO(
            AppearanceInstance? appearance)
        {
            if (appearance == null) return new AppearanceInstanceDTO();

            return new AppearanceInstanceDTO
            {
                SkinID = appearance.SkinID,
                SkinColor = new HSV
                {
                    H = appearance.SkinColor.H,
                    S = appearance.SkinColor.S,
                    V = appearance.SkinColor.V
                },
            };
        }
        #endregion
    }
}