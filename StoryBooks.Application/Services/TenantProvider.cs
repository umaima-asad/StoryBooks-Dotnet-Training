using Microsoft.AspNetCore.Http;
using StoryBooks.Application.Interfaces;
using StoryBooks.Domain.Interfaces;

namespace StoryBooks.Application.Services;

public sealed class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IUsersModelRepository _usersModelRepository;
    public TenantProvider(IHttpContextAccessor httpContextAccessor, IRedisCacheService redisCacheService, IUsersModelRepository usersModelRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _redisCacheService = redisCacheService;
        _usersModelRepository = usersModelRepository;
    }
    private const string UserIDClaim = "user_id";

    public Guid UserID
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(UserIDClaim)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new ApplicationException("User ID claim is missing or invalid.");
            }
            return userId;
        }
    }
    public async Task<int> GetTenantId()
    {
        var cacheKey = $"User_Tenant_{UserID}";
        int TenantIdFromCache = _redisCacheService.GetData<int>(cacheKey);
        if (TenantIdFromCache is not 0)
        {
            return TenantIdFromCache;
        }
        int tenantId = await _usersModelRepository.GetTenantIdByUserIdAsync(UserID.ToString());
        _redisCacheService.SetData<int>(cacheKey, tenantId);
        return tenantId;
    }
}
