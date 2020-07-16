using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Exceptions;
using FluentMessenger.API.Extensions;
using FluentMessenger.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public GroupsController(IRepository<User> userRepo,
            IRepository<Group> groupRepo, IMapper mapper) {
            _userRepo = userRepo;
            _groupRepo = groupRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all the groups created by a user.\
        /// Requires Bearer Token.
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GroupDto>> GetGroups(int userId) {

            var user = _userRepo.Get(userId);
            if (user == null) {
                return NotFound();
            }

            var groups = _groupRepo.GetAll(true).Where(x => x.UserId == userId);
            return Ok(_mapper.Map<IEnumerable<GroupDto>>(groups));
        }

        /// <summary>
        /// Gets a single group created by a user.\
        /// Requires Bearer token
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="groupId">The Group's Id</param>
        /// <returns></returns>
        [HttpGet("{groupId}", Name = "GetGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Delete's a group created by a user.\
        /// Requires Bearer's Token
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="groupId">The group's Id</param>
        /// <returns></returns>
        [HttpDelete("{groupId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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

        /// <summary>
        /// Create's a group.\
        /// Requires Bearer token
        /// </summary>
        /// <param name="userId">The id for the user creating the group</param>
        /// <param name="groupForCreationDto">The object required for creating a group</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Updates a group.\
        /// Requires Bearer Token.
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="groupId">The group's Id</param>
        /// <param name="patchDocument">The json patch document required</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request (this request updates the group's title) \ 
        /// [ \
        ///   { \
        ///      "op": "replace", \
        ///     "path": "/Title", \
        ///     "value": "New title" \
        ///    } \
        /// ] 
        /// </remarks>
        [HttpPost("{groupId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
