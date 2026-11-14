using HookRelay.Api.Destinations;

namespace HookRelay.Tests.Destinations;

public sealed class FakeIdGenerator : IIdGenerator
{
    private int _next = 1;

    public string Next(string prefix) => $"{prefix}_{_next++}";
}

public sealed class FakeSecretGenerator : ISecretGenerator
{
    private int _next = 1;

    public string NewSecret() => $"whsec_test_{_next++}";
}