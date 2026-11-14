namespace HookRelay.Api.Destinations;

public interface IIdGenerator
{
    string Next(string prefix);
}

public sealed class GuidIdGenerator : IIdGenerator
{
    public string Next(string prefix)
    {
        return $"{prefix}_{Guid.NewGuid().ToString("N")}";
    }
}

public interface ISecretGenerator
{
    string NewSecret();
}

public sealed class GuidSecretGenerator : ISecretGenerator
{
    public string NewSecret()
    {
        return "whsec_" + Guid.NewGuid().ToString("N");
    }
}