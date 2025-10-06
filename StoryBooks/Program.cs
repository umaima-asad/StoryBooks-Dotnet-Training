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
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Text;
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

builder.Services.AddDbContext<StoryBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

builder.Services.AddIdentityApiEndpoints<UsersModel>()
        .AddEntityFrameworkStores<StoryBookContext>();

builder.Services.AddAuthorization();

builder.Services.AddScoped<IStoryBookServices, StoryBookService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IValidator<StoryBookDTO>, StoryBookDTOValidator>();
builder.Services.AddScoped<IValidator<CreateStoryBookDTO>, CreateStoryBookDTOValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapIdentityApi<UsersModel>();

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseAuthorization();

app.MapControllers();

app.Run();
