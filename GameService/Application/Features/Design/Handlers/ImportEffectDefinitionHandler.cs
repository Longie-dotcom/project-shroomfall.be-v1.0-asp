using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Services.DesignService;
using Contract.DTO.Definition.MetaDomain;
using Domain.DomainException;
using ResponseCode;
using System.Text.Json;

namespace Application.Features.Design.Handlers
{
    internal class ImportEffectDefinitionHandler : IHandler<ImportEffectDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly EffectDefinitionService effectDefinitionService;
        #endregion

        #region Properties
        #endregion

        public ImportEffectDefinitionHandler(
            IRelationalUoW relationalUoW,
            EffectDefinitionService effectDefinitionService)
        {
            this.relationalUoW = relationalUoW;
            this.effectDefinitionService = effectDefinitionService;
        }

        #region Methods
        public async Task Handle(
            ImportEffectDefinitionCommand command)
        {
            // Validate json file
            if (command.File == null || command.File.Length == 0)
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.EffectFilePayloadEmpty,
                    "The uploaded effect definition file is null or empty.");

            try
            {
                // Deserialize json file
                await using var stream = command.File.OpenReadStream();
                var dtos = await JsonSerializer.DeserializeAsync<List<EffectDefinitionDTO>>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Validate deserialized dtos
                if (dtos == null)
                    throw new BadRequest(
                        ApplicationCode.DesignHandlerCode.EffectFileSchemaParseFailed,
                        "The uploaded file does not contain a valid list of effect definitions.");

                // Save changes
                await relationalUoW.BeginTransactionAsync();
                foreach (var dto in dtos) { await effectDefinitionService.UpsertWithoutSave(dto); }
                await relationalUoW.CommitAsync();
            }
            catch (JsonException ex)
            {
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.EffectFileInvalidJson,
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
