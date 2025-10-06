using Microsoft.AspNetCore.Authorization;
using StoryBooks.Models;
using System.Security.Claims;

namespace StoryBooks.Requirements
{
    public class CanEditNullCoverImageHandler : AuthorizationHandler<CanEditNullCoverImageRequirement, StoryBook>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEditNullCoverImageRequirement requirement, StoryBook resource)
        {
            if (string.IsNullOrEmpty(resource.Cover))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
