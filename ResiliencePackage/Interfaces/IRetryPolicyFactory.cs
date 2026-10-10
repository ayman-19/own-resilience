using Polly;

namespace ResiliencePackage.Interfaces;

public interface IRetryPolicyFactory
{
    IAsyncPolicy CreateRetryPolicy<TException>(int maxRetries = 3)
        where TException : Exception;
}
