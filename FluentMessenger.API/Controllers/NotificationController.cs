using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FluentMessenger.API.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/users/{userId}/notification")]
    public class NotificationController : ControllerBase {
        private IRepository<User> _userRepo;
        private IMapper _mapper;

        public NotificationController(IRepository<User> repositoryService, IMapper mapper) {
            _userRepo = repositoryService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a user's notification given the notification Id
        /// </summary>
        /// <param name="userId">user's Id</param>
        /// <param name="id">Notification Id</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{id}",Name = "GetNotification")]
        public ActionResult<NotificationDto> Get(int userId, int id) {
            var user = _userRepo.Get(userId, true);

            if (user == null) {
                return NotFound();
            }

            var notification = user.Notifications.FirstOrDefault(x => x.Id == id);
            if (notification is null) {
                return NotFound();
            }

            return Ok(_mapper.Map<NotificationDto>(notification));
        }

        /// <summary>
        /// Creates a new notification entry into the database
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="notificationDto">The incoming notification object</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public ActionResult<NotificationDto> CreateNotification(int userId, NotificationDto notificationDto) {
            var user = _userRepo.Get(userId, true);

            if (user == null) {
                return NotFound();
            }

            if(notificationDto == null) {
                return BadRequest();
            }

            var notification = _mapper.Map<Notification>(notificationDto);
            var notifications = user.Notifications != null ?
                                user.Notifications.ToList() :
                                new List<Notification>();
            notifications.Add(notification);
            user.Notifications = notifications;
            _userRepo.Update(user);
            _userRepo.SaveChanges();

            return CreatedAtRoute("GetNotification", new { userId, id = notification.Id },notificationDto);
        }

        /// <summary>
        /// Updates a notification
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="id">The notification Id</param>
        /// <param name="patchDocument">the json patch document</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{Id}")]
        public IActionResult Update (int userId, int id,
            JsonPatchDocument<NotificationDto> patchDocument) {
            var user = _userRepo.Get(userId, true);

            if (user is null) {
                return NotFound();
            }

            if (patchDocument is null) {
                return BadRequest();
            }

            var notification = user.Notifications.FirstOrDefault(x => x.Id == id);
            if (notification is null) {
                return BadRequest();
            }

            var notificationDto = _mapper.Map<NotificationDto>(notification);
            patchDocument.ApplyTo(notificationDto, ModelState);
            if (!TryValidateModel(notificationDto)) {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(notificationDto, notification);

            _userRepo.Update(user);
            _userRepo.SaveChanges();
            return NoContent();
        }
    }
}
