using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using OS.Data.Models;
using OS.Services.Repository;
using System.Security.Claims;

namespace OS.API.Middleware
{
    public class AppendUserMiddleware : IMiddleware
    {
        private readonly IRepository _repository;
        public AppendUserMiddleware(IRepository repository)
        {
            _repository = repository;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            // Check if context has Anonymous user
            var hasAnonymousAttribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.Any(m => m is AllowAnonymousAttribute);

            if (hasAnonymousAttribute != true)
            {
                var sid = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value;
                if (sid == null)
                {
                    await next(context);
                    return;
                }

                // get user from repository
                var user = await _repository.GetAsync<User>(Guid.Parse(sid));
                if (user == null)
                {
                    await next(context);
                }
                else
                    context.Items.Add("User", user);
            }
            await next(context);
        }
    }
}
