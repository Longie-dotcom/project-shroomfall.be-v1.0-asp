namespace Domain.DomainException
{
    public class Unauthorized : Exception
    {
        #region Attributes
        #endregion

        #region Properties
        public string Code { get; }
        #endregion

        public Unauthorized(
            string code,
            string? message = "") : base(message)
        {
            Code = code;
        }

        #region Methods
        #endregion
    }
}
