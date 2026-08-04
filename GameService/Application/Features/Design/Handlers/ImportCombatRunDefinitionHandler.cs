using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Services.DesignService;
using Contract.DTO.Definition.WorldDomain;
using Domain.DomainException;
using ResponseCode;
using System.Text.Json;

namespace Application.Features.Design.Handlers
{
    internal class ImportCombatRunDefinitionHandler : IHandler<ImportCombatRunDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly CombatRunDefinitionService combatRunDefinitionService;
        #endregion

        #region Properties
        #endregion

        public ImportCombatRunDefinitionHandler(
            IRelationalUoW relationalUoW,
            CombatRunDefinitionService combatRunDefinitionService)
        {
            this.relationalUoW = relationalUoW;
            this.combatRunDefinitionService = combatRunDefinitionService;
        }

        #region Methods
        public async Task Handle(
            ImportCombatRunDefinitionCommand command)
        {
            // Validate json file
            if (command.File == null || command.File.Length == 0)
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.CombatRunFilePayloadEmpty,
                    "The uploaded combat run definition file is null or empty.");

            try
            {
                // Deserialize json file
                await using var stream = command.File.OpenReadStream();
                var dtos = await JsonSerializer.DeserializeAsync<List<CombatRunDefinitionDTO>>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Validate deserialized dtos
                if (dtos == null)
                    throw new BadRequest(
                        ApplicationCode.DesignHandlerCode.CombatRunFileSchemaParseFailed,
                        "The uploaded file does not contain a valid list of combat run definitions.");

                // Save changes
                await relationalUoW.BeginTransactionAsync();
                foreach (var dto in dtos) { await combatRunDefinitionService.UpsertWithoutSave(dto); }
                await relationalUoW.CommitAsync();
            }
            catch (JsonException ex)
            {
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.CombatRunFileInvalidJson,
                    $"Failed to deserialize JSON stream due to formatting errors: {ex.Message}");
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
