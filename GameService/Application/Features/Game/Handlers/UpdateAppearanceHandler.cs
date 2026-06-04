using Application.Context;
using Application.Events.Event;
using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Realtime;
using Application.Interfaces.Security;
using AutoMapper;
using Contract.DTO.Runtime;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Features.Game.Handlers
{
    public class UpdateAppearanceHandler : IHandler<UpdateAppearanceCommand>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly WorldContext worldContext;
        private readonly ISessionManager sessionManager;
        private readonly IEventBus eventBus;
        #endregion

        #region Properties
        #endregion

        public UpdateAppearanceHandler(
            IMapper mapper,
            WorldContext worldContext,
            ISessionManager sessionManager,
            IEventBus eventBus)
        {
            this.mapper = mapper;
            this.worldContext = worldContext;
            this.sessionManager = sessionManager;  
            this.eventBus = eventBus;
        }

        #region Methods
        public async Task Handle(
            UpdateAppearanceCommand command)
        {
            var dto = command.DTO;

            // Validate session existence
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (string.IsNullOrWhiteSpace(playerInstanceId))
                throw new Unauthorized(
                    ResponseCode.Move_SessionNotFound,
                    $"User with user ID: {command.UserID} has no session");

            // Validate player instance existence
            var player = worldContext.GetEntity<PlayerInstance>(playerInstanceId);
            if (player == null)
                throw new BadRequest(
                    ResponseCode.Move_PlayerInstanceNotFound,
                    $"User with user ID: {command.UserID} has no player instance");

            // Update appearance
            player.UpdateAppearance(
                dto.SkinID,
                new HSV(dto.SkinColor.H, dto.SkinColor.S, dto.SkinColor.V),
                dto.HairID,
                dto.EyesID,
                dto.ShirtID,
                dto.PantID,
                new HSV(dto.HairColor.H, dto.HairColor.S, dto.HairColor.V),
                new HSV(dto.PantColor.H, dto.PantColor.S, dto.PantColor.V)
            );

            // Publish changes
            eventBus.Publish(new PlayerAppearanceChangedEvent(
                playerInstanceId,
                player.RoomSpatialID,
                mapper.Map<PlayerAppearanceRuntimeDTO>(player.Appearance)));

            await Task.CompletedTask;
        }
        #endregion
    }
}