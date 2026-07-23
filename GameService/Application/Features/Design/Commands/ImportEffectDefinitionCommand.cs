using Microsoft.AspNetCore.Http;

namespace Application.Features.Design.Commands
{
    public class ImportEffectDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public IFormFile File { get; }
        #endregion

        public ImportEffectDefinitionCommand(
            IFormFile file)
        {
            File = file;
        }

        #region Methods
        #endregion
    }
}
