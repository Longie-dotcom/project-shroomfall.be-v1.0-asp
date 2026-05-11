using Domain.Other.IdentityDomain;

namespace Application.Services.Abstraction.OtherService
{
    public interface ITokenService
    {
        (string access, string refresh) Generate(
            User user);
    }
}
