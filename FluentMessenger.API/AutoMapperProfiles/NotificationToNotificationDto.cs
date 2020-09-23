using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentMessenger.API.AutoMapperProfiles {
    public class NotificationToNotificationDto: Profile {
        public NotificationToNotificationDto() {
            CreateMap<Notification, NotificationDto>();
            CreateMap<NotificationDto, Notification>();
        }
    }
}
