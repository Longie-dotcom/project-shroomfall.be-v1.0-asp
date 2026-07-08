namespace Domain.DomainException
{
    public class BadRequest : Exception
    {
        #region Attributes
        #endregion

        #region Properties
        public string Code { get; }
        #endregion

        public BadRequest(
            string code, 
            string? message = "") : base(message) 
        {
            Code = code;
        }

        #region Methods
        #endregion
    }
}