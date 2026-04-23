namespace Domain.DomainException
{
    public class Unauthorized : Exception
    {
        public Unauthorized(string message) : base(message) { }
    }
}
