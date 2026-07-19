using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.NonRelational;
using Application.Services.WorldService;
using Contract.DTO.Common;
using Contract.DTO.Feature.Connection.Response;
using Contract.DTO.Runtime.EntityDomain.Component;
using Domain.Runtime.EntityDomain.Component;
using Domain.Snapshot.EntityDomain.Component;

namespace Application.Features.Connection.Handlers
{
    public class FetchSessionHandler : IHandler<FetchSessionCommand, ExistedSessionDTO>
    {
        #region Attributes
        private readonly INonRelationalUoW nonRelational;
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public FetchSessionHandler(
            INonRelationalUoW nonRelational,
            WorldContext worldContext)
        {
            this.nonRelational = nonRelational;
            this.worldContext = worldContext;
        }

        #region Methods
        public async Task<ExistedSessionDTO> Handle(FetchSessionCommand command)
        {
            var userId = command.UserID;

            Console.WriteLine($"[FetchSession] Fetching sessions for user '{userId}'");

            var repo = nonRelational.GetRepository<IEntitySnapshotRepository>();

            var dbSnapshots = (await repo.GetPlayerSnapshotByUserIdAsync(userId)).ToList();

            Console.WriteLine($"[FetchSession] Repository returned {dbSnapshots.Count} player snapshot(s).");

            var result = new ExistedSessionDTO
            {
                Sessions = new List<ExistedSessionEntryDTO>()
            };

            foreach (var snapshot in dbSnapshots)
            {
                Console.WriteLine($"[FetchSession] Processing snapshot '{snapshot.ID}'");

                var ownership = snapshot.GetComponent<OwnershipSnapshot>();
                Console.WriteLine($"    Ownership UserID = {ownership?.UserID ?? "<null>"}");

                var liveEntity = worldContext.GetEntity(snapshot.ID);

                if (liveEntity != null)
                {
                    Console.WriteLine($"    Entity is LIVE in RAM.");

                    var liveAppearance = liveEntity.GetComponent<AppearanceInstance>();

                    if (liveAppearance == null)
                        Console.WriteLine($"    WARNING: AppearanceInstance missing.");

                    result.Sessions.Add(new ExistedSessionEntryDTO
                    {
                        PlayerInstanceID = liveEntity.ID,
                        PlayerAppearance = MapAppearanceDTO(liveAppearance)
                    });
                }
                else
                {
                    Console.WriteLine($"    Entity is COLD (using Mongo snapshot).");

                    var coldAppearance = snapshot.GetComponent<AppearanceSnapshot>();

                    if (coldAppearance == null)
                        Console.WriteLine($"    WARNING: AppearanceSnapshot missing.");

                    result.Sessions.Add(new ExistedSessionEntryDTO
                    {
                        PlayerInstanceID = snapshot.ID,
                        PlayerAppearance = MapAppearanceDTO(coldAppearance)
                    });
                }
            }

            Console.WriteLine($"[FetchSession] Returning {result.Sessions.Count} session(s).");

            return result;
        }

        private AppearanceInstanceDTO MapAppearanceDTO(AppearanceSnapshot? appearance)
        {
            if (appearance == null) return new AppearanceInstanceDTO();

            return new AppearanceInstanceDTO
            {
                SkinID = appearance.SkinID,
                SkinColor = new HSVDTO
                {
                    H = appearance.SkinColor.H,
                    S = appearance.SkinColor.S,
                    V = appearance.SkinColor.V
                },
            };
        }

        private AppearanceInstanceDTO MapAppearanceDTO(AppearanceInstance? appearance)
        {
            if (appearance == null) return new AppearanceInstanceDTO();

            return new AppearanceInstanceDTO
            {
                SkinID = appearance.SkinID,
                SkinColor = new HSVDTO
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