using Microsoft.AspNetCore.Http;

namespace Application.Features.Design.Commands
{
    public class ImportRoomDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public IFormFile File { get; }
        #endregion

        public ImportRoomDefinitionCommand(
            IFormFile file)
        {
            File = file;
        }

        #region Methods
        #endregion
    }
}