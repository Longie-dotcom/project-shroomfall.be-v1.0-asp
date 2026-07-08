namespace Domain.DomainException
{
    public class InternalException : Exception
    {
        #region Attributes
        #endregion

        #region Properties
        public string Code { get; }
        #endregion

        public InternalException(
            string code,
            string? message = "") : base(message)
        {
            Code = code;
        }

        #region Methods
        #endregion
    }
}