using Microsoft.AspNetCore.Authorization;
using StoryBooks.Models;
using System.Security.Claims;

namespace StoryBooks.Requirements
{
    public class CanEditNullCoverImageHandler : AuthorizationHandler<CanEditNullCoverImageRequirement, StoryBook>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEditNullCoverImageRequirement requirement, StoryBook resource)
        {
            if (resource.Cover==null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
