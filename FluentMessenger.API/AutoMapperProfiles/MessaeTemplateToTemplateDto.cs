using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;

namespace FluentMessenger.API.AutoMapperProfiles {
    public class MessaeTemplateToTemplateDto : Profile {
        public MessaeTemplateToTemplateDto() {
            CreateMap<MessageTemplate, TemplateDto>();
            CreateMap<TemplateDto, MessageTemplate>();
        }
    }
}
