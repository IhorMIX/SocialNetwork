using SocialNetwork.BLL.Models;

namespace SocialNetwork.Web.Models;

public class MessageViewModel
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public ICollection<FileInMessageViewModel>? FileModels { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    
    public int AuthorId { get; set; }
    public int SenderId { get; set; }
    public int ChatId { get; set; }
    public int ToReplyMessageId { get; set; }

    public UserViewModel Author { get; set; } = null!;
    public ChatMemberViewModel Sender { get; set; } = null!;
    
    public ICollection<ReactionViewModel> Reactions { get; set; } = null!;
    public ICollection<MessageReadStatusViewModel> MessageReadStatuses { get; set; } = null!;
}