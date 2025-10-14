using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StoryBooks.Application.Interfaces;
using StoryBooks.Infrastructure.Data;

namespace StoryBooks.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DB
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<StoryBookContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // InMemory DB 
                services.AddDbContext<StoryBookContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Mock Redis cache 
                var mockCache = new Mock<IRedisCacheService>();
                mockCache.Setup(c => c.GetData<object>(It.IsAny<string>())).Returns((object)null);
                mockCache.Setup(c => c.SetData(It.IsAny<string>(), It.IsAny<object>()));
                services.AddSingleton(mockCache.Object);

                // Simplify authorization 
                services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, AllowAnonymousHandler>();
            });
        }
    }

    // Always authorize in tests
    public class AllowAnonymousHandler : Microsoft.AspNetCore.Authorization.IAuthorizationHandler
    {
        public Task HandleAsync(Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.Requirements)
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
