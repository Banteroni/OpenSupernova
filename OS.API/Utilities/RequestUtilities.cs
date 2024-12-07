using System.Security.Claims;

namespace OS.API.Utilities
{
    public static class RequestUtilities
    {
        public static Guid GetUserId(HttpContext context)
        {
            return Guid.Parse(context.User.Claims.First(x => x.Type == ClaimTypes.PrimarySid).Value);
        }
    }
}
