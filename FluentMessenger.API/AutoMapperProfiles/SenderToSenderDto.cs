using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;

namespace FluentMessenger.API.AutoMapperProfiles {
    public class SenderToSenderDto : Profile {
        public SenderToSenderDto() {
            CreateMap<Sender, SenderIdDto>();
            CreateMap<SenderIdDto, Sender>();
        }
    }
}
