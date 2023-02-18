using ChatGPTBot.Db;
using ChatGPTBot.Interfaces;
using ChatGPTBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = CreateHostBuilder(args).Build();
host.Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder =>
        {
            var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .AddJsonFile("appsettings.json")
            .Build();
            builder.Sources.Clear();
            builder.AddConfiguration(configuration);
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService(x => new TelegramBotService(
                x.GetRequiredService<IMessageHandler>(),
                x.GetRequiredService<ApplicationContext>(),
                hostContext.Configuration["TelegramToken"]));

            services.AddDbContext<ApplicationContext>();

            services.AddSingleton<IMessageHandler, ChatGPTService>(x => 
                new ChatGPTService(hostContext.Configuration["ChatGptApiKey"]));
        });