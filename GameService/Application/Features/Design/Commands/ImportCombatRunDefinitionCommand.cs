using Microsoft.AspNetCore.Http;

namespace Application.Features.Design.Commands
{
    public class ImportCombatRunDefinitionCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public IFormFile File { get; }
        #endregion

        public ImportCombatRunDefinitionCommand(
            IFormFile file)
        {
            File = file;
        }

        #region Methods
        #endregion
    }
}
