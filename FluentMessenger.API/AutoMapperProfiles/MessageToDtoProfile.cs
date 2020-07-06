using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;

namespace FluentMessenger.API.AutoMapperProfiles {
    public class MessageToDtoProfile : Profile {
        public MessageToDtoProfile() {
            CreateMap<Message, MessageDto>()
                .ForMember(x => x.Message, y => y.MapFrom(z => z.Value));
            CreateMap<MessageDto, Message>()
                .ForMember(x => x.Value, y => y.MapFrom(z => z.Message));
        }
    }
}
