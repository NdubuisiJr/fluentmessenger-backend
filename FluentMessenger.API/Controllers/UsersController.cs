using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentMessenger.API.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase {
        private readonly IRepository<User> _userRepo;
        private readonly IMapper _mapper;
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;

        public UsersController(IRepository<User> repositoryService,
            IMapper mapper, ISecurityService securityService, IConfiguration configuration) {
            _userRepo = repositoryService;
            _mapper = mapper;
            _securityService = securityService;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets all Users.\
        /// Requires Bearer Token on the Authorization Header
        /// </summary>
        /// <returns>
        ///  An array of users
        /// </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<UserDto>> GetUsers() {
            return Ok(_mapper.Map<IEnumerable<UserDto>>(_userRepo.GetAll(true)));
        }

        /// <summary>
        /// Returns a UserDto give the user's Id.\
        /// Requires Bearer Token on the Authorization Header
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns>A userDto</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{userId}", Name = "GetUser")]
        public ActionResult<UserDto> GetUser(int userId) {
            var user = _userRepo.Get(userId,true);
            if (user == null) {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDto>(user));
        }

        /// <summary>
        /// Creates a user
        /// </summary>
        /// <param name="userForCreation">The object required for creating a user</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<UserDto> CreateUser(
            [FromBody] UserForCreationDto userForCreation) {

            if (userForCreation == null) {
                return BadRequest();
            }

            var result = _userRepo.GetAll().FirstOrDefault(x => x.Email.ToLower() == userForCreation.Email.Trim().ToLower()
            || x.Phone.ToLower() == userForCreation.Phone.Trim().ToLower());

            if (result != null) {
                if(result.IsVerified)
                    return UnprocessableEntity(new {
                        message = "Email or phone number already exists.",
                        status= "Verified already, Please login",
                    });
                else
                    return UnprocessableEntity(new {
                        message = "Please verify your account",
                        status = "Not verified",
                        user = _mapper.Map<UserDto>(result)
                    });
            }

            var user = SecureUser(_mapper.Map<User>(userForCreation));
            _userRepo.Add(user);
            _userRepo.SaveChanges();

            var userDto = GiveOutResources(_mapper.Map<UserDto>(user));
            return CreatedAtRoute("GetUser", new { userId = userDto.Id }, userDto);
        }

        /// <summary>
        /// Carries out a partial update a user model.\ 
        /// Requires Bearer Token on the Authorization Header
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="userForUpdatedDocument">The json patch document 
        /// required for updating the user</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request (this request updates the User's SMSCredit) \ 
        /// [ \
        ///   { \
        ///      "op": "replace", \
        ///     "path": "/SMSCredit", \
        ///     "value": "345" \
        ///    } \
        /// ] 
        /// </remarks>
        [HttpPost("{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult UpdateUser(int userId,
            JsonPatchDocument<UserForUpdateDto> userForUpdatedDocument) {

            var user = _userRepo.Get(userId, true);

            if (user == null) {
                return NotFound();
            }

            var firstSpecialOperation = userForUpdatedDocument.Operations
                    .FirstOrDefault(x => x.OperationType == OperationType.Replace &&
                    x.path.ToLower() == "/password");

            var patchDto = _mapper.Map<UserForUpdateDto>(user);
            userForUpdatedDocument.ApplyTo(patchDto, ModelState);
            if (!TryValidateModel(patchDto)) {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(patchDto, user);

            if (firstSpecialOperation is { }) {
                var newUser = SecureUser(user);
                newUser.IsVerified = true;
            }

            _userRepo.Update(user);
            _userRepo.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// This Deletes a user.\
        /// Requires Bearer Token on the Authorization Header
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteUser(int userId) {
            var user = _userRepo.Get(userId);
            if (user == null) {
                return NotFound();
            }

            _userRepo.Delete(user);
            _userRepo.SaveChanges();
            return NoContent();
        }

        public override ActionResult ValidationProblem(
        [ActionResultObjectValue] ModelStateDictionary modelStateDictionary) {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        private User SecureUser(User user) {
            var result = _securityService.HashPassword(user.Password, 15);
            var dbPassWord = $"{result.Item1}{ISecurityService.SPLITER}{result.Item2}";
            user.Password = dbPassWord;
            user.IsVerified = false;
            user.VerificationCode = new Random().Next(10000, 99000);
            return user;
        }

        private UserDto GiveOutResources(UserDto userDto) {
            var key = GetOneOrTwo();
            var SmsKey = _configuration.GetConnectionString($"SMSKey{key}");
            var SmsId = _configuration.GetConnectionString($"SMSId{key}");
            var password = _configuration.GetConnectionString("password");
            userDto.Token = $"{password}#NdubuisiJr@2severKing*{ISecurityService.SPLITER}{SmsKey}{ISecurityService.SPLITER}{SmsId}";
            return userDto;
        }
        private int GetOneOrTwo() {
            return new Random().Next(1, 3);
        }
    }
}
