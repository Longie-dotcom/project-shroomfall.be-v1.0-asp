using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Services.DesignService;

namespace Application.Features.Design.Handlers
{
    public class UpsertEntityDefinitionHandler : IHandler<UpsertEntityDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly EntityDefinitionService entityDefinitionService;
        #endregion

        #region Properties
        #endregion

        public UpsertEntityDefinitionHandler(
            IRelationalUoW relationalUoW,
            EntityDefinitionService entityDefinitionService)
        {
            this.relationalUoW = relationalUoW;
            this.entityDefinitionService = entityDefinitionService;
        }

        #region Methods
        public async Task Handle(
            UpsertEntityDefinitionCommand command)
        {
            await entityDefinitionService.UpsertWithoutSave(command.DTO);
            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}