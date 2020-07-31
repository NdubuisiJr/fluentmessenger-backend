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
    [Route("api/users/{userId}/sender")]
    public class SenderController : ControllerBase{
        private readonly IRepository<User> _userRepo;
        private readonly IMapper _mapper;

        public SenderController(IRepository<User> userRepository, IMapper mapper) {
            _userRepo = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all the sender Ids.\
        /// Requires Bearer token
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<SenderIdDto>> GetAll(int userId) { 
            var user = _userRepo.Get(userId);
            if (user is null) {
                return NotFound();
            }
            var senders = _userRepo.GetAll(true).Select(x => x.Sender);
            return Ok(_mapper.Map<IEnumerable<SenderIdDto>>(senders));
        }


        /// <summary>
        /// Gets the sender Id for a user.\
        /// Requires Bearer token
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="id">The user's Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SenderIdDto> Get(int userId,int id) { 
            var user = _userRepo.Get(userId,true);
            if (user is null) {
                return NotFound();
            }

            if (user.Sender is null){
                return NotFound();
            }
            
            return Ok(_mapper.Map<SenderIdDto>(user.Sender));
        }


        /// <summary>
        /// Registers a requested Id to a user's profile.\
        /// Requires a Bearer token
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="senderIdDto">The senderId registration object</param>
        /// <param name="isForApproval">The flag to switch between setting a new Sender Id
        /// or checking the approval of the one already set</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<SenderIdDto> RegisterId(int userId, bool isForApproval,
            [FromBody] SenderIdDto senderIdDto) {

            if (senderIdDto is null) {
                return BadRequest();
            }

            var user = _userRepo.Get(userId, true);
            if (user is null) {
                return NotFound();
            }

            if (!isForApproval) {
                if (user.Sender is { }) {
                    return UnprocessableEntity();
                }

                var sender = _mapper.Map<Sender>(senderIdDto);
                user.Sender = sender;
                _userRepo.Update(user);
                _userRepo.SaveChanges();

                return NoContent();
            }
            else {
                if (user.Sender is null ) {
                    return UnprocessableEntity();
                }

                var sender = _mapper.Map<Sender>(senderIdDto);
                user.Sender.IsApproved = sender.IsApproved;
                _userRepo.Update(user);
                _userRepo.SaveChanges();

                return NoContent();
            }
        }
    }
}
