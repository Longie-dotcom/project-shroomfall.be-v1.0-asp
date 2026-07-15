using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Contract.DTO.Feature.Game.Response;

namespace Infrastructure.Realtime.Events.Game
{
    public class EntityActedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public EntityActedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not EntityActedEvent acted)
                return;

            await publisher.SendEntityActed(
                acted.RoomSpatialID,
                new EntityActedDTO()
                {
                    X = acted.Position.X,
                    Y = acted.Position.Y,
                    Direction = acted.Direction,
                    Action = acted.Action,
                    EntityInstanceID = acted.EntityInstanceID,
                    UsedItemDefinitionID = acted.UsedItemDefinitionID,
                });
        }
        #endregion
    }
}