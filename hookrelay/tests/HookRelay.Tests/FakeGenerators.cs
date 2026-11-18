using HookRelay.Api.Destinations;

namespace HookRelay.Tests.Destinations;

public sealed class FakeIdGenerator : IIdGenerator
{
    private int _next = 0;

    public string Next(string prefix) => $"{prefix}_{Interlocked.Increment(ref _next)}";
}

public sealed class FakeSecretGenerator : ISecretGenerator
{
    private int _next = 0;

    public string NewSecret() => $"whsec_test_{Interlocked.Increment(ref _next)}";
}