using Polly;
using ResiliencePackage.Interfaces;

namespace ResiliencePackage.Implementations;

public class RetryPolicyFactory(ILogger<RetryPolicyFactory> logger) : IRetryPolicyFactory
{
    public IAsyncPolicy CreateRetryPolicy<TException>(int maxRetries = 3)
        where TException : Exception
    {
        return Policy
            .Handle<TException>()
            .WaitAndRetryAsync(
                maxRetries,
                attempt => TimeSpan.FromMilliseconds(500 * attempt),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(
                        "Retry {RetryCount}/{MaxRetries} due to {ExceptionType}: {Message}. Next retry in {Delay}ms",
                        retryCount,
                        maxRetries,
                        typeof(TException).Name,
                        exception.Message,
                        timeSpan.TotalMilliseconds
                    );
                }
            );
    }
}
