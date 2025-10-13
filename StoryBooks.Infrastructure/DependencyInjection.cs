using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using StoryBooks.Application.Interfaces;
using StoryBooks.Application.Services;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Infrastructure.Data;
using StoryBooks.Infrastructure.Repositories;


namespace StoryBooks.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<StoryBookContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));
            services.AddScoped<IStoryBookRepository, StoryBookRepository>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("Redis");
                options.InstanceName = "StoryBooks_";
            });
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddHttpClient<PlaceholderService>()
                    .AddPolicyHandler(GetRetryPolicy())
                    .AddPolicyHandler(GetCircuitBreakerPolicy());
            return services;
        }
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() 
                .WaitAndRetryAsync(
                    3, 
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"[Polly] Retry {retryAttempt} after {timespan.TotalSeconds}s due to {outcome.Exception?.Message}");
                    }
                );
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3, 
                    durationOfBreak: TimeSpan.FromSeconds(10), 
                    onBreak: (outcome, breakDelay) =>
                    {
                        Console.WriteLine($"[Polly] Circuit broken for {breakDelay.TotalSeconds}s!");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("[Polly] Circuit closed, back to normal.");
                    });
        }
    }
}
