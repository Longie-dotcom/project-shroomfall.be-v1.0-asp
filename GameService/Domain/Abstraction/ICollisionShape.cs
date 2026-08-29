using Contract.Common;

namespace Domain.Abstraction
{
    public interface ICollisionShape
    {
        bool IsBlocking { get; }

        int ComputeCells(
            Vector2 position, 
            Span<(int x, int y)> output);
        bool Intersects(
            Vector2 selfPosition,
            ICollisionShape other,
            Vector2 otherPosition);
    }
}
