using SocialNetwork.DAL.Entity;

namespace SocialNetwork.Web.Helpers;

public class DelayedWriter
{
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    private TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();

    public async Task QueueUserStatusChangeAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        await semaphore.WaitAsync();
        try
        {
            if (!completionSource.Task.IsCompleted)
            {
                completionSource.SetResult(true);
            }

            await Task.Delay(10000); 
            
            completionSource = new TaskCompletionSource<bool>();

            await ProcessChangesAsync(action, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task ProcessChangesAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await action.Invoke(cancellationToken);
            completionSource.SetResult(true); 
        }
        catch (Exception ex)
        {
            completionSource.SetException(ex); 
        }
    }
}
