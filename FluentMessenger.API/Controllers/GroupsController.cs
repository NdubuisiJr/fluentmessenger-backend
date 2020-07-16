using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Exceptions;
using FluentMessenger.API.Extensions;
using FluentMessenger.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace FluentMessenger.API.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/users/{userId}/groups")]
    public class GroupsController : ControllerBase {

        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Group> _groupRepo;
        private readonly IMapper _mapper;
        private readonly IRepository<Message> _messageRepo;

        public GroupsController(IRepository<User> userRepo,
            IRepository<Group> groupRepo, IMapper mapper, IRepository<Message> messageRepo) {
            _userRepo = userRepo;
            _groupRepo = groupRepo;
            _mapper = mapper;
            _messageRepo = messageRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GroupDto>> GetGroups(int userId) {

            var user = _userRepo.Get(userId);
            if (user == null) {
                return NotFound();
            }

            var groups = _groupRepo.GetAll(true).Where(x => x.UserId == userId);
            return Ok(_mapper.Map<IEnumerable<GroupDto>>(groups));
        }

        [HttpGet("{groupId}", Name = "GetGroup")]
        public ActionResult<GroupDto> GetGroup(int userId, int groupId) {
            var user = _userRepo.Get(userId, true);
            if (user == null) {
                return NotFound();
            }

            var group = user.Groups.SingleOrDefault(x => x.Id == groupId);
            if (group == null) {
                return NotFound();
            }
            group = _groupRepo.LoadRefrencesTypes(group);
            return Ok(_mapper.Map<GroupDto>(group));
        }

        [HttpDelete("{groupId}")]
        public ActionResult DeleteUser(int userId, int groupId) {

            var user = _userRepo.Get(userId, true);
            if (user == null) {
                return NotFound();
            }

            var group = user.Groups.SingleOrDefault(x => x.Id == groupId);

            if (group == null) {
                return NotFound();
            }

            _groupRepo.Delete(group);
            _groupRepo.SaveChanges();
            return NoContent();
        }

        [HttpPost]
        public ActionResult<GroupDto> CreateGroup(int userId,
            [FromBody] GroupForCreationDto groupForCreationDto) {

            if (groupForCreationDto == null) {
                return BadRequest();
            }

            var user = _userRepo.Get(userId);
            if (user == null) {
                return NotFound();
            }

            var group = _mapper.Map<Group>(groupForCreationDto);
            group.UserId = userId;
            _groupRepo.Add(group);
            _groupRepo.SaveChanges();

            return CreatedAtRoute("GetGroup", new { userId, groupId = group.Id },
                _mapper.Map<GroupDto>(group));
        }

        [HttpPost("{groupId}")]
        public ActionResult UpdateGroup(int userId, int groupId,
            [FromBody] JsonPatchDocument<GroupForCreationDto> patchDocument) {

            var user = _userRepo.Get(userId, true);
            if (user == null) {
                return NotFound();
            }

            var group = user.Groups.SingleOrDefault(x => x.Id == groupId);
            if (group == null) {
                return NotFound();
            }
            group = _groupRepo.LoadRefrencesTypes(group);
            var patchDto = _mapper.Map<GroupForCreationDto>(group);
            try {
                patchDocument.ApplyToCustom(group, ModelState, _mapper);
            }
            catch (PatchIdNotFoundException e) {
                return NotFound(e.Message);
            }

            if (!TryValidateModel(patchDto)) {
                return ValidationProblem(ModelState);
            }

            _groupRepo.Update(group);
            _groupRepo.SaveChanges();

            return NoContent();
        }

        public override ActionResult ValidationProblem(
        [ActionResultObjectValue] ModelStateDictionary modelStateDictionary) {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
