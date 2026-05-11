using Domain.Document.EntityDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Interfaces.Factory
{
    public interface IEntityDocumentFactory
    {
        EntityInstance CreateFromDocument(EntityDocument doc);
    }
}