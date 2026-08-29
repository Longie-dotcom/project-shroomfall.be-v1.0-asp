using Application.System.Queue;

namespace Application.System.Abstraction
{
    public interface ITickService
    {
        void Tick(
            float dt, 
            CommandBuffer buffer);
    }
}
