namespace Domain.DomainException
{
    public class NotFound : Exception
    {
        public NotFound(string message) : base(message) { }
    }
}
