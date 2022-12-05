using System.Security.Claims;

namespace TaxComputingProject.Utils;

public class HttpContextAccessorUtil
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextAccessorUtil(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetId()
    {
        string result = string.Empty;
        if (_httpContextAccessor.HttpContext != null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
        }

        int.TryParse(result, out var userId);
        return userId;
    }
}