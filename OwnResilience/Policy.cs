using OwnResilience.Implementations;
using OwnResilience.Interfaces;

namespace OwnResilience;

public class Context
{
    // Empty class to mimic Polly's Context
}

public class PolicyBuilder
{
    private readonly Func<Exception, bool> _exceptionPredicate;

    internal PolicyBuilder(Func<Exception, bool> exceptionPredicate)
    {
        _exceptionPredicate = exceptionPredicate;
    }

    public IAsyncPolicy WaitAndRetryAsync(
        int retryCount,
        Func<int, TimeSpan> sleepDurationProvider,
        Action<Exception, TimeSpan, int, Context>? onRetry = null
    )
    {
        return new RetryPolicy(retryCount, _exceptionPredicate, sleepDurationProvider, onRetry);
    }
}

public static class Policy
{
    public static PolicyBuilder Handle<TException>()
        where TException : Exception
    {
        return new PolicyBuilder(ex => ex is TException);
    }
}
