namespace Domain.DomainException
{
    public class NotFound : Exception
    {
        #region Attributes
        #endregion

        #region Properties
        public string Code { get; }
        #endregion

        public NotFound(
            string code,
            string? message = "") : base(message)
        {
            Code = code;
        }

        #region Methods
        #endregion
    }
}
