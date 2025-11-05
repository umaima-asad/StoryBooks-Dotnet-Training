using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StoryBooks.Application.DTOs;
using StoryBooks.Application.Interfaces;
using StoryBooks.Application.Services;
using StoryBooks.Application.Validators;
using StoryBooks.Domain.Interfaces;

namespace StoryBooks.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IStoryBookServices, StoryBookService>();
            services.AddScoped<IValidator<StoryBookDTO>, StoryBookDTOValidator>();
            services.AddScoped<IValidator<CreateStoryBookDTO>, CreateStoryBookDTOValidator>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantProvider, TenantProvider>();
            return services;
        }
    }
}
