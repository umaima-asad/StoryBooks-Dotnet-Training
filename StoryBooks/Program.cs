using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StoryBooks.Data;
using StoryBooks.DTOs;
using StoryBooks.Models;
using StoryBooks.Services;
using System;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "StoryBooks API",
        Version = "v1"
    });

    // Define JWT Bearer scheme
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http, // Important: Http not ApiKey
        Scheme = "bearer", // must be lowercase
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token only (no 'Bearer ' prefix)"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
    });
});

builder.Services.AddDbContext<StoryBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

builder.Services.AddIdentity<UsersModel, IdentityRole>()
        .AddEntityFrameworkStores<StoryBookContext>();
      //  .AddDefaultTokenProviders();

IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
        ValidAudience = builder.Configuration["AppSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!))
    };
    options.Events = new JwtBearerEvents
     {
         OnAuthenticationFailed = context =>
         {
             Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
             return Task.CompletedTask;
         },
         OnTokenValidated = context =>
         {
             Console.WriteLine($" JWT Token validated for {context.Principal.Identity?.Name}");
             return Task.CompletedTask;
         },
         OnChallenge = context =>
         {
             Console.WriteLine(" JWT Challenge triggered");
             return Task.CompletedTask;
         }
     };

});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IStoryBookServices, StoryBookService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IValidator<StoryBookDTO>, StoryBookDTOValidator>();
builder.Services.AddScoped<IValidator<CreateStoryBookDTO>, CreateStoryBookDTOValidator>();
var app = builder.Build();
app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
    {
        Console.WriteLine($"AUTH HEADER: {authHeader}");
    }
    await next();
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseAuthentication();  // Add this line BEFORE UseAuthorization()
app.UseAuthorization();

app.MapControllers();

app.Run();
