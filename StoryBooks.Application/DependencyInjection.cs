using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryBooks.Application.Services;
using StoryBooks.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
namespace StoryBooks.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IStoryBookServices, StoryBookService>();
            services.AddScoped<IValidator<StoryBookDTO>, StoryBookDTOValidator>();
            services.AddScoped<IValidator<CreateStoryBookDTO>, CreateStoryBookDTOValidator>();
            return services;
        }
    }
}
