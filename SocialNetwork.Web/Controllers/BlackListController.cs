using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Helpers;
using SocialNetwork.Web.Models;
using System.Collections.Generic;

namespace SocialNetwork.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BlackListController : ControllerBase
    {
        private readonly ILogger<BlackListController> _logger;
        private readonly TokenHelper _tokenHelper;
        private readonly IMapper _mapper;
        private readonly IBlackListService _blackListService;

        public BlackListController(IBlackListService blackListService, TokenHelper tokenHelper, ILogger<BlackListController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _tokenHelper = tokenHelper;
            _blackListService = blackListService;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddUserToBlackList([FromQuery] int wantToBanId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _blackListService.AddUserToBlackListAsync(userId, wantToBanId, cancellationToken);
            return Ok();
        }
        [HttpDelete("del")]
        public async Task<IActionResult> DelUserBlackList([FromQuery] int bannedID, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _blackListService.DeleteUserFromBlackListAsync(userId, bannedID, cancellationToken);
            return Ok();
        }

        [HttpGet("getBannedUsers")]
        public async Task<IActionResult> GetBannedUsers([FromQuery] int userId, CancellationToken cancellationToken)
        {
            var loggedInUserId = User.GetUserId();
            if (userId != loggedInUserId)
            {
                return Unauthorized();
            }
            var userModels = await _blackListService.GetAllBannedUser(userId, cancellationToken);
            return Ok(_mapper.Map<List<BannedUserViewModel>>(userModels));
        }
    }
}