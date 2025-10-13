using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using StoryBooks.Application;
using StoryBooks.Application.DTOs;
using StoryBooks.Domain.Models;
using StoryBooks.Infrastructure;
using StoryBooks.Infrastructure.Data;
using StoryBooks.Requirements;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

public class Program
{
    public static async Task Main(string[] args)
    {
        IdentityModelEventSource.ShowPII = true;
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        
        
        //swagger
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
        
        
        //Serilog
        Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithSpan()
                    .WriteTo.Console()
                    .WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = "http://localhost:18889";
                        options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc;
                        options.ResourceAttributes = new Dictionary<string, object>
                        {
                            ["service.name"] = "StoryBooks",
                            ["deployment.environment"] = "development"
                        };
                    })
                    .CreateLogger();
        builder.Host.UseSerilog();

        //openTelemetry
        builder.Services.AddOpenTelemetry()
                        .ConfigureResource(r => r.AddService("StoryBooks.API"))
                        .WithMetrics(m =>
                        {
                             m.AddAspNetCoreInstrumentation();
                             m.AddHttpClientInstrumentation();
                             m.AddOtlpExporter(options =>
                             {
                                options.Endpoint = new Uri("http://localhost:18889");
                             });
                        })
                        .WithTracing(t =>
                        {
                            t.AddAspNetCoreInstrumentation();
                            t.AddHttpClientInstrumentation();
                            t.AddEntityFrameworkCoreInstrumentation();
                            t.AddOtlpExporter(options =>
                            {
                                options.Endpoint = new Uri("http://localhost:18889");
                            });
                        });


        //authentication
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
        
        
        //validation
        builder.Services.AddValidatorsFromAssembly(Assembly.Load("StoryBooks.Application"));
        builder.Services.AddFluentValidationAutoValidation();
        
        
        //clean arch
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        
        
        //middlware
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.MapIdentityApi<UsersModel>();

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseCors("CORS Testing");

        app.UseAuthorization();

        app.MapControllers();

        app.MapPost("/custom-register", async (
            UserManager<UsersModel> userManager,
            RoleManager<IdentityRole> roleManager,
            RegisterDTO request) =>
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