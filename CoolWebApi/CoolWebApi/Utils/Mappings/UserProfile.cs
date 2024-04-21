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
            // CreateMap<ChatUserResponse, BlazorHeroUser>().ReverseMap()
            //     .ForMember(dest => dest.EmailAddress, source => source.MapFrom(source => source.Email)); //Specific Mapping
        }
    }
}