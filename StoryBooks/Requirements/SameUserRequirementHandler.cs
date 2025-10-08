using Microsoft.AspNetCore.Authorization;
using StoryBooks.Application.DTOs;
using StoryBooks.Domain.Models;
using System.Security.Claims;

namespace StoryBooks.Requirements
{
    public class CanEditNullCoverImageHandler : AuthorizationHandler<CanEditNullCoverImageRequirement, StoryBookDTO>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEditNullCoverImageRequirement requirement, StoryBookDTO resource)
        {
            if (resource.Cover==null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
