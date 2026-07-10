using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LuckySpin.Auth
{
    public class AdminApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string HeaderName = "X-Admin-Api-Key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var expectedKey = config["Admin:ApiKey"];

            if (string.IsNullOrEmpty(expectedKey))
            {
                context.Result = new ObjectResult(new { message = "Admin API Key chưa được cấu hình trên server." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedKey)
                || !ConstantTimeEquals(providedKey.ToString(), expectedKey))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Thiếu hoặc sai Admin API Key." });
                return;
            }

            await next();
        }

        private static bool ConstantTimeEquals(string provided, string expected)
        {
            var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(provided));
            var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
            return CryptographicOperations.FixedTimeEquals(providedHash, expectedHash);
        }
    }
}