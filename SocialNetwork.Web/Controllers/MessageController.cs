using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.Web.Extensions;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly IMapper _mapper;
    private readonly IMessageService _messageService;

    public MessageController(IMapper mapper, IMessageService messageService, ILogger<ChatController> logger)
    {
        _mapper = mapper;
        _messageService = messageService;
        _logger = logger;
    }

    [HttpGet("{chatId:int}")]
    public async Task<IActionResult> GetLastMessage(int chatId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var lastMessage = await _messageService.GetLastMessageAsync(userId, chatId, cancellationToken);
        return Ok(_mapper.Map<MessageViewModel>(lastMessage));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMessages([FromQuery] int chatId, [FromQuery] PaginationModel pagination, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var messages = await _messageService.GetMessagesAsync(userId, chatId, pagination, cancellationToken);
        return Ok(_mapper.Map<PaginationResultViewModel<MessageViewModel>>(messages));
    }
    
    
}