using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Services.DesignService;

namespace Application.Features.Design.Handlers
{
    public class UpsertCombatRunDefinitionHandler : IHandler<UpsertCombatRunDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly CombatRunDefinitionService combatRunDefinitionService;
        #endregion

        #region Properties
        #endregion

        public UpsertCombatRunDefinitionHandler(
            IRelationalUoW relationalUoW,
            CombatRunDefinitionService combatRunDefinitionService)
        {
            this.relationalUoW = relationalUoW;
            this.combatRunDefinitionService = combatRunDefinitionService;
        }

        #region Methods
        public async Task Handle(
            UpsertCombatRunDefinitionCommand command)
        {
            await combatRunDefinitionService.UpsertWithoutSave(command.DTO);
            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}