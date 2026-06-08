using Application.Features.Abstraction;

namespace Application.Features.Identity.Commands
{
    public class UpdatePreferredLocaleCommand : ICommand<string>
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string PreferredLocale { get; }
        #endregion

        public UpdatePreferredLocaleCommand(
            string userId,
            string preferredLocale)
        {
            UserID = userId;
            PreferredLocale = preferredLocale;
        }

        #region Methods
        #endregion
    }
}