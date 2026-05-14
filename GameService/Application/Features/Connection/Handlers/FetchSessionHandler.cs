using Application.DTO.Common;
using Application.DTO.Connection;
using Application.DTO.Runtime;
using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Repository.NonRelational;

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

                        PlayerAppearance = new PlayerAppearanceRuntimeDTO
                        {
                            SkinID = x.PlayerAppearance.SkinID,

                            HairID = x.PlayerAppearance.HairID,
                            GlassesID = x.PlayerAppearance.GlassesID,
                            ShirtID = x.PlayerAppearance.ShirtID,
                            PantID = x.PlayerAppearance.PantID,
                            ShoeID = x.PlayerAppearance.ShoeID,
                            EyesID = x.PlayerAppearance.EyesID,

                            SkinColor = new HSVDTO
                            {
                                H = x.PlayerAppearance.SkinColor.H,
                                S = x.PlayerAppearance.SkinColor.S,
                                V = x.PlayerAppearance.SkinColor.V
                            },

                            HairColor = new HSVDTO
                            {
                                H = x.PlayerAppearance.HairColor.H,
                                S = x.PlayerAppearance.HairColor.S,
                                V = x.PlayerAppearance.HairColor.V
                            },

                            EyeColor = new HSVDTO
                            {
                                H = x.PlayerAppearance.EyeColor.H,
                                S = x.PlayerAppearance.EyeColor.S,
                                V = x.PlayerAppearance.EyeColor.V
                            },

                            PantColor = new HSVDTO
                            {
                                H = x.PlayerAppearance.PantColor.H,
                                S = x.PlayerAppearance.PantColor.S,
                                V = x.PlayerAppearance.PantColor.V
                            }
                        }
                    })
                    .ToList()
            };

            return result;
        }
        #endregion
    }
}