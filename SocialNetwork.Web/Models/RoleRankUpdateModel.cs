namespace SocialNetwork.Web.Models;

public class RoleRankUpdateModel
{
    public int ChatId { get; set; }
    public List<RoleRankModel> RoleRanksModel { get; set; } = null!;
}