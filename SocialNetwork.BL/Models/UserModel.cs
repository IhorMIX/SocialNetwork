using Scriban.Runtime;
using SocialNetwork.BL.Extensions;
using SocialNetwork.BL.Models.Enums;

namespace SocialNetwork.BL.Models;

public class UserModel : BaseModel
{
    public string Login { get; set; }

    public string Password { get; set; }

    public OnlineStatus OnlineStatus { get; set; }

    public bool IsEnabled { get; set; }

    public ProfileModel Profile { get; set; }

    public AuthorizationInfoModel AuthorizationInfo { get; set; }

    public IEnumerable<BlackListModel> BlackList { get; set; }
    

    public IScriptObject ToScriptObject() {

        IScriptObject data = new ScriptObject();
        data.SetValue("name", Profile.Name, true);
        data.SetValue("email", Profile.Email, true);

        var link = $"{Environment.GetEnvironmentVariable("ASPNETCORE_URLS")}/api/User/activation/{Id.ToString().ToBase64()}";
        
        data.SetValue("link", link, true);
        

        return data;
    }
}