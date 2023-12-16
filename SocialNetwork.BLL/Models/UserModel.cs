using Scriban.Runtime;
using SocialNetwork.BLL.Extensions;
using SocialNetwork.BLL.Models.Enums;

namespace SocialNetwork.BLL.Models;

public class UserModel : BaseModel
{
    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public OnlineStatus OnlineStatus { get; set; }

    public bool IsEnabled { get; set; }

    public ProfileModel Profile { get; set; } = null!;

    public AuthorizationInfoModel AuthorizationInfo { get; set; } = null!;

    public IEnumerable<BlackListModel> BlackList { get; set; } = null!;

    public IScriptObject ToScriptObject() {

        IScriptObject data = new ScriptObject();
        data.SetValue("name", Profile.Name, true);
        data.SetValue("email", Profile.Email, true);

        var link = $"{Environment.GetEnvironmentVariable("ASPNETCORE_URLS")}/api/User/activation/{Id.ToString().ToBase64()}";
        
        data.SetValue("link", link, true);
        

        return data;
    }
    public IScriptObject ToScriptObject_ResetPass(string linkToFront, string key, string iv) {

        IScriptObject data = new ScriptObject();
        var link = $"{linkToFront}/reset-password/{Id.ToString().Encrypt(key, iv)}";
        data.SetValue("link", link, true);
        return data;
    }
}