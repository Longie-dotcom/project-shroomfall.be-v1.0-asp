using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.NonRelational;
using Contract.DTO.Common;
using Contract.DTO.Connection;
using Contract.DTO.Domain.Runtime;
using Domain.Common;
using Domain.Snapshot.EntityDomain.Component;

namespace Application.Features.Connection.Handlers
{
    public class FetchSessionHandler : IHandler<FetchSessionCommand, ExistedSessionDTO>
    {
        #region Attributes
        private readonly INonRelationalUoW nonRelational;
        #endregion

        #region Properties
        #endregion

        public FetchSessionHandler(
            INonRelationalUoW nonRelational)
        {
            this.nonRelational = nonRelational;
        }

        #region Methods
        public async Task<ExistedSessionDTO> Handle(
            FetchSessionCommand command)
        {
            var userId = command.UserID;

            // Resolve repository
            var repo = nonRelational.GetRepository<IEntitySnapshotRepository>();

            // Retrieve save files
            var snapshot = await repo.GetPlayerDocumentsByUserIdAsync(userId);

            // Mapping and return the DTO
            var result = new ExistedSessionDTO
            {
                Sessions = snapshot
                    .Select(x => new ExistedSessionEntryDTO
                    {
                        PlayerInstanceID = x.ID,
                        PlayerAppearance = MapAppearanceDTO(x.GetComponent<AppearanceSnapshot>())
                    })
                    .ToList()
            };

            return result;
        }

        private AppearanceInstanceDTO MapAppearanceDTO(AppearanceSnapshot? appearance)
        {
            if (appearance == null) return new AppearanceInstanceDTO();

            return new AppearanceInstanceDTO
            {
                SkinID = appearance.SkinID,
                HairID = appearance.HairID ?? string.Empty,
                EyesID = appearance.EyesID ?? string.Empty,
                ShirtID = appearance.ShirtID ?? string.Empty,
                PantID = appearance.PantID ?? string.Empty,
                SkinColor = MapHSVDTO(appearance.HairColor),
                HairColor = MapHSVDTO(appearance.HairColor),
                PantColor = MapHSVDTO(appearance.PantColor)
            };
        }

        private HSVDTO MapHSVDTO(HSV? hsv)
        {
            if (hsv == null) return new HSVDTO();

            return new HSVDTO
            {
                H = hsv.H,
                S = hsv.S,
                V = hsv.V
            };
        }
        #endregion
    }
}