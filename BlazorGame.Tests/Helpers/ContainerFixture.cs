using BlazorGame.Data;
using BlazorGame.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;

namespace BlazorGame.Tests.Helpers
{
    public class ContainerFixture
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ServiceCollection _services;

        public T GetService<T>() => _serviceProvider.GetService<T>();

        public GameSessionService GameService => _serviceProvider.GetService<GameSessionService>();

        public ContainerFixture()
        {
            _services = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true);

            var configuration = builder.Build();

            _services.AddSingleton<ICardProvider, CardProvider>();
            _services.AddSingleton<GameSessionService>();
            _services.AddScoped<IConfiguration>(p => configuration);
            _services.AddSingleton(p => Substitute.For<IHubContext<GameHub>>());

            _serviceProvider = _services.BuildServiceProvider();
        }
    }
}
