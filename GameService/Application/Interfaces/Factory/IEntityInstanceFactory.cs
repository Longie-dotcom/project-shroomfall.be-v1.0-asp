using Domain.Document.EntityDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Interfaces.Factory
{
    public interface IEntityInstanceFactory
    {
        EntityInstance CreateFromDocument(EntityDocument doc);
    }
}