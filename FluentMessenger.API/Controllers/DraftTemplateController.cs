using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Gets all draft created by a user for a give group.\
        /// requires Bearer Token
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="groupId">The group's Id</param>
        /// <returns></returns>
        [HttpGet("{groupId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        /// <summary>
        /// Updates draft.\
        /// Requires Bearer Token
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="groupId">The group's Id</param>
        /// <param name="messageDto">The Draft object</param>
        /// <returns></returns>
        [HttpPost("{groupId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
