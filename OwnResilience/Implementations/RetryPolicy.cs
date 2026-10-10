using OwnResilience.Interfaces;

namespace OwnResilience.Implementations;

internal sealed class RetryPolicy(
    int retryCount,
    Func<Exception, bool> shouldHandle,
    Func<int, TimeSpan> delayProvider,
    Action<Exception, TimeSpan, int, Context>? onRetry
) : IAsyncPolicy
{
    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default
    )
    {
        for (var attempt = 0; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await action(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (attempt < retryCount && shouldHandle(ex))
            {
                var retryNumber = attempt + 1;
                var delay = delayProvider(retryNumber);
                var context = new Context();

                onRetry?.Invoke(ex, delay, retryNumber, context);

                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default
    ) =>
        ExecuteAsync<object?>(
            async token =>
            {
                await action(token).ConfigureAwait(false);
                return null;
            },
            cancellationToken
        );
}
