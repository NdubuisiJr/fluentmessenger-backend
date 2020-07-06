using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;

namespace FluentMessenger.API.AutoMapperProfiles {
    public class UserToDtosProfile : Profile {
        public UserToDtosProfile() {
            CreateMap<User, UserDto>();
            CreateMap<UserForCreationDto, User>();
            CreateMap<User, UserForUpdateDto>();
            CreateMap<UserForUpdateDto, User>();
        }
    }
}
