using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;

namespace FluentMessenger.API.AutoMapperProfiles {
    public class GroupToDtosProfile  : Profile{
        public GroupToDtosProfile() {
            CreateMap<Group, GroupDto>()
                .ForMember(x => x.Contacts, y => y.MapFrom(z => z.Contacts))
                .ForMember(x => x.Messages, y => y.MapFrom(z => z.Messages));

            CreateMap<Group, GroupForCreationDto>()
                .ForMember(x => x.Contacts, y => y.MapFrom(z => z.Contacts))
                .ForMember(x => x.Messages, y => y.MapFrom(z => z.Messages));

            CreateMap<GroupForCreationDto, Group>()
                .ForMember(x => x.Contacts, y => y.MapFrom(z => z.Contacts))
                .ForMember(x => x.Messages, y => y.MapFrom(z => z.Messages));
        }
    }
}
