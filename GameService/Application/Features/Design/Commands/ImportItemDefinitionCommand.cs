using Microsoft.AspNetCore.Http;

namespace Application.Features.Design.Commands
{
    public class ImportItemDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public IFormFile File { get; }
        #endregion

        public ImportItemDefinitionCommand(
            IFormFile file)
        {
            File = file;
        }

        #region Methods
        #endregion
    }
}
