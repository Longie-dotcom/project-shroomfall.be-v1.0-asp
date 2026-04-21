namespace Application.ApplicationException
{
    public class BadRequest : Exception
    {
        public BadRequest(string message) : base(message) { }
    }
}