using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace FluentMessenger.API.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/")]
    public class LoginController : ControllerBase {
        private readonly IRepository<User> _userRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;

        public LoginController(IRepository<User> repository, IMapper mapper,
            IConfiguration configuration, ISecurityService securityService) {
            _userRepo = repository;
            _mapper = mapper;
            _configuration = configuration;
            _securityService = securityService;
        }

        /// <summary>
        /// Fetches SMS credentials.\
        /// Requires Bearer Token
        /// </summary>
        /// <returns></returns>
        [HttpGet("credentials")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetServerCredentials() {
            var credentials = ReturnServerCredentials();
            return Ok(new {
                SmsKey = credentials.Item1,
                SmsId = credentials.Item2
            });
        }

        /// <summary>
        /// Used for changing user's password. it generates a new verification
        /// code that is used to authenticate the user's credentials.
        /// </summary>
        /// <param name="passwordResetDto">A password reset object</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("resetpassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<UserDto> ResetPassword(
          [FromBody] PasswordResetDto passwordResetDto) {
            if (passwordResetDto is null) {
                return BadRequest();
            }

            var user = _userRepo.GetAll().FirstOrDefault(x => x.Email.ToLower() ==
                passwordResetDto.Email.Trim().ToLower() && x.Phone.ToLower() ==
                passwordResetDto.Phone.Trim().ToLower());

            if (user is null) {
                return BadRequest();
            }

            user.VerificationCode = new Random().Next(10000, 99000);

            _userRepo.Update(user);
            _userRepo.SaveChanges();
            var userDto = GiveOutResources(_mapper.Map<UserDto>(user));
            return Ok(userDto);
        }

        /// <summary>
        /// This methods verifies a user given it's email, phone number and verification code. 
        /// It can be used for first time verification or for password reset verification.
        /// </summary>
        /// <param name="userForVerificationDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("verify")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<UserDto> VerifyContact(
           [FromBody] UserForVerificationDto userForVerificationDto) {
            if (userForVerificationDto is null) {
                return BadRequest();
            }

            var user = _userRepo.GetAll().FirstOrDefault(x => x.Email.ToLower() ==
                userForVerificationDto.Email.Trim().ToLower() && x.Phone.ToLower() ==
                userForVerificationDto.Phone.Trim().ToLower());

            if (user is null || user.VerificationCode != userForVerificationDto.VerificationCode) {
                return BadRequest();
            }

            if (user.IsVerified && !userForVerificationDto.IsPasswordReset) {
                return UnprocessableEntity(new {
                    message = "User is verified already!",
                    status="Verified already, Please login"
                });
            }

            user.IsVerified = true;
            _userRepo.Update(user);
            _userRepo.SaveChanges();
            user = _userRepo.LoadRefrencesTypes(user);
            var userDto = GiveOutResources(_mapper.Map<UserDto>(user), user);
            return Ok(userDto);
        }

        /// <summary>
        /// Logs a user's into the api by generating a unique token for the login.
        /// </summary>
        /// <param name="retrieveUserDto">The object required for login</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<UserDto> RetrieveUser(
            [FromBody] RetrieveUserDto retrieveUserDto) {

            if (retrieveUserDto == null) {
                return BadRequest();
            }

            var user = _userRepo.GetAll().SingleOrDefault(x => x.Email.ToLower() ==
                                retrieveUserDto.Email.Trim().ToLower());
            if (user == null) {
                return NotFound();
            }

            if (!user.IsVerified) {
                return UnprocessableEntity(new {
                    message = "Please verify your account",
                    status = "Not verified",
                    user= _mapper.Map<UserDto>(user)
                });
            }

            var result = user.Password.Split(ISecurityService.SPLITER);
            if (!_securityService.VerifyPassword(retrieveUserDto.PassWord, result[1], result[0])) {
                return Unauthorized("Authorization failed");
            }

            user = _userRepo.LoadRefrencesTypes(user);
            var userDto = GiveOutResources(_mapper.Map<UserDto>(user), user);

            return Ok(userDto);
        }

        private UserDto GiveOutResources(UserDto userDto, User user) {
            (string, string) credentials;
            if (user.Sender is null){
                credentials = ReturnServerCredentials();
            }
            else if(!user.Sender.IsApproved){
                var smsKey = _configuration.GetConnectionString($"SMSKey{user.Sender.KeyId}");
                var smsId = _configuration.GetConnectionString($"SMSId{user.Sender.KeyId}");
                credentials.Item1=smsKey;
                credentials.Item2=smsId;
            }
            else {
                var smsId = user.Sender.SenderId;
                var smsKey = _configuration.GetConnectionString($"SMSKey{user.Sender.KeyId}");
                credentials.Item1 = smsKey;
                credentials.Item2 = smsId;
            }
            var token = _securityService.GenerateJwtToken(user);
            var SmsKey = credentials.Item1;
            var SmsId = credentials.Item2;
            userDto.Token = $"{token}{ISecurityService.SPLITER}{SmsKey}{ISecurityService.SPLITER}{SmsId}";
            return userDto;
        }

        private UserDto GiveOutResources(UserDto userDto) {
            var key = GetOneOrTwo();
            var SmsKey = _configuration.GetConnectionString($"SMSKey{key}");
            var SmsId = _configuration.GetConnectionString($"SMSId{key}");
            var password = _configuration.GetConnectionString("password");
            userDto.Token = $"{password}#NdubuisiJr@2severKing*{ISecurityService.SPLITER}{SmsKey}{ISecurityService.SPLITER}{SmsId}";
            return userDto;
        }

        private (string, string) ReturnServerCredentials() {
            var key = GetOneOrTwo();
            var SmsKey = _configuration.GetConnectionString($"SMSKey{key}");
            var SmsId = _configuration.GetConnectionString($"SMSId{key}");
            return (SmsKey, SmsId);
        }

        private int GetOneOrTwo() {
            return new Random().Next(1, 3);
        }
    }
}
