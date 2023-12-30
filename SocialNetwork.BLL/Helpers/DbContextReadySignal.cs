namespace SocialNetwork.BLL.Helpers;

public class DbContextReadySignal : IDbReadySignal
{
    private readonly TaskCompletionSource<object?> _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public Task Ready => _ready.Task;

    public void MarkAsReady()
    {
        _ready.SetResult(null);
    }
}