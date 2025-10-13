using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using StoryBooks.Application.Services;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Infrastructure.Data;

namespace StoryBooks.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // 1️⃣ Remove existing DB
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<StoryBookContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // 2️⃣ Add InMemory DB for test isolation
                services.AddDbContext<StoryBookContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // 3️⃣ Mock Redis cache so tests don’t depend on external service
                var mockCache = new Mock<IRedisCacheService>();
                mockCache.Setup(c => c.GetData<object>(It.IsAny<string>())).Returns((object)null);
                mockCache.Setup(c => c.SetData(It.IsAny<string>(), It.IsAny<object>()));
                services.AddSingleton(mockCache.Object);

                // 4️⃣ Simplify authorization for test purposes
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
