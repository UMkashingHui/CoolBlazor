using AutoMapper;
using CoolWebApi.Models.Identity;
using CoolWebApi.Models.Responses.Identity;

namespace CoolWebApi.Utils.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserResponse, CoolBlazorUser>().ReverseMap();
            CreateMap<RoleResponse, CoolBlazorRole>().ReverseMap();
            CreateMap<RoleClaimResponse, CoolBlazorRoleClaim>().ReverseMap();
        }
    }
}