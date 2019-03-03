using Checkout.Basket.Token.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Checkout.Basket.API.Middleware
{
    public class BasketTokenRequest : BaseMiddleware<BasketTokenRequest>
    {
        private readonly RequestDelegate _next;        

        public BasketTokenRequest(RequestDelegate next, ILogger<BasketTokenRequest> logger) : base (logger)
        {
            this.logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IBasketTokenService tokenService)
        {
            if (httpContext.User == null) 
            {
                await WriteErrorResponse(httpContext);
                return;
            }

            Guid token = Guid.Empty;
            if (httpContext.Request.Headers.TryGetValue(Constants.Headers.Xtoken, out StringValues header))
            {
                Guid.TryParse(header[0], out token);
            }

            if (!await tokenService.BasketExistAsync(token))
            {
                token = await tokenService.InitBasket();
            }

            var claims = new List<Claim> {
                    new Claim(Constants.Claims.BasketToken, token.ToString())
                };

            var appIdentity = new ClaimsIdentity(claims);
            httpContext.User.AddIdentity(appIdentity);

            if (token == Guid.Empty)
            {
                await WriteErrorResponse(httpContext);
                return;
            }

            await _next(httpContext);
        }
    }
}
