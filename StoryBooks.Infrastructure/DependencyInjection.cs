using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using StoryBooks.Application.Services;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Infrastructure.Data;
using StoryBooks.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return services;
        }
    }
}
