using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FluentMessenger.API.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/users/{userId}/ContactMessage")]
    public class ContactMessgeController : ControllerBase {
        private readonly IContactMessageRepository<ContactMessagesReceived> _receivedRepo;
        private readonly IContactMessageRepository<ContactMessagesNotReceived> _notReceivedRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Contact> _contactRepo;
        private readonly IRepository<Message> _messageRepo;
        private readonly IMapper _mapper;

        public ContactMessgeController(IContactMessageRepository<ContactMessagesReceived> receivedRepo,
            IContactMessageRepository<ContactMessagesNotReceived> notReceivedRepo,
            IRepository<User> userRepo,
            IRepository<Contact> contactRepo,
            IRepository<Message> messageRepo,
            IMapper mapper) {
            _receivedRepo = receivedRepo;
            _notReceivedRepo = notReceivedRepo;
            _userRepo = userRepo;
            _contactRepo = contactRepo;
            _messageRepo = messageRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll(int userId,
            [FromQuery] ContactMessageQuery contactMessageQuery) {

            var user = _userRepo.Get(userId);

            if (user is null || contactMessageQuery is null ||
                !contactMessageQuery.IsConsistent()) {
                return BadRequest();
            }

            if (contactMessageQuery.ContactId > 0) {
                if (_contactRepo.Get(contactMessageQuery.ContactId) is null)
                    return BadRequest();

                var messagesReceived = _receivedRepo.GetMessages(contactMessageQuery.ContactId);
                var messagesNotReceived = _notReceivedRepo.GetMessages(contactMessageQuery.ContactId);
                return Ok(new {
                    ReceivedMessages = _mapper.Map<IEnumerable<MessageDto>>(messagesReceived),
                    MissedMessages = _mapper.Map<IEnumerable<MessageDto>>(messagesNotReceived)
                });
            }
            else {
                if (_messageRepo.Get(contactMessageQuery.MessageId) is null)
                    return BadRequest();

                var contactReceived = _receivedRepo.GetContacts(contactMessageQuery.MessageId);
                var contactNotReceived = _notReceivedRepo.GetContacts(contactMessageQuery.MessageId);
                return Ok(new {
                    ReceivedContacts = _mapper.Map<IEnumerable<ContactDto>>(contactReceived),
                    MissedContacts = _mapper.Map<IEnumerable<ContactDto>>(contactNotReceived)
                });
            }
        }

        [HttpPost]
        public IActionResult CreateContactMessage(int userId,
            [FromBody] ContactMessageForCreationDto contactMessageForCreation) {
            var user = _userRepo.Get(userId);
            if (user is null ||
                contactMessageForCreation is null || 
                !contactMessageForCreation.IsConsistent()) {
                return BadRequest();
            }
            var check = contactMessageForCreation.IsReceived;
            for (int i = 0; i < contactMessageForCreation.ContactKeys.Count(); i++) {
                if (check)
                    _receivedRepo.Add(new ContactMessagesReceived {
                        ContactId = contactMessageForCreation.ContactKeys.ElementAt(i),
                        MessageId = contactMessageForCreation.MessageKeys.ElementAt(i)
                    });
                else
                    _notReceivedRepo.Add(new ContactMessagesNotReceived {
                        ContactId = contactMessageForCreation.ContactKeys.ElementAt(i),
                        MessageId = contactMessageForCreation.MessageKeys.ElementAt(i)
                    });
            }
            if (check)
                _receivedRepo.SaveChanges();
            else
                _notReceivedRepo.SaveChanges();

            return NoContent();
        }

        [HttpPost("{groupId}")]
        public IActionResult CreateContactMessage(int userId, int groupId,
           [FromBody] ContactMessageDto contactMessage) {
            var user = _userRepo.Get(userId, true);

            if (user is null || contactMessage is null ||
                !contactMessage.IsConsistent()) {
                return BadRequest();
            }

            var group = user.Groups.FirstOrDefault(x => x.Id == groupId);
            if (group is null) {
                return BadRequest();
            }

            if (contactMessage.ContactsDtos is { } contactsForCreation) {
                var contacts = _mapper.Map<IEnumerable<Contact>>(contactsForCreation);
                foreach (var contact in contacts)
                    contact.GroupId = groupId;

                _contactRepo.AddRange(contacts);
                _contactRepo.SaveChanges();
                return Ok(_mapper.Map<IEnumerable<ContactDto>>(contacts));
            }
            else {
                var messages = _mapper.Map<IEnumerable<Message>>(contactMessage.MessageDtos);
                foreach (var message in messages)
                    message.GroupId = groupId;

                _messageRepo.AddRange(messages);
                _contactRepo.SaveChanges();
                return Ok(_mapper.Map<IEnumerable<MessageDto>>(messages));
            }
        }
    }
}
