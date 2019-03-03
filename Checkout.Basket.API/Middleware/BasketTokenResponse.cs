using Checkout.Basket.API.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Checkout.Basket.API.Middleware
{
    public class BasketTokenResponse : BaseMiddleware<BasketTokenResponse>
    {
        private readonly RequestDelegate _next;

        public BasketTokenResponse(RequestDelegate next, ILogger<BasketTokenResponse> logger) : base(logger)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode > 399) return;
            
            var token = httpContext.User?.GetBasketToken();
            if (token == null || token.Value == Guid.Empty)
            {
                await WriteErrorResponse(httpContext);
                return;
            }

            httpContext.Response.OnStarting(() =>
            {
                httpContext.Response.Headers.Add(Constants.Headers.Xtoken, token.ToString());
                return Task.CompletedTask;
            });
            
            await _next(httpContext);
        }
    }
}
