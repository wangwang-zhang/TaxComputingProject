using System.Security.Claims;

namespace TaxComputingProject.Utils;

public static class HttpContextAccessorUtil
{
    public static int GetId()
    {
        var httpContextAccessor = new HttpContextAccessor();
        string result = string.Empty;
        if (httpContextAccessor.HttpContext != null)
        {
            result = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
        }

        int.TryParse(result, out var userId);
        return userId;
    }
}