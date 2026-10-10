using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;
using ResiliencePackage.Implementations;

namespace Benchmarks;

[MemoryDiagnoser]
public class RetryBenchmarks
{
    private OwnResilience.Interfaces.IAsyncPolicy _ownPolicy = null!;
    private Polly.IAsyncPolicy _pollyPolicy = null!;

    [GlobalSetup]
    public void Setup()
    {
        _ownPolicy = OwnResilience
            .Policy.Handle<InvalidOperationException>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(500 * attempt), null);

        var factory = new RetryPolicyFactory(NullLogger<RetryPolicyFactory>.Instance);
        _pollyPolicy = factory.CreateRetryPolicy<InvalidOperationException>(3);
    }

    [Benchmark(Baseline = true)]
    public async Task OwnResilience_Success()
    {
        await _ownPolicy.ExecuteAsync(
            async ct =>
            {
                await Task.Yield();
            },
            CancellationToken.None
        );
    }

    [Benchmark]
    public async Task Polly_Success()
    {
        await _pollyPolicy.ExecuteAsync(
            async ct =>
            {
                await Task.Yield();
            },
            CancellationToken.None
        );
    }

    [Benchmark]
    public async Task OwnResilience_Retry_Then_Success()
    {
        int attempt = 0;
        await _ownPolicy.ExecuteAsync(
            async ct =>
            {
                await Task.Yield();
                if (++attempt < 3)
                {
                    throw new InvalidOperationException("Test");
                }
            },
            CancellationToken.None
        );
    }

    [Benchmark]
    public async Task Polly_Retry_Then_Success()
    {
        int attempt = 0;
        await _pollyPolicy.ExecuteAsync(
            async ct =>
            {
                await Task.Yield();
                if (++attempt < 3)
                {
                    throw new InvalidOperationException("Test");
                }
            },
            CancellationToken.None
        );
    }
}
