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
    [Route("api/users/{userId}/DraftTemplate")]
    public class DraftTemplateController : ControllerBase {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Message> _messageRepo;
        private readonly IRepository<Group> _groupRepo;
        private readonly IMapper _mapper;

        public DraftTemplateController(IRepository<User> userRepo,
           IMapper mapper, IRepository<Message> messageRepo, IRepository<Group> groupRepo) {
            _userRepo = userRepo;
            _mapper = mapper;
            _messageRepo = messageRepo;
            _groupRepo = groupRepo;
        }
        [HttpGet("{groupId}")]
        public ActionResult<IEnumerable<MessageDto>> GetDrafts(int userId, int groupId) {
            var user = _userRepo.Get(userId, true);
            if (user == null) {
                return NotFound();
            }

            var group = user.Groups.SingleOrDefault(x => x.Id == groupId);
            if (group == null) {
                return NotFound();
            }

            group = _groupRepo.LoadRefrencesTypes(group);
            var drafts = group.Messages.Where(x => x.IsDraft).Select(x => x);
            return Ok(_mapper.Map<IEnumerable<MessageDto>>(drafts));
        }

        [HttpPost("{groupId}")]
        public ActionResult<IEnumerable<MessageDto>> UpdateDraft(int userId, int groupId,
            MessageDto messageDto) {
            if (messageDto is null) {
                return BadRequest();
            }

            var user = _userRepo.Get(userId, true);
            if (user == null) {
                return NotFound();
            }

            var group = user.Groups.SingleOrDefault(x => x.Id == groupId);
            if (group == null) {
                return NotFound();
            }

            var message = _mapper.Map<Message>(messageDto);
            message.IsDraft = false;
            _messageRepo.Update(message);
            _messageRepo.SaveChanges();

            return NoContent();
        }
    }
}
