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
        private readonly IRepository<MessageTemplate> _templateRepo;
        private readonly IMapper _mapper;

        public DraftTemplateController(IRepository<User> userRepo, IMapper mapper, 
            IRepository<Message> messageRepo, IRepository<Group> groupRepo,
           IRepository<MessageTemplate> templateRepo) {
            _userRepo = userRepo;
            _mapper = mapper;
            _messageRepo = messageRepo;
            _groupRepo = groupRepo;
            _templateRepo = templateRepo;
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
        /// <param name="messageId">Id for the message to update</param>
        /// <returns></returns>
        [HttpPost("{groupId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<IEnumerable<MessageDto>> UpdateDraft(int userId, int groupId,
            [FromQuery]int messageId) {

            var user = _userRepo.Get(userId, true);
            if (user == null) {
                return NotFound();
            }

            var group = user.Groups.SingleOrDefault(x => x.Id == groupId);
            if (group == null) {
                return NotFound();
            }
            group = _groupRepo.LoadRefrencesTypes(group);

            var message = group.Messages.SingleOrDefault(x => x.Id == messageId);
            if (message is null) {
                return NotFound();
            }
            message.IsDraft = false;
            _messageRepo.Update(message);
            _messageRepo.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Gets all the message templates defined by a user.\
        /// Requiress a Bearer token
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        [HttpGet(Name ="GetTemplates")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TemplateDto>> GetTemplates(int userId) {
            var user = _userRepo.Get(userId, true);
            if (user == null) {
                return NotFound();
            }

            var templates = _templateRepo.GetAll().Where(x => x.UserId == userId);
            
            return Ok(_mapper.Map<IEnumerable<TemplateDto>>(templates));
        }

        /// <summary>
        /// Creates a message template.\
        /// Requires a Bearer Token
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="templateDto">The template object</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<TemplateDto> CreateTemplates(int userId, TemplateDto templateDto) {
            var user = _userRepo.Get(userId, true);
            if (user == null) {
                return NotFound();
            }

            if (templateDto is null) {
                return BadRequest();
            }

            var messageTemplate = _mapper.Map<MessageTemplate>(templateDto);
            messageTemplate.UserId = userId;

            _templateRepo.Add(messageTemplate);
            _templateRepo.SaveChanges();

            return CreatedAtRoute("GetTemplates", new { userId = messageTemplate.UserId },
                _mapper.Map<TemplateDto>(messageTemplate));
        }

        /// <summary>
        /// Deletes a message template.\
        /// Requires Bearer token
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="templateId">The template Id</param>
        /// <returns></returns>
        [HttpDelete("{templateId}")]
        public ActionResult DeleteTemplate(int userId, int templateId) {
            var user = _userRepo.Get(userId, true);
            if (user == null) {
                return NotFound();
            }

            var template = _templateRepo.Get(templateId);
            if (template is null) {
                return NotFound();
            }

            _templateRepo.Delete(template);
            _templateRepo.SaveChanges();

            return NoContent();
        }
    }
}
