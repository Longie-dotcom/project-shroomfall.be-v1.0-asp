using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Services.DesignService;
using Contract.DTO.Definition.EntityDomain.Component;
using Domain.DomainException;
using ResponseCode;
using System.Text.Json;

namespace Application.Features.Design.Handlers
{
    internal class ImportEntityDefinitionHandler : IHandler<ImportEntityDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly EntityDefinitionService entityDefinitionService;
        #endregion

        #region Properties
        #endregion

        public ImportEntityDefinitionHandler(
            IRelationalUoW relationalUoW,
            EntityDefinitionService entityDefinitionService)
        {
            this.relationalUoW = relationalUoW;
            this.entityDefinitionService = entityDefinitionService;
        }

        #region Methods
        public async Task Handle(
            ImportEntityDefinitionCommand command)
        {
            // Validate json file
            if (command.File == null || command.File.Length == 0)
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.EntityFilePayloadEmpty,
                    "The uploaded entity definition file is null or empty.");

            try
            {
                // Deserialize json file
                await using var stream = command.File.OpenReadStream();
                var dtos = await JsonSerializer.DeserializeAsync<List<EntityDefinitionDTO>>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Validate deserialized dtos
                if (dtos == null)
                    throw new BadRequest(
                        ApplicationCode.DesignHandlerCode.EntityFileSchemaParseFailed,
                        "The uploaded file does not contain a valid list of entity definitions.");

                // Save changes
                await relationalUoW.BeginTransactionAsync();
                foreach (var dto in dtos)
                {
                    await entityDefinitionService.UpsertWithoutSave(dto);
                }
                await relationalUoW.CommitAsync();
            }
            catch (JsonException ex)
            {
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.EntityFileInvalidJson,
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