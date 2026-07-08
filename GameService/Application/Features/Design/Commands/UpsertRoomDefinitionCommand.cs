using Microsoft.AspNetCore.Http;

namespace Application.Features.Design.Commands
{
    public class UpsertRoomDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public IFormFile File { get; }
        #endregion

        public UpsertRoomDefinitionCommand(
            IFormFile file)
        {
            File = file;
        }

        #region Methods
        #endregion
    }
}