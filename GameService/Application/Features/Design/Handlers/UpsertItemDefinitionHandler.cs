using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Services.DesignService;

namespace Application.Features.Design.Handlers
{
    public class UpsertItemDefinitionHandler : IHandler<UpsertItemDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly ItemDefinitionService itemDefinitionService;
        #endregion

        #region Properties
        #endregion

        public UpsertItemDefinitionHandler(
            IRelationalUoW relationalUoW,
            ItemDefinitionService itemDefinitionService)
        {
            this.relationalUoW = relationalUoW;
            this.itemDefinitionService = itemDefinitionService;
        }

        #region Methods
        public async Task Handle(
            UpsertItemDefinitionCommand command)
        {
            await itemDefinitionService.UpsertWithoutSave(command.DTO);
            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}