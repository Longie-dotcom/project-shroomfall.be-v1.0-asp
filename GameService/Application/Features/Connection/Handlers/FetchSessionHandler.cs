using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Repository.NonRelational;
using Contract.DTO.Common;
using Contract.DTO.Connection;
using Contract.DTO.Runtime;
using Domain.Common;
using Domain.Definition.EntityDomain.Component;
using Domain.Document.EntityDomain.Component;

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
            var repo = nonRelational.GetRepository<IEntityDocumentRepository>();

            // Retrieve save files
            var docs = await repo.GetPlayerDocumentsByUserIdAsync(userId);

            // Mapping and return the DTO
            var result = new ExistedSessionDTO
            {
                Sessions = docs
                    .Select(x => new ExistedSessionEntryDTO
                    {
                        PlayerInstanceID = x.ID,
                        PlayerAppearance = MapAppearanceDTO(x.Appearance)
                    })
                    .ToList()
            };

            return result;
        }

        private AppearanceRuntimeDTO MapAppearanceDTO(AppearanceDocument appearance)
        {
            return new AppearanceRuntimeDTO
            {
                SkinID = appearance.SkinID,
                HairID = appearance.HairID ?? string.Empty,
                EyesID = appearance.EyesID ?? string.Empty,
                ShirtID = appearance.ShirtID ?? string.Empty,
                PantID = appearance.PantID ?? string.Empty,

                // SkinColor is mandatory: Direct mapping (no null check needed)
                SkinColor = new HSVDTO
                {
                    H = appearance.SkinColor.H,
                    S = appearance.SkinColor.S,
                    V = appearance.SkinColor.V
                },

                // Hair/Pant are nullable: Use the helper
                HairColor = MapHSVDTO(appearance.HairColor),
                PantColor = MapHSVDTO(appearance.PantColor)
            };
        }

        private HSVDTO? MapHSVDTO(HSVDocument? hsv)
        {
            // Return null if the source is null
            if (hsv == null) return null;

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