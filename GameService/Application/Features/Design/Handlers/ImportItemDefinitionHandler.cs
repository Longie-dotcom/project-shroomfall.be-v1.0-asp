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
    internal class ImportItemDefinitionHandler : IHandler<ImportItemDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly ItemDefinitionService itemDefinitionService;
        #endregion

        #region Properties
        #endregion

        public ImportItemDefinitionHandler(
            IRelationalUoW relationalUoW,
            ItemDefinitionService itemDefinitionService)
        {
            this.relationalUoW = relationalUoW;
            this.itemDefinitionService = itemDefinitionService;
        }

        #region Methods
        public async Task Handle(
            ImportItemDefinitionCommand command)
        {
            // Validate json file
            if (command.File == null || command.File.Length == 0)
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.ItemFilePayloadEmpty,
                    "The uploaded item definition file is null or empty.");

            try
            {
                // Deserialize json file
                await using var stream = command.File.OpenReadStream();
                var dtos = await JsonSerializer.DeserializeAsync<List<ItemDefinitionDTO>>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Validate deserialized dtos
                if (dtos == null)
                    throw new BadRequest(
                        ApplicationCode.DesignHandlerCode.ItemFileSchemaParseFailed,
                        "The uploaded file does not contain a valid list of item definitions.");

                // Save changes
                await relationalUoW.BeginTransactionAsync();
                foreach (var dto in dtos)
                {
                    await itemDefinitionService.UpsertWithoutSave(dto);
                }
                await relationalUoW.CommitAsync();
            }
            catch (JsonException ex)
            {
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.ItemFileInvalidJson,
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