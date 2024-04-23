namespace SocialNetwork.BLL.Models;

public class MessageModel : BaseModel
{
    public string Text { get; set; } = null!;
    public ICollection<FileInMessageModel>? Files { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public bool IsEdited { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public int CreatorId { get; set; }
    public int SenderId { get; set; }
    public int ChatId { get; set; }
    public int? ToReplyMessageId { get; set; }

    public UserModel Creator { get; set; } = null!;
    public ChatMemberModel Sender { get; set; } = null!;
    public ChatModel Chat { get; set; } = null!;
    public MessageModel? ToReplyMessage { get; set; }
    
    public ICollection<ReactionModel> Reactions { get; set; } = null!;
    public ICollection<MessageReadStatusModel> MessageReadStatuses { get; set; } = null!;
}