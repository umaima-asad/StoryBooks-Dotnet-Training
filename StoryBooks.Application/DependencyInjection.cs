using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using StoryBooks.Application.DTOs;
using StoryBooks.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace StoryBooks.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IStoryBookServices, StoryBookService>();
            services.AddScoped<IValidator<StoryBookDTO>, StoryBookDTOValidator>();
            services.AddScoped<IValidator<CreateStoryBookDTO>, CreateStoryBookDTOValidator>();
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            return services;
        }
    }
}
