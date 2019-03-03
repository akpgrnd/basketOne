using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Checkout.Basket.API.Middleware
{
    public class BaseMiddleware<T> where T : class
    {
        protected ILogger<T> logger;

        public BaseMiddleware(ILogger<T> logger)
        {
            this.logger = logger;
        }

        protected async Task WriteErrorResponse(HttpContext httpContext)
        {
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsync("Server error");
        }
    }
}
