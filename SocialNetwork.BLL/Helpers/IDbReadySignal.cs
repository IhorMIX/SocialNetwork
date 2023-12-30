namespace SocialNetwork.BLL.Helpers;

public interface IDbReadySignal
{
    Task Ready { get; }

    void MarkAsReady();
}