using Application.Systems.Abstraction;
using System.Collections.Concurrent;

namespace Application.Systems.Queue
{
    public class CommandBuffer
    {
        #region Attributes
        #endregion

        #region Properties
        public ConcurrentQueue<IEntityCommand> Commands { get; } = new();
        public ConcurrentQueue<IEntityResult> Results { get; } = new();
        #endregion

        public CommandBuffer() { }

        #region Methods
        #endregion
    }
}