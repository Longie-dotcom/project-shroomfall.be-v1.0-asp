using Application.Systems.Queue;

namespace Application.Systems.Abstraction
{
    public interface ITickService
    {
        void Tick(
            float dt, 
            CommandBuffer buffer);
    }
}
