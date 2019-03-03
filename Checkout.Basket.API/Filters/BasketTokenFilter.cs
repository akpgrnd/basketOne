using Checkout.Basket.API.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkout.Basket.API.Filters
{
    public class BasketTokenFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Guid? basketId = context.HttpContext.User.GetBasketToken();
            if (basketId == null)
            {
                context.Result = new ContentResult
                {
                    Content = Constants.Claims.BasketToken,
                    StatusCode = 400
                };
            }
            else
            {
                var result = await next();
            }
        }
    }
}
