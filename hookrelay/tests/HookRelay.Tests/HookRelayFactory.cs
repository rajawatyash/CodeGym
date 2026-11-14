using HookRelay.Api.Destinations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;

namespace HookRelay.Tests.Destinations;

public sealed class HookRelayFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IIdGenerator>();
            services.RemoveAll<ISecretGenerator>();
            services.AddSingleton<IIdGenerator, FakeIdGenerator>();
            services.AddSingleton<ISecretGenerator, FakeSecretGenerator>();
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(
                new FakeTimeProvider(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)));
        });
}