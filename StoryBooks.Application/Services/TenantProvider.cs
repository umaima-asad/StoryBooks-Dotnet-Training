using Microsoft.AspNetCore.Http;

namespace StoryBooks.Application.Services;

public sealed class TenantProvider
{
    private const string TenantIdHeader = "X-Tenant-ID";
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public int GetTenantId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return 0;

        var tenantIdHeader = _httpContextAccessor
                                .HttpContext?
                                .Request
                                .Headers[TenantIdHeader];

        if (!tenantIdHeader.HasValue || !int.TryParse(tenantIdHeader.Value,out int tenantId))
        {
            throw new ApplicationException("Tenant ID header is missing.");
        }
        return tenantId;
    }
}
