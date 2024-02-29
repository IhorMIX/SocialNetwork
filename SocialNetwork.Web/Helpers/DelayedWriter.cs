using Microsoft.Extensions.Options;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Options;

namespace SocialNetwork.Web.Helpers;

public class DelayedWriter
{
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    private TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
    private long actionCounter;
    private readonly ChunkSizeOfDisconnectedUsers _chunkOption;

    public DelayedWriter(IOptions<ChunkSizeOfDisconnectedUsers> chunkSizeOptions)
    {
        _chunkOption = chunkSizeOptions.Value;
    }

    
    public async Task QueueUserStatusChangeAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            actionCounter++;

            await ProcessChangesAsync(action, cancellationToken);

            if (actionCounter % _chunkOption.ChunkSize == 0)
            {
                if (!completionSource.Task.IsCompleted)
                {
                    completionSource.SetResult(true);
                }

                completionSource = new TaskCompletionSource<bool>();

                await Task.Delay(_chunkOption.Delay, cancellationToken);
            }
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
            // completionSource.SetResult(true); 
        }
        catch (Exception ex)
        {
            completionSource.SetException(ex); 
        }
    }
}
