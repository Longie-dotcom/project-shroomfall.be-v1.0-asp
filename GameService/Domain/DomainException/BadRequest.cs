namespace Domain.DomainException
{
    public class BadRequest : Exception
    {
        public BadRequest(string message) : base(message) { }
    }
}