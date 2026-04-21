namespace Application.ApplicationException
{
    public class NotFound : Exception
    {
        public NotFound(string message) : base(message) { }
    }
}