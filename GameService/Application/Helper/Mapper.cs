using Application.DTO;
using AutoMapper;
using Domain.IdentityDomain;

namespace Application.Helper
{
    public class Mapper : Profile
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public Mapper()
        {
            CreateMap<User, UserDTO>();
        }
    }
}