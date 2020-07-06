using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;

namespace FluentMessenger.API.AutoMapperProfiles {
    public class ContactToDtos : Profile{
        public ContactToDtos() {
            CreateMap<Contact, ContactDto>()
                .ForMember(x => x.Number, y => y.MapFrom(z => z.PhoneNumber));
            CreateMap<ContactDto, Contact>()
                .ForMember(x => x.PhoneNumber, y => y.MapFrom(z => z.Number));
        }
    }
}
