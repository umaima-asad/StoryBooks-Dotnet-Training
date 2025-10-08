using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StoryBooks.Infrastructure.Data;
using StoryBooks.Application.DTOs;
using StoryBooks.Domain.Models;
using StoryBooks.Requirements;
using StoryBooks.Application.Services;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Text;
using StoryBooks.Application;
using StoryBooks.Infrastructure;
public class Program
{
    public static async Task Main(string[] args)
    {
        IdentityModelEventSource.ShowPII = true;
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "StoryBooks API",
                Version = "v1",
                Description = "API documentation for the StoryBooks Web API"
            });
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            });
            c.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        builder.Services.AddIdentityApiEndpoints<UsersModel>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<StoryBookContext>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CORS Testing", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("CanEditWithoutCover", policy =>
                policy.Requirements.Add(new CanEditNullCoverImageRequirement()));
        });
        builder.Services.AddScoped<IAuthorizationHandler, CanEditNullCoverImageHandler>();
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.MapIdentityApi<UsersModel>();

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseCors("CORS Testing");

        app.UseAuthorization();

        app.MapControllers();

        app.MapPost("/custom-register", async (
            UserManager<UsersModel> userManager,
            RoleManager<IdentityRole> roleManager,
            RegisterDto request) =>
        {
            var user = new UsersModel
            {
                UserName = request.Email,
                Email = request.Email,
                Fullname = request.Fullname
            };

            var defaultRole = "Student";

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);

            // Ensure role exists
            if (!await roleManager.RoleExistsAsync(defaultRole))
            {
                await roleManager.CreateAsync(new IdentityRole(defaultRole));
            }

            // Assign default role
            await userManager.AddToRoleAsync(user, defaultRole);

            return Results.Ok(new { Message = "User registered and assigned 'Student' role" });
        });

        using (var scope = app.Services.CreateScope())
        {
            var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Librarian", "Student" };
            foreach (var role in roles)
            {
                if (!await RoleManager.RoleExistsAsync(role))
                    await RoleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<UsersModel>>();

            string email = "librarian@gmail.com";
            string password = "Librarian@123";

            if (await UserManager.FindByEmailAsync("admin@gmail.com") == null)
            {
                var user = new UsersModel
                {
                    Fullname = "Admin User",
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user, "Librarian");
                }
            }

        }

        app.Run();

    }
}