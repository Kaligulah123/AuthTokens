using AuthTokens.Domain.Contracts;
using AuthTokens.Domain.Entities;
using AutoMapper;

namespace AuthTokens.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserResponse>();
            CreateMap<ApplicationUser, CurrentUserResponse>();
            CreateMap<UserRegisterRequest, ApplicationUser>();

        }
    }
}
