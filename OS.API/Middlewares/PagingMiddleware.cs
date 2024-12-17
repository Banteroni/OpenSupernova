using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;
using System.Text;

namespace OS.API.Middlewares
{
    public class PagingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var originalBody = context.Response.Body;
            using var stream = new MemoryStream();
            context.Response.Body = stream;
            await next(context);
            stream.Seek(0, SeekOrigin.Begin);

            // Check if the response is an array
            if (context.Response.ContentType != null && context.Response.ContentType.Contains("application/json"))
            {
                var responseBody = await new StreamReader(stream).ReadToEndAsync();
                try
                {
                    var jsonArray = JsonSerializer.Deserialize<object[]>(responseBody);
                    if (jsonArray != null)
                    {
                        var take = context.Request.Query["take"].FirstOrDefault();
                        var skip = context.Request.Query["skip"].FirstOrDefault();

                        var takeInt = int.TryParse(take, out var takeResult) ? takeResult : jsonArray.Length;
                        var skipInt = int.TryParse(skip, out var skipResult) ? skipResult : 0;

                        var pagedArray = jsonArray.Skip(skipInt).Take(takeInt).ToArray();
                        var pagedJson = JsonSerializer.Serialize(pagedArray);
                        context.Response.Body = originalBody;
                        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(pagedJson));
                    }
                }
                catch (JsonException)
                {
                    context.Response.Body = originalBody;
                    await context.Response.Body.WriteAsync(stream.ToArray());
                }
            }
            else
            {
                context.Response.Body = originalBody;
                await context.Response.Body.WriteAsync(stream.ToArray());
            }
        }
    }
}
