using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Services.DesignService;

namespace Application.Features.Design.Handlers
{
    public class UpsertEffectDefinitionHandler : IHandler<UpsertEffectDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly EffectDefinitionService effectDefinitionService;
        #endregion

        #region Properties
        #endregion

        public UpsertEffectDefinitionHandler(
            IRelationalUoW relationalUoW,
            EffectDefinitionService effectDefinitionService)
        {
            this.relationalUoW = relationalUoW;
            this.effectDefinitionService = effectDefinitionService;
        }

        #region Methods
        public async Task Handle(
            UpsertEffectDefinitionCommand command)
        {
            await effectDefinitionService.UpsertWithoutSave(command.DTO);
            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}